using EBS.WinPos.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EBS.WinPos.Service;
using EBS.WinPos.Domain.Entity;
using EBS.WinPos.Domain;
using EBS.WinPos.Service.Dto;
using EBS.WinPos.Service.Task;
using EBS.Infrastructure;
namespace EBS.WinPos
{
    public partial class frmSetting : Form
    {
        SettingService _settingService;
        StoreService _storeService;
        PosSettings _currentSetting;
        SyncService _service ;
        CommandService _cmdService;

        private static frmSetting _instance;
        public static frmSetting CreateForm()
        {
            //判断是否存在该窗体,或时候该字窗体是否被释放过,如果不存在该窗体,则 new 一个字窗体  
            if (_instance == null || _instance.IsDisposed)
            {
                _instance = new frmSetting();
            }
            return _instance;
        }
        public frmSetting()
        {
            InitializeComponent();

            _settingService = new Service.SettingService();
            _storeService = new Service.StoreService();
            _service = new SyncService(AppContext.Log);
            _cmdService = new Service.CommandService();
        }

        private void frmSetting_Load(object sender, EventArgs e)
        {
            var stores = _storeService.GetAll();
            cbbStores.DataSource = stores;
            cbbStores.DisplayMember = "Name";
            cbbStores.ValueMember = "Id";
            cbbStores.SelectedIndex = 0;

            //加载设置
            _currentSetting = _settingService.GetSettings();
            if(_currentSetting!=null)
            {
                txtPosId.Text = _currentSetting.PosId.ToString();
                cbbStores.SelectedValue = _currentSetting.StoreId;
            }

            //设置对账日期
            dtpDate.Format = DateTimePickerFormat.Custom; //设置为显示格式为自定义
            dtpDate.CustomFormat = "yyyy-MM-dd"; //设置显示格式
            this.dtpDate.Value = DateTime.Now.Date;

            this.lblMsg.Text = "";
        }


        private void btnSavePosId_Click(object sender, EventArgs e)
        {
            int posId = 0;
            if (int.TryParse(txtPosId.Text, out posId))
            {
                _currentSetting.PosId = posId;
                _settingService.Update(2, _currentSetting.PosId.ToString());
                MessageBox.Show("修改成功", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("门店编号只能是整数", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }    
        }

        private void btnSaveStoreID_Click(object sender, EventArgs e)
        {
            var storeId = cbbStores.SelectedValue;
            _currentSetting.StoreId = (int)storeId;
            _settingService.Update(1, storeId.ToString());
            MessageBox.Show("修改成功", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
       
           
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {           
            ShowInfo(_service.DownloadData);

            MessageBox.Show("下载完成", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnUpdateProduct_Click(object sender, EventArgs e)
        {
            //更新商品
            ShowInfo(_service.DownloadProductSync);
            MessageBox.Show("下载完成", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSaleSync_Click(object sender, EventArgs e)
        {
            //上传销售数据
            this.lblMsg.Text = "数据处理中...";
            var today= this.dtpDate.Value;
            _service.SaleSyncDaily(today);
            _service.UploadSaleSync(today);
            this.lblMsg.Text = "";
            MessageBox.Show("同步完成", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ShowInfo(Action action)
        {
            this.lblMsg.Text = "数据处理中...";
            action();
            this.lblMsg.Text = "";
        }

        private void btnCommand_Click(object sender, EventArgs e)
        {
            string sql = txtInfo.Text;
            if (string.IsNullOrEmpty(sql))
            {
                return;
            }
            int rows= _cmdService.ExecuteCommand(sql);
            this.txtInfo.Text = string.Format("影响行数{0}", rows);
        }
    }
}
