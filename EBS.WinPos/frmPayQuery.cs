using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EBS.WinPos.Service;
using EBS.WinPos.Domain.Entity;
using EBS.Infrastructure;
using EBS.WinPos.Service.Dto;
using EBS.Infrastructure.Extension;
namespace EBS.WinPos
{
    public partial class frmPayQuery : Form
    {

        private static frmPayQuery _instance;
        public static frmPayQuery CreateForm()
        {
            //判断是否存在该窗体,或时候该字窗体是否被释放过,如果不存在该窗体,则 new 一个字窗体  
            if (_instance == null || _instance.IsDisposed)
            {
                _instance = new frmPayQuery();
            }
            return _instance;
        }

        public SaleOrderService _orderService;

        public frmPayQuery()
        {
            InitializeComponent();

            _orderService = new SaleOrderService();
        }

        private void FrmPayQuery_Load(object sender, EventArgs e)
        {
            this.lstOrderInfo.Items.Clear();
            this.lstPayInfo.Items.Clear();
        }

        private void BtnQuery_Click(object sender, EventArgs e)
        {
            Query(this.txtOrderCode.Text);
        }

        public void Query(string code)
        {
            if (string.IsNullOrEmpty(code)) { return; }

            var order = _orderService.QuerySaleOrder(code);
            if (order == null)
            {
                // 增加联网查询
                throw new AppException(string.Format("小票号{0},单据不存在。", code));
            }

            ShowOrderInfo(order);

            var response = new PayResponse();
            if (order.OrderType == 1 && order.PaymentWay == Domain.ValueObject.PaymentWay.WechatPay)
            {
                response = _orderService.WechatTradeQuery(code, order.StoreId);
                ShowWechatPayInfo(response);
                _orderService.CorrectWechatOrderStatus(order, response);
            }
            else if (order.OrderType == 1 && order.PaymentWay == Domain.ValueObject.PaymentWay.AliPay)
            {
                response = _orderService.AliPayTradeQuery(code, order.StoreId);
                ShowWechatPayInfo(response);
                _orderService.CorrentAlipayOrderStatus(order, response);
            }
            else if (order.OrderType == 2 && order.PaymentWay == Domain.ValueObject.PaymentWay.WechatPay)
            {
                response = _orderService.WechatRefundQuery(code, order.StoreId);
                ShowWechatRefundInfo(response);
                _orderService.CorrectWechatRefundStatus(order, response);

            }
            else if (order.OrderType == 2 && order.PaymentWay == Domain.ValueObject.PaymentWay.AliPay)
            {
               response=  _orderService.AliPayRefundQuery(code, order.StoreId);
                ShowAlipayRefundInfo(response);
                _orderService.CorrentAlipayRefundStatus(order, response);
            }
            else
            {
                
            }

        }


        private void ShowOrderInfo(SaleOrder order)
        {
            var queryList = new List<string>();
            queryList.Add("订单号：" + order.Code);
            queryList.Add("创建日期：" + order.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss"));
            queryList.Add("金额：" + order.OrderAmount + " 元");
            queryList.Add("支付方式：" + order.PaymentWay.Description());
            queryList.Add("支付状态：" + order.Status.Description());
            this.lstOrderInfo.Items.Clear();
            this.lstOrderInfo.Items.AddRange(queryList.ToArray());

        }

        private void ShowWechatPayInfo(PayResponse response)
        {
            var result = response;
            var queryList = new List<string>();
            if (result.WechatTransactionSuccess())
            {
                queryList.Add("微信单号:" + result.data["TradeNo"]);
                queryList.Add("商户单号:" + result.data["OutTradeNo"]);
                queryList.Add("交易状态:" + result.data["TradeState"]);
                queryList.Add("交易日期:" + result.data["TimeEnd"]);
                queryList.Add("交易状态描述:" + result.data["TradeStateDesc"]);
                var orderAmount = (Convert.ToDecimal(result.data["TotalAmount"]) / 100).ToString("F");
                queryList.Add("订单金额:" + orderAmount);
            }
            else
            {
                queryList.Add("查询状态:交易查询失败");
            }
            this.lstPayInfo.Items.Clear();
            this.lstPayInfo.Items.AddRange(queryList.ToArray());


        }

        private void ShowWechatRefundInfo(PayResponse response)
        {
            var result = response;
            var queryList = new List<string>();

            if (result.WechatTransactionSuccess())
            {
                // var tradeStateString = result.data["TradeState"] == "SUCCESS" ? " 支付成功!" : result.data["TradeState"];
                queryList.Add("微信订单号:" + result.data["TradeNo"]);
                queryList.Add("商户单号:" + result.data["OutTradeNo"]);
                //queryList.Add("交易状态:" + result.data["RefundStatus"]);
                queryList.Add("退款次数:" + result.data["TotalRefundCount"]);
                var orderAmount = (Convert.ToDecimal(result.data["TotalAmount"]) / 100).ToString("F");
                queryList.Add("订单金额:" + orderAmount);
            }
            else
            {
                queryList.Add("查询状态:交易查询失败");
            }
            this.lstPayInfo.Items.Clear();
            this.lstPayInfo.Items.AddRange(queryList.ToArray());
        }

        private void ShowAliPayInfo(PayResponse response)
        {
            var result = response;
            var queryList = new List<string>();
            if (result.WechatTransactionSuccess())
            {
                //交易状态：WAIT_BUYER_PAY（交易创建，等待买家付款）、TRADE_CLOSED（未付款交易超时关闭，或支付完成后全额退款）、TRADE_SUCCESS（交易支付成功）、TRADE_FINISHED（交易结束，不可退款）
                // var tradeStateString = result.data["TradeState"] == "TRADE_SUCCESS" ? " 交易支付成功!" : result.data["TradeState"];
                queryList.Add("支付宝单号:" + result.data["TradeNo"]);
                queryList.Add("商户单号:" + result.data["OutTradeNo"]);
                queryList.Add("交易状态:" + result.data["TradeStatus"]);
                queryList.Add("交易日期:" + result.data["SendPayDate"]);
                var orderAmount = result.data["TotalAmount"];
                queryList.Add("订单金额:" + orderAmount);
            }
            else
            {
                queryList.Add("查询状态:交易查询失败");
            }
            this.lstPayInfo.Items.Clear();
            this.lstPayInfo.Items.AddRange(queryList.ToArray());
        }

        private void ShowAlipayRefundInfo(PayResponse response)
        {
            var result = response;
            var queryList = new List<string>();
            if (result.WechatTransactionSuccess())
            {
                //交易状态：WAIT_BUYER_PAY（交易创建，等待买家付款）、TRADE_CLOSED（未付款交易超时关闭，或支付完成后全额退款）、TRADE_SUCCESS（交易支付成功）、TRADE_FINISHED（交易结束，不可退款）
                // var tradeStateString = result.data["TradeState"] == "TRADE_SUCCESS" ? " 交易支付成功!" : result.data["TradeState"];
                queryList.Add("支付宝订单号:" + result.data["TradeNo"]);
                queryList.Add("交易状态:" + result.data["TradeState"]);
                //queryList.Add("交易状态描述:" + result.data["TradeStateDesc"]);
                var orderAmount = result.data["TotalAmount"];
                queryList.Add("订单金额:" + orderAmount);
            }
            else
            {
                queryList.Add("查询状态:交易查询失败");
            }
            this.lstPayInfo.Items.Clear();
            this.lstPayInfo.Items.AddRange(queryList.ToArray());
        }




    }
}
