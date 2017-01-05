using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EBS.WinPos.Service;
using EBS.WinPos.Domain;
namespace EBS.WinPos
{
    public partial class frmWork : Form
    {
        WorkScheduleService _service;
        // 窗体单例
        private static frmWork _instance;
        public static frmWork CreateForm()
        {
            //判断是否存在该窗体,或时候该字窗体是否被释放过,如果不存在该窗体,则 new 一个字窗体  
            if (_instance == null || _instance.IsDisposed)
            {
                _instance = new frmWork();
            }
            return _instance;
        }
        public frmWork()
        {
            InitializeComponent();
            _service = new WorkScheduleService();
        }

        private void frmWork_Load(object sender, EventArgs e)
        {
            var worker = _service.GetWorking(ContextService.StoreId, ContextService.PosId);
            if (worker == null)
            {
                //没人上班
                this.btnEnd.Enabled = false;
                this.lblCreatedBy.Text = " 员工：" + ContextService.CurrentAccount.NickName;
                this.lblTime.Text = " 上班时间：";
            }
            else
            {
                this.btnBegin.Enabled = false;
                this.lblCreatedBy.Text = " 员工：" + worker.CreatedByName + ",正在上班";
                this.lblTime.Text = " 上班时间：" + worker.StartDate.ToString("yyyy-MM-dd HH:mm:ss");
                this.btnEnd.Text = worker.CreatedBy == ContextService.CurrentAccount.Id ? "下 班" : "交 班";
            }
            this.gbUserBegin.Text = "上 班  [当前登录员工：" + ContextService.CurrentAccount.NickName + " 工号：" + ContextService.CurrentAccount.UserName + "]";
        }

        private void btnBegin_Click(object sender, EventArgs e)
        {
            try
            {
                _service.BeginWork(ContextService.CurrentAccount, ContextService.StoreId, ContextService.PosId);
                //收银前台
                frmPos posForm = frmPos.CreateForm();
                posForm.Show();

                var mainForm = frmMain.CreateForm();
                mainForm.Hide();

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            try
            {
                _service.EndWork(ContextService.CurrentAccount, ContextService.StoreId, ContextService.PosId);
                //关闭收银台
                this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void frmWork_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}
