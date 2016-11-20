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
namespace EBS.WinPos
{
    public partial class frmLogin : Form
    {
        AccountService _accountService;
        public frmLogin()
        {
            InitializeComponent();

            _accountService = new AccountService();
        }      

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                var user = _accountService.Login(this.txtUserName.Text, this.txtPasswrod.Text);
                ContextService.SetCurrentAccount(user);

                frmWork workForm = new frmWork();
                ContextService.AddFrom(workForm);
                workForm.MdiParent = ContextService.ParentForm;
                workForm.Show();
               
                this.Hide();
            }
            catch (Exception ex)
            {
                this.lblMsg.Text = ex.Message;
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
