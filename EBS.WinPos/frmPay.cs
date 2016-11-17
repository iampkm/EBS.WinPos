using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EBS.WinPos.Service.Dto;
using EBS.Infrastructure.Extension;
using EBS.WinPos.Domain.ValueObject;
using EBS.WinPos.Domain.Entity;
namespace EBS.WinPos
{
    public partial class frmPay : Form
    {
        public frmPay()
        {
            InitializeComponent();
        }

        public ShopCart Cat { get; set; }

        public SaleOrder CurrentOrder { get; set; }
        public frmPos PosForm { get; set; }

        private void frmPay_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
           
            if (Cat.PayAmount < this.Cat.OrderAmount)
            {
                MessageBox.Show("请录入支付金额或使用在线支付","系统消息",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            //修改订单状态
            var selectedPaymentWay = (int)lstPaymentWay.SelectedValue; 


            this.Close();
            this.PosForm.ClearAll(); 
        }

        private void frmPay_Load(object sender, EventArgs e)
        {
            if (Cat == null) { MessageBox.Show("商品项为空!"); return; }

            this.lblOrderAmount.Text = Cat.OrderAmount.ToString();
            this.lblPayAmount.Text = Cat.PayAmount.ToString();
            this.lblChargeAmount.Text = Cat.ChargeAmount.ToString();

            // 显示支付方式

            var paymentWays= typeof(PaymentWay).GetValueToDescription();
            this.lstPaymentWay.DataSource = new BindingSource(paymentWays, null); ;
            this.lstPaymentWay.DisplayMember = "Value";
            this.lstPaymentWay.ValueMember = "Key";
            this.lstPaymentWay.SelectedIndex = 0;

            //隐藏支付条码
            //this.lblPayBarCode.Hide();
            //this.txtPayBarCode.Hide();
        }

        private void lstPaymentWay_SelectedValueChanged(object sender, EventArgs e)
        {
            if ((int)this.lstPaymentWay.SelectedValue == (int)PaymentWay.Cash)
            {
                this.lblPayBarCode.Hide();
                this.txtPayBarCode.Hide();
            }
            else {
                this.lblPayBarCode.Show();
                this.txtPayBarCode.Show();
                this.txtPayBarCode.Focus();
            }
        }

        private void txtPayBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            { 
                //发起支付
            }
        }
    }
}
