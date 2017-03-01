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
using EBS.Infrastructure;
using EBS.Infrastructure.Log;
namespace EBS.WinPos
{
    public partial class frmPay : Form
    {
        public ShopCart CurrentOrder { get; set; }
        public frmPos PosForm { get; set; }

        public SaleOrderService _orderService;

        ILogger _log;

        private static frmPay _instance;
        public static frmPay CreateForm()
        {
            //判断是否存在该窗体,或时候该字窗体是否被释放过,如果不存在该窗体,则 new 一个字窗体  
            if (_instance == null || _instance.IsDisposed)
            {
                _instance = new frmPay();
            }
            return _instance;
        }
        
        public frmPay()
        {
            InitializeComponent();

            _orderService = new SaleOrderService();
            _log = AppContext.Log;
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

            // 显示支付方式
            var paymentWays = typeof(PaymentWay).GetValueToDescription();
            this.lstPaymentWay.DataSource = new BindingSource(paymentWays, null);
            this.lstPaymentWay.DisplayMember = "Value";
            this.lstPaymentWay.ValueMember = "Key";
            this.lstPaymentWay.SelectedIndex = 0;

            this.lblOrderAmount.Text = CurrentOrder.OrderAmount.ToString("F2");
            this.txtPayAmount.Text = CurrentOrder.PayAmount.ToString("F2");
            this.txtOnlinePayAmount.Enabled = false;
            this.txtPayBarCode.Enabled = false;

        }

        private void lstPaymentWay_SelectedValueChanged(object sender, EventArgs e)
        {
            var select = (KeyValuePair<int, string>)this.lstPaymentWay.SelectedItem;
            if (select.Key == (int)PaymentWay.Cash)
            {
                this.txtPayBarCode.Enabled = false;
                this.txtPayAmount.Text = "0.00";
                this.txtPayBarCode.Text = "";
                this.txtOnlinePayAmount.Text = "0.00";
            }
            else
            {
                this.txtPayBarCode.Enabled = true;
                this.txtPayAmount.Text = "0.00";
                this.txtOnlinePayAmount.Text = CurrentOrder.OrderAmount.ToString();

            }
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
                decimal amount = 0m;
                decimal.TryParse(txtPayAmount.Text, out amount);
                var select = (KeyValuePair<int, string>)this.lstPaymentWay.SelectedItem;
                if (select.Key == (int)PaymentWay.Cash)
                {
                    //校验录入金额是否超过订单金额+100
                    if (amount > this.CurrentOrder.OrderAmount + 100)
                    {
                        MessageBox.Show("实收款录入的金额过大", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.txtPayAmount.Text = "0.00";                       
                        this.txtPayAmount.Focus();
                        this.txtPayAmount.SelectAll();
                        return;
                    }
                    CashPay();  // 现金开始支付
                }
                else
                {
                    if (amount > this.CurrentOrder.OrderAmount)
                    {
                        MessageBox.Show("现金支付部分不能超过订单金额", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.txtPayAmount.Text = "0.00";                       
                        this.txtPayAmount.Focus();
                        this.txtPayAmount.SelectAll();
                        return;
                    }
                    this.txtOnlinePayAmount.Text = (this.CurrentOrder.OrderAmount - amount).ToString();
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
        }

        public void CashPay()
        {
            try
            {
                var payAmount = CurrentOrder.PayAmount;
                decimal.TryParse(txtPayAmount.Text, out payAmount);
                CurrentOrder.PayAmount = payAmount;
                CurrentOrder.OnlinePayAmount = 0m;
                _orderService.CashPay(CurrentOrder.OrderId, CurrentOrder.PayAmount);
                PosForm.ClearItems();
                ClosePayForm();                         
                MessageBox.Show("支付成功！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (AppException aex)
            {
                MessageBox.Show(aex.Message, "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
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
                _orderService.AliPay(CurrentOrder.OrderId, payBarCode, CurrentOrder.PayAmount);
                PosForm.ClearItems();
                ClosePayForm();               
                MessageBox.Show("支付成功！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (AppException aex)
            {
                MessageBox.Show(aex.Message, "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
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
                _orderService.WechatPay(CurrentOrder.OrderId, payBarCode, CurrentOrder.PayAmount);
                PosForm.ClearItems();
                ClosePayForm();               
                MessageBox.Show("支付成功！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (AppException aex)
            {
                MessageBox.Show(aex.Message, "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
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
