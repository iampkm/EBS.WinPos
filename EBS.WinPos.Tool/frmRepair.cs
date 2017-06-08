using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EBS.WinPos.Domain;
using EBS.WinPos.Service;
using EBS.WinPos.Service.Task;
using EBS.WinPos.Domain.Entity;
using EBS.Infrastructure;

namespace EBS.WinPos.Tool
{
    public partial class frmRepair : Form
    {
        public frmRepair()
        {
            InitializeComponent();
            _syncService = new SyncService(AppContext.Log);
        }

        DapperContext _sqliteDB;
        MySqlDBContext _serverDB;
        List<SaleOrder> _entitys;
        List<SaleOrder> _serverEntitys;
        SyncService _syncService;
        private void frmRepair_Load(object sender, EventArgs e)
        {

            txtSqlite.Text = @"data source=D:\github\EBS.WinPos\EBS.WinPos\sqlite\posdata.db";
            txtServiceDB.Text = "database=ebs;server=112.74.83.55;uid=root;pwd=guguxiang@123;";
        }
        private void btnCheck_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(txtSqlite.Text))
            {
                MessageBox.Show("input sqlite string");
                return;
            }
            if (string.IsNullOrEmpty(txtServiceDB.Text))
            {
                MessageBox.Show("input serverDB string");
                return;
            }
            _sqliteDB = new DapperContext();
            _serverDB = new MySqlDBContext(txtServiceDB.Text);
            var selectDate = dtpDate.Value.Date;
            IList<SaleOrder> list = new List<SaleOrder>();
            this.dataGridView1.DataSource = list;
            _entitys = _sqliteDB.Query<SaleOrder>("select * from saleOrder where date(UpdatedOn)=@UpdatedOn and Status in (-1,3)", new { UpdatedOn = selectDate.ToString("yyyy-MM-dd") }).ToList();
            if (_entitys.Count == 0)
            {
                MessageBox.Show("本地无销售数据");
                return;
            }
            var entity = _entitys.FirstOrDefault();
             _serverEntitys = _serverDB.Query<SaleOrder>("select Code from saleOrder where  StoreId=@StoreId and PosId=@PosId and UpdatedOn between @StartDate and @EndDate and Status in (-1,3)",
                   new {  StoreId = entity.StoreId, PosId = entity.PosId, StartDate = selectDate, EndDate = selectDate.AddDays(1) }).ToList();

            foreach (var model in _entitys)
            {
                this.lblMsg.Text = "检查订单：" + model.Code;
               
                if (!_serverEntitys.Exists(n=>n.Code==model.Code))
                {
                    list.Add(model);
                }
            }

            if (list.Count == 0)
            {
                MessageBox.Show("无差异数据");
            }
           
           // list = entitys;
            this.dataGridView1.DataSource = new BindingList<SaleOrder>(list);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var id =Convert.ToInt32(this.dataGridView1.CurrentRow.Cells["Id"].Value);
           var model=  _entitys.FirstOrDefault(n => n.Id == id);
            if (model == null)
            {
                MessageBox.Show("单据不存在");
                return;
            }
            model.Code = model.Code + "0";
            _sqliteDB.ExecuteSql("update SaleOrder set Code=@Code where Id=@Id", new { Code = model.Code, Id = id });

           var items =  _sqliteDB.Query<SaleOrderItem>("select * from SaleOrderItem where SaleOrderId=@Id", new { Id = id }).ToList();
            if (items.Count == 0) {
                MessageBox.Show("明细为空");
                return;
            }

            model.Items = items;

            // 上传
            _syncService.Send(model);

            this.lblMsg.Text = model.Code + "执行完成";

        }

        private void btnUpOrder_Click(object sender, EventArgs e)
        {
            var id = Convert.ToInt32(this.dataGridView1.CurrentRow.Cells["Id"].Value);
            var model = _entitys.FirstOrDefault(n => n.Id == id);
            if (model == null)
            {
                MessageBox.Show("单据不存在");
                return;
            }
            model.Code = model.Code + "0";
            _sqliteDB.ExecuteSql("update SaleOrder set Code=@Code where Id=@Id", new { Code = model.Code, Id = id });

            var items = _sqliteDB.Query<SaleOrderItem>("select * from SaleOrderItem where SaleOrderId=@Id", new { Id = id }).ToList();
            if (items.Count == 0)
            {
                MessageBox.Show("明细为空");
                return;
            }

            model.Items = items;

            // 上传
            _syncService.Send(model);
            MessageBox.Show("执行完毕");
        }

        private void btnUpdateSummary_Click(object sender, EventArgs e)
        {
            var selectDate = dtpDate.Value.Date;
            _syncService.UploadSaleSync(selectDate);
            MessageBox.Show("执行完毕");
        }
    }
}
