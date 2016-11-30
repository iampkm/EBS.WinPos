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
        public ShopCart CurrentOrder { get; set; }
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
            if (CurrentOrder == null) { MessageBox.Show("订单创建失败返回请重试！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            this.lblOrderAmount.Text = CurrentOrder.OrderAmount.ToString("C");
            this.txtPayAmount.Text = CurrentOrder.PayAmount.ToString();
            this.txtOnlinePayAmount.Enabled = false;
            this.txtPayBarCode.Enabled = false;

            // 显示支付方式
            var paymentWays = typeof(PaymentWay).GetValueToDescription();
            this.lstPaymentWay.DataSource = new BindingSource(paymentWays, null); ;
            this.lstPaymentWay.DisplayMember = "Value";
            this.lstPaymentWay.ValueMember = "Key";
            this.lstPaymentWay.SelectedIndex = 0;


        }

        private void lstPaymentWay_SelectedValueChanged(object sender, EventArgs e)
        {
            var select = (KeyValuePair<int, string>)this.lstPaymentWay.SelectedItem;
            if (select.Key == (int)PaymentWay.Cash)
            {
                this.txtPayBarCode.Enabled = false;
                this.txtPayAmount.Text = CurrentOrder.OrderAmount.ToString();
                this.txtPayBarCode.Text = "";
                this.txtOnlinePayAmount.Text = "0";
               
            }
            else
            {
                this.txtPayBarCode.Enabled = true;
                this.txtPayAmount.Text = "0";
                this.txtOnlinePayAmount.Text = CurrentOrder.OrderAmount.ToString();

            }
        }

      
        private void txtPayAmount_TextChanged(object sender, EventArgs e)
        {
            decimal amount = this.CurrentOrder.PayAmount;
            decimal.TryParse(txtPayAmount.Text, out amount);
            this.CurrentOrder.PayAmount = amount;
            this.txtOnlinePayAmount.Text = (this.CurrentOrder.OrderAmount - this.CurrentOrder.PayAmount).ToString();
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
                var selectedPaymentWay = (PaymentWay)(int)lstPaymentWay.SelectedValue;
                if (selectedPaymentWay == PaymentWay.Cash)
                {
                    CashPay();  // 现金开始支付
                }
                else {
                    this.txtPayBarCode.Focus();  //条码支付，转入条码输入
                }               
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
                case PaymentWay.AliPay:
                  AliPay(this.txtPayBarCode.Text);
                    break;
                case PaymentWay.WechatPay:
                    WechatPay(this.txtPayBarCode.Text);
                    break;
                default:
                    MessageBox.Show("请选择支付方式", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                var payAmount = CurrentOrder.PayAmount;
                decimal.TryParse(txtPayAmount.Text, out payAmount);
                CurrentOrder.PayAmount = payAmount;
                _orderService.CashPay(CurrentOrder.OrderId, CurrentOrder.PayAmount);
                PosForm.ShowPreOrderInfo();
                MessageBox.Show("支付成功！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClosePayForm();
                // 打印小票
                _orderService.PrintTicket(CurrentOrder.OrderId);
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
                var payAmount = CurrentOrder.PayAmount;
                decimal.TryParse(txtPayAmount.Text, out payAmount);
                CurrentOrder.PayAmount = payAmount;
                var onlinePayAmount = 0m;
                decimal.TryParse(txtOnlinePayAmount.Text, out onlinePayAmount);
                CurrentOrder.OnlinePayAmount = onlinePayAmount;
                _orderService.AliPay(CurrentOrder.OrderId, payBarCode,CurrentOrder.PayAmount);
                PosForm.ShowPreOrderInfo();
                MessageBox.Show("支付成功！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClosePayForm();
                // 打印小票
                _orderService.PrintTicket(CurrentOrder.OrderId);
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
                var payAmount = CurrentOrder.PayAmount;
                decimal.TryParse(txtPayAmount.Text, out payAmount);
                CurrentOrder.PayAmount = payAmount;
                var onlinePayAmount = 0m;
                decimal.TryParse(txtOnlinePayAmount.Text, out onlinePayAmount);
                CurrentOrder.OnlinePayAmount = onlinePayAmount;
                _orderService.WechatPay(CurrentOrder.OrderId, payBarCode,CurrentOrder.PayAmount);
                PosForm.ShowPreOrderInfo();
                MessageBox.Show("支付成功！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClosePayForm();
                // 打印小票
                _orderService.PrintTicket(CurrentOrder.OrderId);
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
                this.txtPayAmount.Focus();
            }
        }

        
    }
}
