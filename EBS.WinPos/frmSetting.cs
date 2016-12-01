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
namespace EBS.WinPos
{
    public partial class frmSetting : Form
    {
        SettingService _settingService;
        StoreService _storeService;
        PosSettings _currentSetting;
        
        public frmSetting()
        {
            InitializeComponent();

            _settingService = new Service.SettingService();
            _storeService = new Service.StoreService();
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
    }
}
