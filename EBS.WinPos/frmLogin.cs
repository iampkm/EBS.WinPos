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
using EBS.Infrastructure.Log;
using EBS.Infrastructure;
namespace EBS.WinPos
{
    public partial class frmLogin : Form
    {
       Sunisoft.IrisSkin.SkinEngine SkinEngine = new Sunisoft.IrisSkin.SkinEngine();
        AccountService _accountService;
        WorkScheduleService _wrokService;
        SettingService _settingService;

        private static frmLogin _instance;
        public static frmLogin CreateForm()
        {
            //判断是否存在该窗体,或时候该字窗体是否被释放过,如果不存在该窗体,则 new 一个字窗体  
            if (_instance == null || _instance.IsDisposed)
            {
                _instance = new frmLogin();
            }
            return _instance;
        }
        public frmLogin()
        {
            InitializeComponent();

            _accountService = new AccountService();
            _wrokService = new WorkScheduleService();
            _settingService = new SettingService();
        }      

        private void btnLogin_Click(object sender, EventArgs e)
        {           

            var user = _accountService.Login(this.txtUserName.Text, this.txtPasswrod.Text);
            var setting = _settingService.GetSettings();
            ContextService.SetCurrentAccount(user,setting.StoreId,setting.PosId);
            var worker = _wrokService.GetWorking(ContextService.StoreId, setting.PosId);
            if (worker == null)  
            {
                ShowWrokForm();
            }
            else
            {
                if (worker.CreatedBy == ContextService.CurrentAccount.Id)
                {
                    ShowPosForm();
                }
                else
                {
                    ShowWrokForm();
                }
            }
            this.Hide();   
        }

        public void ShowWrokForm()
        {
            frmMain main = frmMain.CreateForm();
            main.Show();

            frmWork workForm = frmWork.CreateForm();
            workForm.MdiParent = main;
            workForm.Show();
        }

        public void ShowPosForm()
        {
            frmPos posForm = frmPos.CreateForm();
            posForm.Show();
        }

        private void frmLogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            DialogResult result = MessageBox.Show("你确定关闭pos系统？", "系统信息", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                return;
            }

            Application.Exit();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            // 加载皮肤
          
           SkinEngine.SkinFile = Application.StartupPath + @"/Skins/mp10.ssk";
        }

       
    }
}
