using EBS.WinPos.Domain.ValueObject;
using EBS.WinPos.Service;
using EBS.WinPos.Service.Dto;
using EBS.Infrastructure.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EBS.Infrastructure;
namespace EBS.WinPos
{
    public partial class frmRefund : Form
    {
        public ShopCart CurrentOrder { get; set; }
        public frmPos PosForm { get; set; }

        public SaleOrderService _orderService;
        private static frmRefund _instance;
        public static frmRefund CreateForm()
        {
            //判断是否存在该窗体,或时候该字窗体是否被释放过,如果不存在该窗体,则 new 一个字窗体  
            if (_instance == null || _instance.IsDisposed)
            {
                _instance = new frmRefund();
            }
            return _instance;
        }
        public frmRefund()
        {
            InitializeComponent();
            _orderService = new SaleOrderService();
        }

        private void frmRefund_KeyDown(object sender, KeyEventArgs e)
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

        private void frmRefund_Load(object sender, EventArgs e)
        {
            if (CurrentOrder == null) { MessageBox.Show("订单创建失败返回请重试！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information); this.Close(); }

            this.lblOrderAmount.Text =Math.Abs(CurrentOrder.OrderAmount).ToString("C");
            this.txtPayAmount.Text =Math.Abs(CurrentOrder.PayAmount).ToString();
            this.txtOnlinePayAmount.Enabled = false;
            this.txtOnlinePayAmount.Text = "0";
            this.txtRefundAccount.Enabled = false;
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
                this.txtRefundAccount.Enabled = false;
                this.txtOnlinePayAmount.Text = "0";
                this.txtPayAmount.Text =Math.Abs(CurrentOrder.OrderAmount).ToString();
            }
            else
            {
                this.txtRefundAccount.Enabled = true;
                this.txtOnlinePayAmount.Text =Math.Abs(this.CurrentOrder.OrderAmount).ToString();               
                this.txtPayAmount.Text = "0";
            }
        }


        private void txtPayAmount_TextChanged(object sender, EventArgs e)
        {
            //decimal amount = this.CurrentOrder.PayAmount;
            //decimal.TryParse(txtPayAmount.Text, out amount);
            //this.CurrentOrder.PayAmount = amount;
            //this.txtOnlinePayAmount.Text = (Math.Abs(this.CurrentOrder.OrderAmount) - this.CurrentOrder.PayAmount).ToString();           
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
                decimal amount = this.CurrentOrder.PayAmount;
                decimal.TryParse(txtPayAmount.Text, out amount);
                this.CurrentOrder.PayAmount = amount;

                var selectedPaymentWay = (PaymentWay)(int)lstPaymentWay.SelectedValue;
                if (selectedPaymentWay == PaymentWay.Cash)
                {
                    this.txtLicenseCode.Focus();  // 现金输入完，就输入授权码
                }
                else
                {
                    this.txtOnlinePayAmount.Text = (Math.Abs(this.CurrentOrder.OrderAmount) - this.CurrentOrder.PayAmount).ToString();   
                    this.txtRefundAccount.Focus();  // 在线支付，输入退款账户
                }
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
                    AliPay();
                    break;
                case PaymentWay.WechatPay:
                    WechatPay();
                    break;
                default:
                    MessageBox.Show("请选择支付方式", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

        public void ClosePayForm()
        {
            this.Close();
        }

        public void CashPay()
        {
            try
            {
                var payAmount = CurrentOrder.PayAmount;
                decimal.TryParse(txtPayAmount.Text, out payAmount);
                CurrentOrder.PayAmount = -payAmount;
                var lincenseCode = txtLicenseCode.Text;
                _orderService.CashRefund(CurrentOrder.OrderId, lincenseCode, CurrentOrder.PayAmount);
                PosForm.ClearItems();
                MessageBox.Show("退款成功！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClosePayForm();
            }
            catch (AppException aex)
            {
                MessageBox.Show(aex.Message, "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
          

        }

        public void AliPay()
        {
            try
            {
                var payAmount = 0m;
                decimal.TryParse(txtPayAmount.Text, out payAmount);
                CurrentOrder.PayAmount = -payAmount;
                var onlinePayAmount = 0m;
                decimal.TryParse(txtOnlinePayAmount.Text, out onlinePayAmount);
                CurrentOrder.OnlinePayAmount = -onlinePayAmount;
                var lincenseCode = txtLicenseCode.Text;
                CurrentOrder.RefundAccount = txtRefundAccount.Text;
                _orderService.AliRefund(CurrentOrder.OrderId, lincenseCode, CurrentOrder.PayAmount, CurrentOrder.RefundAccount);
                PosForm.ClearItems();
                MessageBox.Show("退款申请提交成功！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClosePayForm();
            }
            catch (AppException aex)
            {
                MessageBox.Show(aex.Message, "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                AppContext.Log.Error(ex);
                MessageBox.Show(ex.Message, "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
          
        }

        public void WechatPay()
        {           
            try
            {
                var payAmount = 0m;
                decimal.TryParse(txtPayAmount.Text, out payAmount);
                CurrentOrder.PayAmount = -payAmount;
                var onlinePayAmount = 0m;
                decimal.TryParse(txtOnlinePayAmount.Text, out onlinePayAmount);
                CurrentOrder.OnlinePayAmount = -onlinePayAmount;
                var lincenseCode = txtLicenseCode.Text;
                CurrentOrder.RefundAccount = txtRefundAccount.Text;
                _orderService.WechatRefund(CurrentOrder.OrderId, lincenseCode, CurrentOrder.PayAmount, CurrentOrder.RefundAccount);
                PosForm.ClearItems();
                MessageBox.Show("退款申请提交成功！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClosePayForm();
            }
            catch (AppException aex)
            {
                MessageBox.Show(aex.Message, "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void txtOnlinePayAmount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.txtRefundAccount.Focus();
            }
        }

        private void txtRefundAccount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.txtLicenseCode.Focus();
            }
        }

        private void txtLicenseCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BeginPay();
            }
        }
    }
}
