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
        public frmWork()
        {
            InitializeComponent();
            _service = new WorkScheduleService();
        }

        private void frmWork_Load(object sender, EventArgs e)
        {
           var worker=  _service.GetWorking(ContextService.CurrentAccount.StoreId,Config.PosId);
           if (worker == null)
           {
               //没人上班
              this.btnEnd.Enabled = false;
           }
           else {
              this.btnBegin.Enabled = false;
              this.lblCreatedBy.Text = worker.CreatedByName + "正在上班" ;
              this.lblTime.Text ="上班时间：" + worker.StartDate.ToString("yyyy-MM-dd HH:mm:ss");
              this.btnEnd.Text = worker.CreatedBy == ContextService.CurrentAccount.Id ? "下 班" : "交 班";
            }
           this.gbUserBegin.Text = "上 班     当前账号：" + ContextService.CurrentAccount.NickName;
           
          
        }

        private void btnBegin_Click(object sender, EventArgs e)
        {
            try
            {
                _service.BeginWork(ContextService.CurrentAccount, Config.PosId);
                //收银前台
                frmPos posForm = new frmPos();
                ContextService.AddFrom(posForm);
                // posForm.MdiParent = ContextService.ParentForm;
                posForm.TopLevel = true;
                posForm.Show();
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
                _service.EndWork(ContextService.CurrentAccount, Config.PosId);
                //关闭收银台
                var posForm= ContextService.GetFrom(typeof(frmPos));
                if (posForm != null)
                {
                    ContextService.RemoveFrom(typeof(frmPos));
                    posForm.Close();
                }
                this.Close();
              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
           
        }

        private void frmWork_FormClosing(object sender, FormClosingEventArgs e)
        {
            ContextService.RemoveFrom(this.GetType());
        }
    }
}
