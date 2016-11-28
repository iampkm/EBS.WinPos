using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace EBS.WinPos
{
    public partial class frmCustomerId : Form
    {
        public frmPos PosFrom { get; set; }
        public frmCustomerId()
        {
            InitializeComponent();
        }

        private void frmDiscount_Load(object sender, EventArgs e)
        {
            this.txtCode.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveCode();
        }

        public void SaveCode()
        {
            this.PosFrom.SetVipCard(this.txtCode.Text);           
            this.Close();
        }

        private void txtCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SaveCode();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
