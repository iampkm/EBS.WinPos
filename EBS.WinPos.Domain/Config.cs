using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
namespace EBS.WinPos.Domain
{
   public class Config
    {
        /// <summary>
        /// 支付服务器
        /// </summary>
        public static string PayServer
        {
            get
            {
                string configKey = "PayServer";
                return ConfigurationManager.AppSettings[configKey].ToString();
               
            }
        }
        /// <summary>
        /// 微信条码支付
        /// </summary>
        public static string Api_Pay_WeChatBarcode
        {
            get
            {
                return PayServer + "/wechatpay/barcodepay";
            }
        }
        public static string SignKey_WeChatBarcode
        {
            get
            {
                string configKey = "SignKey_WeChatBarcode";
                return ConfigurationManager.AppSettings[configKey].ToString().Trim();               
            }
        }
    }
}
