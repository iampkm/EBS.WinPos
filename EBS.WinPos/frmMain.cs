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
namespace EBS.WinPos
{
    public partial class frmMain : Form
    {
        frmLogin _loginFrom;
        public frmMain()
        {
            InitializeComponent();

            
        }
             

        private void frmMain_Load(object sender, EventArgs e)
        {
            _loginFrom = new frmLogin();
            ContextService.AddFrom(this);
            ContextService.AddFrom(_loginFrom);
            _loginFrom.ShowDialog(this);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //收银台
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            ContextService.SignOut();
            _loginFrom.ShowDialog();

        }
    }
}
