using EBS.Infrastructure;
using EBS.WinPos.Service.Task;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EBS.WinPos.Service;
namespace EBS.WinPos
{
    public partial class frmInit : Form
    {
        private static frmInit _instance;
        public static frmInit CreateForm()
        {
            //判断是否存在该窗体,或时候该字窗体是否被释放过,如果不存在该窗体,则 new 一个字窗体  
            if (_instance == null || _instance.IsDisposed)
            {
                _instance = new frmInit();
            }
            return _instance;
        }

        private BackgroundWorker _backWorker;
       
        SettingService _settingService;
        public frmInit()
        {
            InitializeComponent();

           
            _backWorker = new BackgroundWorker();
            _settingService = new SettingService();

            _backWorker.DoWork += _backWorker_DoWork;
            _backWorker.ProgressChanged += _backWorker_ProgressChanged;
            _backWorker.RunWorkerCompleted += _backWorker_RunWorkerCompleted;
            _backWorker.WorkerReportsProgress = true;
            this.lblMsg.Text = "下载数据，大约持续1分钟，请耐心等待...";
            this.lblMsg.Visible = false;
            this.progressBar1.Visible = false;
        }

        void _backWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetButton(true);
           // MessageBox.Show("下载完成", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            var winLogin = frmLogin.CreateForm();
            winLogin.Show();
            this.Hide();
        }

        void _backWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar1.Value = e.ProgressPercentage;
            // Set the text.
            this.lblMsg.Text = string.Format("已下载:{0}%", e.ProgressPercentage.ToString());

        }

        void _backWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            worker.ReportProgress(50);
             SyncService _syncService = new SyncService(AppContext.Log);
            _syncService.LoadDataByName();
            worker.ReportProgress(100);
            System.Threading.Thread.Sleep(500);
        }

        private void btnDownloadAll_Click(object sender, EventArgs e)
        {
            SetButton(false);
            _backWorker.RunWorkerAsync(null);
        }        

        private void SetButton(bool enabledValue)
        {
            btnSave.Enabled = enabledValue;
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

            this.lblMsg.Visible = true;
            this.lblMsg.Text = "开始下载数据";
            System.Threading.Thread.Sleep(1000);
            this.progressBar1.Visible = true;
            _backWorker.RunWorkerAsync(null);
          

        }

        public void CheckInput(TextBox input, string message)
        {
            if (string.IsNullOrEmpty(input.Text)) {
                MessageBox.Show(message, "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; 
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
           
          
        }
    }
}
