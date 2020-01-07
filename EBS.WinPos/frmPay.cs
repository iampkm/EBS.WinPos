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
            this.txtCashOrAuthCode.Focus();            
            this.lblInfo.Text = "现金支付，请输入收款金额，然后按Enter键。微信或支付宝，请扫顾客手机上的付款条码，然后按Enter键。如输入有问题，请手动输入条码数字串。";
            this.lblOrderAmount.Text = CurrentOrder.OrderAmount.ToString("F2");
            this.lblPaymentWay.Text = "";
        }
        

        private void txtPayAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar) || e.KeyChar == '\b' || (e.KeyChar == '.' && txtCashOrAuthCode.Text.IndexOf(".") < 0)))
            {
                e.Handled = true;
            }

        }

        private bool IsAlipayAuthCode(string authCode)
        {
            //18位纯数字，以10、11、12、13、14、15开头）  微信支付
            var isPayBarCode = authCode.StartsWith("10") || authCode.StartsWith("11") || authCode.StartsWith("12") || authCode.StartsWith("13") || authCode.StartsWith("14") || authCode.StartsWith("15");
            return isPayBarCode && authCode.Length >= 18;
        }

        private bool IsWechatAuthCode(string authCode)
        {
            //支付宝： 支付授权码，25~30开头的长度为16~24位的数字，实际字符串长度以开发者获取的付款码长度为准
            var isPayBarCode = authCode.StartsWith("25") || authCode.StartsWith("26") || authCode.StartsWith("27") || authCode.StartsWith("28") || authCode.StartsWith("29") || authCode.StartsWith("30");
            return isPayBarCode && authCode.Length >= 16;
        }

        private void TxtCashOrAuthCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                BeginPay();
            }          
        }

        public void BeginPay()
        {
            var inputCashOrAuthCode = this.txtCashOrAuthCode.Text;
            if (string.IsNullOrEmpty(inputCashOrAuthCode)) return;
            // 判断支付方式
            this.CurrentOrder.PaymentWay = ConfirmPaymentWay(inputCashOrAuthCode);

            // 现金支付方式
            if (this.CurrentOrder.PaymentWay == PaymentWay.Cash)
            {
                CashPay(inputCashOrAuthCode);  // 现金开始支付
            }
            else if (this.CurrentOrder.PaymentWay == PaymentWay.WechatPay)
            {
                WechatPay(inputCashOrAuthCode);
            }
            else if (this.CurrentOrder.PaymentWay == PaymentWay.WechatScan)
            {
                ScanPay(PaymentWay.WechatScan);
            }
            else if (this.CurrentOrder.PaymentWay == PaymentWay.AliPay)
            {
                AliPay(inputCashOrAuthCode);
            }
            else if (this.CurrentOrder.PaymentWay == PaymentWay.AliPayScan)
            {
                ScanPay(PaymentWay.AliPayScan);
            }
            else {
                MessageBox.Show("支付授权码不正确，请重新输入", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

        }

        private void CheckAuthCode(string inputAuthCode)
        {

            if (inputAuthCode.Length < 16)
            {
                MessageBox.Show("支付授权码长度不正确", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        /// <summary>
        ///  确认支付方式
        /// </summary>
        /// <param name="inputCashOrAuthCode"></param>
        /// <returns></returns>
        private PaymentWay ConfirmPaymentWay(string inputCashOrAuthCode)
        {
            if (IsWechatAuthCode(inputCashOrAuthCode))
            {
                return PaymentWay.WechatPay;
            }
            else if (IsAlipayAuthCode(inputCashOrAuthCode))
            {
                return PaymentWay.AliPay;
            }
            else if (inputCashOrAuthCode.ToUpper().StartsWith("A"))
            {
                //支付宝扫码
                return PaymentWay.AliPayScan;
            }
            else if (inputCashOrAuthCode.ToUpper().StartsWith("W"))
            {
                // 微信扫码
                return PaymentWay.WechatScan;
            }
            else
            {
                return PaymentWay.Cash;
            }
        }

        public void ClosePayForm()
        {
            this.Close();
        }

        public void CashPay(string inputMoney)
        {
            try
            {
                decimal payAmount = 0m;
                if (!decimal.TryParse(inputMoney, out payAmount)) { throw new AppException("输入金额有误，请重新输入!"); }
                if (payAmount > this.CurrentOrder.OrderAmount + 100)
                {
                    MessageBox.Show("实收款录入的金额过大", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.txtCashOrAuthCode.Text = "0.00";
                    this.txtCashOrAuthCode.Focus();
                    this.txtCashOrAuthCode.SelectAll();
                    return;
                }
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
            try
            {
                this.CheckAuthCode(payBarCode);
                _orderService.AliPay(CurrentOrder.OrderId, payBarCode);
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
            try
            {
                this.CheckAuthCode(payBarCode);
                _orderService.WechatPay(CurrentOrder.OrderId, payBarCode);
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

        /// <summary>
        /// 客户扫我的支付码支付。
        /// </summary>
        /// <param name="paymentWay"></param>
        public void ScanPay(PaymentWay paymentWay)
        {
            try
            {
                CurrentOrder.PayAmount = 0;
                CurrentOrder.OnlinePayAmount = this.CurrentOrder.OrderAmount;
                _orderService.ScanPay(CurrentOrder.OrderId, paymentWay);
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

        private void TxtCashOrAuthCode_TextChanged(object sender, EventArgs e)
        {
            // 根据输入，确定是现金支付，还是支付宝，微信
            var txtCashOrAuthCode = (TextBox)sender;
            if (string.IsNullOrEmpty(txtCashOrAuthCode.Text)) { return; }
            this.CurrentOrder.PaymentWay = ConfirmPaymentWay(txtCashOrAuthCode.Text);
            this.lblPaymentWay.Text = this.CurrentOrder.PaymentWay.Description();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Enter(object sender, EventArgs e)
        {
            this.btnSave.BackColor = Color.ForestGreen;
        }

        private void btnSave_Leave(object sender, EventArgs e)
        {
            this.btnSave.BackColor = Color.LimeGreen ;
        }
    }
}
