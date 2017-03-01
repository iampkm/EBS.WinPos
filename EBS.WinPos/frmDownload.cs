using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EBS.WinPos.Service.Task;
using EBS.Infrastructure;
namespace EBS.WinPos
{
    public partial class frmDownload : Form
    {
        // 窗体单例
        private static frmDownload _instance;
        public static frmDownload CreateForm()
        {
            //判断是否存在该窗体,或时候该字窗体是否被释放过,如果不存在该窗体,则 new 一个字窗体  
            if (_instance == null || _instance.IsDisposed)
            {
                _instance = new frmDownload();
            }
            return _instance;
        }

        private BackgroundWorker _backWorker;
        SyncService _syncService;
        public frmDownload()
        {
            InitializeComponent();

            _syncService = new SyncService(AppContext.Log);
            _backWorker = new BackgroundWorker();
            
            _backWorker.DoWork += _backWorker_DoWork;
            _backWorker.ProgressChanged += _backWorker_ProgressChanged;
            _backWorker.RunWorkerCompleted += _backWorker_RunWorkerCompleted;
            _backWorker.WorkerReportsProgress = true;
            this.lblMsg.Text = "下载数据，大约持续1分钟，请耐心等待...";
        }

        void _backWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetButton(true);
            MessageBox.Show("下载完成", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            var loadDataType = (string)e.Argument;
            worker.ReportProgress(50);
            if (loadDataType == "all")
            {
                _syncService.DownloadData();
            }
            else {
                _syncService.DownloadProductSync();
            }
            worker.ReportProgress(100);
        }

        private void btnDownloadAll_Click(object sender, EventArgs e)
        {
            SetButton(false);
            _backWorker.RunWorkerAsync("all"); 
        }

        private void btnDownloadProduct_Click(object sender, EventArgs e)
        {
            SetButton(false);
            _backWorker.RunWorkerAsync("product"); 
        }

        private void SetButton(bool enabledValue)
        {
            btnDownloadAll.Enabled = enabledValue;
            btnDownloadProduct.Enabled = enabledValue;
        }
                
    }
}
