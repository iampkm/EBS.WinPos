using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EBS.WinPos.Domain.Entity;
using EBS.WinPos.Domain;
using EBS.WinPos.Service;
namespace EBS.WinPos
{
    public partial class frmMy : Form
    {
        WorkScheduleService _workService;

        int _curentWrokId = 0;
        public frmMy()
        {
            InitializeComponent();

            _workService = new WorkScheduleService();
        }

        private void frmMy_Load(object sender, EventArgs e)
        {
            dtpDate.Format = DateTimePickerFormat.Custom; //设置为显示格式为自定义
            dtpDate.CustomFormat = "yyyy-MM-dd"; //设置显示格式
            this.dtpDate.Value = DateTime.Now.Date;
            InitData(dtpDate.Value.Date);
        }

        public void InitData(DateTime selectDate)
        {
            //默认查询当天的
            var data = _workService.GetWorkList(selectDate, ContextService.CurrentAccount.StoreId, Config.PosId, ContextService.CurrentAccount.Id);
            this.dgvData.AutoGenerateColumns = false;
           // this.dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvData.DataSource = data;


        }

        private void dgvData_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(e.RowBounds.Location.X,
          e.RowBounds.Location.Y,
          dgvData.RowHeadersWidth - 4,
          e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                dgvData.RowHeadersDefaultCellStyle.Font,
                rectangle,
                Color.Black,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

      
        private void dgvData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvData.Rows.Count > 0)
            {                
               txtCashAmount.Text = this.dgvData.Rows[e.RowIndex].Cells["CashAmount"].Value.ToString();
               _curentWrokId = (int)this.dgvData.Rows[e.RowIndex].Cells["Id"].Value;  
            }
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            InitData(dtpDate.Value.Date);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            InputAmount();
        }

        public void InputAmount()
        {
            if (_curentWrokId == 0)
            {
                MessageBox.Show("请选择一个上班记录");
                return;
            }
            decimal amount = 0m;
            if (decimal.TryParse(txtCashAmount.Text, out amount))
            {
                _workService.InputCashAmount(this._curentWrokId, amount);
                InitData(dtpDate.Value.Date);
                this.txtCashAmount.Text = "";
            }
            else
            {
                MessageBox.Show("金额格式错误！");
            }
        }

        private void txtCashAmount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                InputAmount();
            }
        }
    }
}
