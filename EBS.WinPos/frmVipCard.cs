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
    public partial class frmVipCard : Form
    {
        public frmPos PosFrom { get; set; }
        private static frmVipCard _instance;
        public static frmVipCard CreateForm()
        {
            //判断是否存在该窗体,或时候该字窗体是否被释放过,如果不存在该窗体,则 new 一个字窗体  
            if (_instance == null || _instance.IsDisposed)
            {
                _instance = new frmVipCard();
            }
            return _instance;
        }
        public frmVipCard()
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
