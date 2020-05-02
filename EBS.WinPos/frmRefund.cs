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

            this.lblOrderAmount.Text = Math.Abs(CurrentOrder.OrderAmount).ToString("F2");            
            this.txtLicenseCode.Text = "";
            this.txtRefundCode.Text = "";
            this.txtRefundCode.Focus();
        }


        //private void TxtRefundCode_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (!(Char.IsNumber(e.KeyChar) || e.KeyChar == '\b' || (e.KeyChar == '.' && txtRefundCode.Text.IndexOf(".") < 0)))
        //    {
        //        e.Handled = true;
        //    }
        //}

        private void TxtRefundCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    this.txtLicenseCode.Focus();
                    ConfirmPaymentWay(txtRefundCode.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            else if (e.KeyCode == Keys.F1)
            {
                this.txtLicenseCode.Focus();
                this.CurrentOrder.PaymentWay = PaymentWay.WechatScan;
                this.lblPaymentWay.Text = "微信扫码";

            }
            else if (e.KeyCode == Keys.F2) {
                //支付宝退款
                this.txtLicenseCode.Focus();
                this.CurrentOrder.PaymentWay = PaymentWay.AliPayScan;
                this.lblPaymentWay.Text = "支付宝扫码";
            }

        }

        /// <summary>
        ///  查询退款订单
        /// </summary>
        /// <param name="code">原销售单单号</param>
        private void QueryRefundOrder(string code)
        {
            
            var sourceSaleOrder = _orderService.QuerySaleOrder(code);
            if (sourceSaleOrder == null)
            { // 增加联网查询
                throw new AppException(string.Format("小票号{0},单据不存在。", code));
            }
            if (sourceSaleOrder.Status != SaleOrderStatus.Paid) { throw new AppException(string.Format("原销售单{0}未支付,不能退款。", code)); }

            //已退明细
            var refundedOrderItems = _orderService.QueryRefundOrderItems(code);
            //判断商品是否属于原单
            foreach (var item in this.CurrentOrder.Items)
            {
                var line = sourceSaleOrder.Items.FirstOrDefault(n => n.ProductId == item.ProductId);
                if (line == null)
                {
                    throw new AppException(string.Format("商品{0}原单中不存在,不能退款。", line.ProductCode));
                }
                // 已退数量
                var quantityReturned = Math.Abs(refundedOrderItems.Where(n => n.ProductId == item.ProductId).Sum(n => n.Quantity));
                if ((item.Quantity + quantityReturned) > Math.Abs(line.Quantity))
                {
                    throw new AppException(string.Format("商品{0}可退{1}，已退{2}，本次退货{3}，超过最大可退数量。",
                        line.ProductCode, item.Quantity, quantityReturned, Math.Abs(line.Quantity)));
                }

                // 修改购物车中商品价格按原单价格计算
                item.SalePrice = line.SalePrice;
                item.RealPrice = line.RealPrice;
            }

            // 显示可退金额
            this.CurrentOrder.PaymentWay = sourceSaleOrder.PaymentWay;
            this.lblPaymentWay.Text = this.CurrentOrder.PaymentWay.Description() + "退款";
            this.lblOrderAmount.Text = Math.Abs(this.CurrentOrder.OrderAmount).ToString("F2");
            this.CurrentOrder.SourceSaleOrderCode = sourceSaleOrder.Code;

        }

        private void txtLicenseCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BeginPay();
            }
        }




        public void BeginPay()
        {
            var selectedPaymentWay = this.CurrentOrder.PaymentWay;
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
                case PaymentWay.AliPayScan:
                    ScanRefund();
                    break;
                case PaymentWay.WechatScan:
                    ScanRefund();
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
                CurrentOrder.PayAmount = CurrentOrder.OrderAmount;
                var lincenseCode = txtLicenseCode.Text;
                _orderService.CashRefund(CurrentOrder, lincenseCode);
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
        public void ScanRefund()
        {
            try
            {
                CurrentOrder.PayAmount = CurrentOrder.OrderAmount;
                var lincenseCode = txtLicenseCode.Text;
                _orderService.ScanRefund(CurrentOrder, lincenseCode);
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
                CurrentOrder.OnlinePayAmount = CurrentOrder.OrderAmount;
                var lincenseCode = txtLicenseCode.Text;
                _orderService.AliRefund(CurrentOrder, lincenseCode);
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

                CurrentOrder.OnlinePayAmount = CurrentOrder.OrderAmount;
                var lincenseCode = txtLicenseCode.Text;
                _orderService.WechatRefund(CurrentOrder, lincenseCode);
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

        private void txtRefundCode_TextChanged(object sender, EventArgs e)
        {
            // 根据输入，确定是现金支付，还是支付宝，微信
            var txtCashOrAuthCode = (TextBox)sender;
            this.lblPaymentWay.Text = this.CurrentOrder.PaymentWay.Description() + "退款";
        }

        /// <summary>
        ///  确认支付方式
        /// </summary>
        /// <param name="inputCashOrAuthCode"></param>
        /// <returns></returns>
        private void ConfirmPaymentWay(string inputCashOrAuthCode)
        {
           
            if (string.IsNullOrEmpty(inputCashOrAuthCode))
            {
                // 现金退款
                this.CurrentOrder.PaymentWay = PaymentWay.Cash;
            }            
            else {
                // 原单退款
                QueryRefundOrder(inputCashOrAuthCode);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
