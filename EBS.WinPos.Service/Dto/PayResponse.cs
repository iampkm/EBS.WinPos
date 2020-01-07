using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBS.WinPos.Service.Dto
{
    /// <summary>
    /// 支付响应结果
    /// </summary>
   public class PayResponse
    {
        public PayResponse()
        {
            this.data = new Dictionary<string, string>();
        }
         public Dictionary<string, string> data { get; set; }

         public bool success { get; set; }

         public string message { get; set; }


        /// <summary>
        /// 微信交易成功
        /// </summary>
        /// <returns></returns>
        public bool WechatTransactionSuccess()
        {
            return success && data.ContainsKey("ResultCode") && data["ResultCode"] == "SUCCESS";
        }

        /// <summary>
        /// 交易状态trade_state=SUCCESS
        /// </summary>
        /// <returns></returns>
        public bool WechatTradeStatusSuccess()
        {
           return data.ContainsKey("TradeState") && data["TradeState"] == "SUCCESS";
        }

        public bool AlipayTransactionSuccess()
        {
            return success && data.ContainsKey("Code") && data["Code"] == "10000";
        }

        public bool AlipayTradeStatusSuccess()
        {
            return data.ContainsKey("TradeState") && data["TradeState"] == "TRADE_SUCCESS";
        }

        public DateTime GetPayDate()
        { 
            return DateTime.ParseExact(data["TimeEnd"], "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
        }
    }
}
