using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace EBS.WinPos.Domain.ValueObject
{
   public enum SaleOrderPaidStatus
    {
        [Description("作废")]
        Cancel = -1,
        [Description("初始")]
        Create = 1,
        [Description("待支付")]
        WaitPaid = 2,
        [Description("已支付")]
        Paid = 3
    }

    public enum PaymentWay
    {
        [Description("现金")]
        Cash = 1,
        /// <summary>
        /// 支付宝条码支付： 我扫客户支付条码
        /// </summary>
        [Description("支付宝")]
        AliPay = 2,
        /// <summary>
        /// 微信条码支付： 我扫客户支付条码
        /// </summary>
        [Description("微信支付")]
        WechatPay = 3,
        /// <summary>
        /// 支付宝扫码支付： 客户扫我的支付码
        /// </summary>
        [Description("支付宝扫码")]
        AliPayScan = 4,
        /// <summary>
        /// 微信扫码支付： 客户扫我的支付码
        /// </summary>
        [Description("微信扫码")]
        WechatScan = 5
    }
}
