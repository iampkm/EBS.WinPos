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
using EBS.WinPos.Service;
namespace EBS.WinPos
{
    public partial class frmPay : Form
    {
        public OrderInfo CurrentOrder { get; set; }
        public frmPos PosForm { get; set; }

        public SaleOrderService _orderService;
        public frmPay()
        {
            InitializeComponent();

            _orderService = new SaleOrderService();
        }
       

        private void frmPay_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            BeginPay();           
        }

        private void frmPay_Load(object sender, EventArgs e)
        {
            if (CurrentOrder == null) { MessageBox.Show("订单创建失败返回请重试!"); return; }

            this.lblOrderAmount.Text = CurrentOrder.OrderAmount.ToString();
            this.txtPayAmount.Text = CurrentOrder.PayAmount.ToString();
            this.lblChargeAmount.Text = CurrentOrder.ChargeAmount.ToString();

            // 显示支付方式

            var paymentWays = typeof(PaymentWay).GetValueToDescription();
            this.lstPaymentWay.DataSource = new BindingSource(paymentWays, null); ;
            this.lstPaymentWay.DisplayMember = "Value";
            this.lstPaymentWay.ValueMember = "Key";
            this.lstPaymentWay.SelectedIndex = 0;

            //默认现金支付方式
            this.btnSave.Focus();

            //隐藏支付条码
            this.lblPayBarCode.Hide();
            this.txtPayBarCode.Hide();
        }

        private void lstPaymentWay_SelectedValueChanged(object sender, EventArgs e)
        {
            var select = (KeyValuePair<int, string>)this.lstPaymentWay.SelectedItem;
            if (select.Key == (int)PaymentWay.Cash)
            {
                this.lblPayBarCode.Hide();
                this.txtPayBarCode.Hide();
                this.txtPayAmount.Enabled = true;
            }
            else
            {
                this.lblPayBarCode.Show();
                this.txtPayBarCode.Show();
                this.txtPayAmount.Enabled = false;
            }
        }

        private void txtPayBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BeginPay();               
            }
        }

        public void BeginPay()
        {            
            var selectedPaymentWay = (PaymentWay)(int)lstPaymentWay.SelectedValue;
            switch (selectedPaymentWay)
            {
                case PaymentWay.Cash:
                    CashPay();
                    break;
                case PaymentWay.AliPay:
                    AliPay(this.txtPayBarCode.Text);
                    break;
                case PaymentWay.WechatPay:
                    WechatPay(this.txtPayBarCode.Text);
                    break;
                default:
                    MessageBox.Show("选择支付方式");
                    break;
            }           
        }

        public void ClosePayForm()
        {
            this.Close();
            this.PosForm.ClearAll();
        }

        public void CashPay()
        {           
            try
            {
                _orderService.CashPay(CurrentOrder.OrderId, CurrentOrder.PayAmount);
              
                MessageBox.Show("支付成功！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ClosePayForm();
                // 打印作废小票
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
          
        }

        public void AliPay(string payBarCode)
        {
           
            if (string.IsNullOrEmpty(payBarCode)) { return; }
            try
            {
                _orderService.AliPay(CurrentOrder.OrderId, payBarCode);
                MessageBox.Show("支付成功！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ClosePayForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void WechatPay(string payBarCode)
        {
            if (string.IsNullOrEmpty(payBarCode)) { return; }
            try
            {
                _orderService.WechatPay(CurrentOrder.OrderId, payBarCode);
                MessageBox.Show("支付成功！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ClosePayForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void lstPaymentWay_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if ((int)this.lstPaymentWay.SelectedValue == (int)PaymentWay.Cash)
                {
                    this.txtPayAmount.Focus();
                }
                if ((int)this.lstPaymentWay.SelectedValue == (int)PaymentWay.AliPay)
                {
                    this.txtPayBarCode.Focus();
                }
                if ((int)this.lstPaymentWay.SelectedValue == (int)PaymentWay.WechatPay)
                {
                    this.txtPayBarCode.Focus();
                }
            }
        }

        private void txtPayAmount_TextChanged(object sender, EventArgs e)
        {
            decimal amount = this.CurrentOrder.PayAmount;
            decimal.TryParse(txtPayAmount.Text,out amount);
            this.CurrentOrder.PayAmount = amount;
            this.lblChargeAmount.Text = this.CurrentOrder.ChargeAmount.ToString();
        }

        private void txtPayAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar) || e.KeyChar == '\b' || (e.KeyChar == '.' && txtPayAmount.Text.IndexOf(".") < 0)))
            {
                e.Handled = true;
            }

        }

        private void txtPayAmount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BeginPay();
            }
        }
    }
}
