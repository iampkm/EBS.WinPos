using EBS.WinPos.Domain.ValueObject;
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
    public partial class frmRefund : Form
    {
        public frmRefund()
        {
            InitializeComponent();
        }

        private void lstPaymentWay_SelectedValueChanged(object sender, EventArgs e)
        {
            var select = (KeyValuePair<int, string>)this.lstPaymentWay.SelectedItem;
            if (select.Key == (int)PaymentWay.Cash)
            {
                this.lblPayBarCode.Hide();
                this.txtPayBarCode.Hide();
                this.txtPayBarCode.Text = "";
            }
            else
            {
                this.lblPayBarCode.Show();
                this.txtPayBarCode.Show();
            }
        }

        private void frmRefund_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
