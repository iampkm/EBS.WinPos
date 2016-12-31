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
        
        public frmSetting()
        {
            InitializeComponent();

            _settingService = new Service.SettingService();
            _storeService = new Service.StoreService();
            _service = new SyncService(AppContext.Log);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IPosPrinter _printService = new DriverPrinterService();
            _printService.Print("-------------小票打印机测试 ---------------");
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
           
            _service.DownloadData();
            MessageBox.Show("下载完成", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnUpdateProduct_Click(object sender, EventArgs e)
        {
            //更新商品
            _service.DownloadProductSync();
            MessageBox.Show("下载完成", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSaleSync_Click(object sender, EventArgs e)
        {
            //上传销售数据
            var today= this.dtpDate.Value.ToString("yyyy-MM-dd");
            _service.SaleSyncDaily(today);
            MessageBox.Show("同步完成", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
