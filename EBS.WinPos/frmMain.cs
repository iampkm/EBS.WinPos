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
using EBS.WinPos.Service.Dto;

namespace EBS.WinPos
{
    public partial class frmMain : Form
    {
        WorkScheduleService _workService;
        SettingService _settingService;
        PosSettings _setting;
       

        // 窗体单例
        private static frmMain _instance;
        public static frmMain CreateForm()
        {
            //判断是否存在该窗体,或时候该字窗体是否被释放过,如果不存在该窗体,则 new 一个字窗体  
            if (_instance == null || _instance.IsDisposed)
            {
                _instance = new frmMain();
            }
            return _instance;
        }

        public frmMain()
        {
            InitializeComponent();

            _workService = new WorkScheduleService();
            _settingService = new SettingService();
            _setting = _settingService.GetSettings();
           
        }

       
             

        private void frmMain_Load(object sender, EventArgs e)
        {
            if (Config.Allowsetting.Contains(ContextService.CurrentAccount.RoleId))
            {
                toolStripButton6.Visible = true;
            }
            else {
                toolStripButton6.Visible = false;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //收银台
            // 检查当前班次收银员账户是否一致
            var worker = _workService.GetWorking(ContextService.StoreId, _setting.PosId);
            if (worker == null)
            {
                ShowWorkForm();
                MessageBox.Show("请先上班再开始销售", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (worker.CreatedBy != ContextService.CurrentAccount.Id)
            {
                ShowWorkForm();
                MessageBox.Show("请先交接班再开始销售", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            frmPos posForm = frmPos.CreateForm();
            posForm.Show();
            this.Hide();
           
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            // 注销
            ContextService.SignOut();
            Application.Restart();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            ShowWorkForm();  
        }

        private void ShowWorkForm()
        {
            // 交接班          
            frmWork workForm = frmWork.CreateForm();
            workForm.MdiParent = this;
            workForm.Show();    
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            // 我的班次
            frmMy myWork = frmMy.CreateForm();
            myWork.MdiParent = this;
            myWork.Show();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            frmQuery query = frmQuery.CreateForm();
            query.MdiParent = this;
            query.Show();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {

            //设置
            frmSetting setting = frmSetting.CreateForm();
            setting.MdiParent = this;
            setting.Show();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("你确定退出应用程序？", "系统信息", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                return;
            }
            System.Environment.Exit(Environment.ExitCode);
        }

        private void tsbUp_Click(object sender, EventArgs e)
        {            
            frmUpload progress = frmUpload.CreateForm();
            progress.Show();
            
        }

        private void tsbDownLoad_Click(object sender, EventArgs e)
        {
            // 下载数据
            frmDownload download = frmDownload.CreateForm();
            download.Show();
        }
    }
}
