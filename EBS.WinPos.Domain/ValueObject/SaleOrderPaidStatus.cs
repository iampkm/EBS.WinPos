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
        [Description("支付宝")]
        AliPay = 2,
        [Description("微信")]
        WechatPay = 3
    }
}
