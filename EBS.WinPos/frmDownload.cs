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
using EBS.Infrastructure.Helper;
using System.Threading;
using EBS.WinPos.Domain;
using EBS.WinPos.Service;
namespace EBS.WinPos
{
    public partial class frmProgress : Form
    {
        SyncService _syncService;
        SaleOrderService _saleService;
        WorkScheduleService _workScheduleService;
        // 窗体单例
        private static frmProgress _instance;
        public static frmProgress CreateForm()
        {
            //判断是否存在该窗体,或时候该字窗体是否被释放过,如果不存在该窗体,则 new 一个字窗体  
            if (_instance == null || _instance.IsDisposed)
            {
                _instance = new frmProgress();
            }
            return _instance;
        }
        public frmProgress()
        {
            InitializeComponent();
            _syncService = new SyncService(AppContext.Log);
            _saleService = new SaleOrderService();
            _workScheduleService = new WorkScheduleService();
        }

        private void frmProgress_Load(object sender, EventArgs e)
        {
           
            this.backgroundWorker1.WorkerReportsProgress = true;
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {         
            
            //上传销售数据
            var selectDate = this.dtpDate.Value;
            var orders = _saleService.QueryUploadSaleOrders(selectDate);
            if (orders.Count == 0)
            {
                MessageBox.Show("今天暂时没有可上传的销售数据", "系统信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //班次数据
            var works = _workScheduleService.GetWorkList(selectDate, ContextService.StoreId, ContextService.PosId);
            try
            {
                int totalTasks = orders.Count +works.Count + 1;
                //上传销售数据
                for (var i = 0; i < orders.Count; i++)
                {
                    var model = orders[i];
                    Thread.Sleep(5);
                    _syncService.SendSaleOrder(model);
                    int persent = (int)Math.Round((decimal)(i + 1) / totalTasks * 100, 0);
                    this.backgroundWorker1.ReportProgress(persent);
                }
                //上传班次数据
                for(var j=0;j<works.Count;j++)
                {
                    var workModel = works[j];
                    _syncService.SendWorkSchedule(workModel);
                    int persent = (int)Math.Round((decimal)(orders.Count + 1) / totalTasks * 100, 0);
                    this.backgroundWorker1.ReportProgress(persent);
                }
                //上传汇总数据,报告最后一个任务
                _syncService.UploadSaleSync(this.dtpDate.Value);
               // int LastPersent = (int)Math.Round((decimal)(totalTasks) / totalTasks * 100, 0);
                this.backgroundWorker1.ReportProgress(100);
            }
            catch (Exception ex)
            {
                AppContext.Log.Error(ex);
            }

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar1.Value = e.ProgressPercentage;
            // Set the text.
            this.lblMsg.Text =string.Format("已上传:{0}%", e.ProgressPercentage.ToString());
        }

        private void btnSaleSync_Click(object sender, EventArgs e)
        {
            this.backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //this.Close();
        }
    }
}
