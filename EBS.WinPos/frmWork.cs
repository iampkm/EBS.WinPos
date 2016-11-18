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
           var worker=  _service.GetWorking(ContextService.CurrentAccount.StoreId,1);
           if (worker == null)
           {
               //没人上班
              this.btnEnd.Enabled = false;
           }
           else {
              this.btnBegin.Enabled = false;
              this.lblCreatedBy.Text = "当班人：" + worker.CreatedByName;
              this.lblTime.Text = worker.StartDate.ToShortDateString();
           }
           this.btnEnd.Text = worker.CreatedBy == ContextService.CurrentAccount.Id ? "下 班" : "交 班";
          
        }

        private void btnBegin_Click(object sender, EventArgs e)
        {

            _service.BeginWork(ContextService.CurrentAccount, 1);

            //收银前台
            frmPos posForm = new frmPos();
            ContextService.AddFrom(posForm);
            posForm.MdiParent = ContextService.ParentForm;
            posForm.Show();
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            _service.EndWork(ContextService.CurrentAccount, 1);
        }
    }
}
