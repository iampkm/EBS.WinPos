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
using EBS.WinPos.Service.Task;
using EBS.Infrastructure;
using EBS.Infrastructure.Helper;
using System.Threading;
namespace EBS.WinPos
{
    public partial class frmMain : Form
    {
        WorkScheduleService _workService;
        SettingService _settingService;
        PosSettings _setting;
        SyncService _syncService;
        SaleOrderService _saleService;

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
            _syncService = new SyncService(AppContext.Log);
            _saleService = new SaleOrderService();
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
            //上传销售数据
             var orders= _saleService.QueryUploadSaleOrders();
             if (orders.Count == 0) {
                 MessageBox.Show("今天暂时没有可上传的销售数据", "系统信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                 return;
             }
             try
             {
                 frmProgress progress = frmProgress.CreateForm();
                 progress.Show();
                 var taskCount = orders.Count();
                 progress.InitProgressBar(taskCount);
                // MultiThreadResetEvent threadEvent = new MultiThreadResetEvent(taskCount);
                 for (var i = 0; i < orders.Count; i++)
                 {
                     var model = orders[i];
                   //  model.SetAre(threadEvent);  // 线程同步
                   //  Thread.Sleep(5);
                     _syncService.SendSaleOrder(model);
                     progress.PerformStep();

                 }
                // threadEvent.WaitAll();
                // threadEvent.Dispose();
                 //上传完毕，关闭

                progress.Close();
             }
             catch (Exception ex)
             {
                 AppContext.Log.Error(ex);
             }

            
        }
    }
}
