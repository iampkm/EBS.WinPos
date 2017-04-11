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
using EBS.WinPos.Service.Dto;
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
            var dataList = e.Argument as List<DataItem>;
            worker.ReportProgress(50);
            if (dataList == null)
            {
                _syncService.LoadDataByName();
            }
            else {
                foreach (var item in dataList)
                {
                    _syncService.LoadDataByName(item.Value);
                }
            }
           
            worker.ReportProgress(100);
        }

        private void btnDownloadAll_Click(object sender, EventArgs e)
        {
            SetButton(false);
            _backWorker.RunWorkerAsync(null); 
        }

        private void btnDownloadProduct_Click(object sender, EventArgs e)
        {
            SetButton(false);
            //获取选择的数据项
            List<DataItem> checkItems = new List<DataItem>();
            for (int i = 0; i < this.chkList.Items.Count; i++)
            {
                if (this.chkList.GetItemChecked(i))
                {
                    var item = this.chkList.Items[i] as DataItem;
                    checkItems.Add(item);
                }
            }
            if (checkItems.Count == 0) {
                MessageBox.Show("请选择要下载的数据", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            _backWorker.RunWorkerAsync(checkItems); 
        }

        private void SetButton(bool enabledValue)
        {
            btnDownloadAll.Enabled = enabledValue;
            btnDownloadProduct.Enabled = enabledValue;
        }

        private void frmDownload_Load(object sender, EventArgs e)
        {
            List<DataItem> list = new List<DataItem>();
            list.Add(new DataItem("商品信息", "Product"));
            list.Add(new DataItem("账户信息", "Account"));
            list.Add(new DataItem("门店信息", "Store"));
            list.Add(new DataItem("会员信息", "VipCard"));
            list.Add(new DataItem("会员商品信息", "VipProduct"));
            list.Add(new DataItem("门店价格信息", "ProductStorePrice"));

            this.chkList.DataSource = list;
            this.chkList.DisplayMember = "DisplayName";
            this.chkList.ValueMember = "Value";

           
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < this.chkList.Items.Count; i++)
                this.chkList.SetItemChecked(i, this.chkAll.Checked); 
        }      
                
    }

   
}
