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
        PosSettings _currentSetting;
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
            _cmdService = new Service.CommandService();
        }

        private void frmSetting_Load(object sender, EventArgs e)
        {
            //加载设置
            _currentSetting = _settingService.GetSettings();
            if(_currentSetting!=null)
            {
                txtStoreId.Text = _currentSetting.StoreId.ToString();
                txtPosId.Text = _currentSetting.PosId.ToString();
                txtCDKey.Text = _currentSetting.CDKey;
            }
        }
       

        private void btnCommand_Click(object sender, EventArgs e)
        {
            string sql = txtInfo.Text;
            if (string.IsNullOrEmpty(sql))
            {
                return;
            }
            int rows= _cmdService.ExecuteCommand(sql);
            string result = rows > 0 ? "执行成功!" : "执行失败!";
            this.txtInfo.Text = string.Format("{0}影响行数{1}.", result,rows);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SetButton(false);
            CheckInput(txtStoreId, "请输入门店整数编号");
            CheckInput(txtPosId, "请输入收银机整数编号");
            CheckInput(txtCDKey, "请输入授权码");
            int storeId = 0;
            int.TryParse(txtStoreId.Text, out storeId);
            int posId = 0;
            int.TryParse(txtPosId.Text, out posId);
            string cdkey = txtCDKey.Text;

            _settingService.Update(1, storeId.ToString());
            _settingService.Update(2, posId.ToString());
            _settingService.Update(3, cdkey);

            MessageBox.Show("保存成功!", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            SetButton(true);

        }

        public void CheckInput(TextBox input, string message)
        {
            if (string.IsNullOrEmpty(input.Text))
            {
                MessageBox.Show(message, "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void SetButton(bool enabledValue)
        {
            btnSave.Enabled = enabledValue;
        }
    }
}
