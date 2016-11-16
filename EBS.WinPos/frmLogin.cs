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
                //var user = _accountService.Login(this.txtUserName.Text, this.txtPasswrod.Text);
                //ContextService.SetCurrentAccount(user);
               
                //收银前台
                frmPos posForm = new frmPos();
                ContextService.AddFrom(posForm);
                posForm.MdiParent = ContextService.ParentForm; 
                posForm.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                this.lblMsg.Text = ex.Message;
            }
          
        }
    }
}
