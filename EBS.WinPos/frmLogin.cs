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
        AccountService _accountService;

        WorkScheduleService _wrokService;
        public frmLogin()
        {
            InitializeComponent();

            _accountService = new AccountService();
            _wrokService = new WorkScheduleService();
        }      

        private void btnLogin_Click(object sender, EventArgs e)
        {           

            var user = _accountService.Login(this.txtUserName.Text, this.txtPasswrod.Text);
            ContextService.SetCurrentAccount(user);
            var worker = _wrokService.GetWorking(ContextService.CurrentAccount.StoreId, Config.PosId);
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
            frmWork workForm = new frmWork();
            ContextService.AddFrom(workForm);
            workForm.MdiParent = ContextService.ParentForm;
            workForm.Show();
        }

        public void ShowPosForm()
        {
            var posForm = ContextService.GetFrom(typeof(frmPos));
            if (posForm == null)
            {
                posForm = new frmPos();
                ContextService.AddFrom(posForm);
                // posForm.MdiParent = ContextService.ParentForm;
                posForm.TopLevel = true;
                posForm.Show();
               
                
            }
            else
            {
                posForm.Show();
            } 
        }

        private void frmLogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {

        }

       
    }
}
