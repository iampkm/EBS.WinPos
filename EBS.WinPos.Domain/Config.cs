using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using EBS.Infrastructure.Extension;
namespace EBS.WinPos.Domain
{
   public class Config
    {
        /// <summary>
        ///sqlite 数据库连接字符串
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                string configKey = "SqliteTest";
                return ConfigurationManager.ConnectionStrings[configKey].ConnectionString;

            }
        }
        /// <summary>
        /// EBS 后台服务地址
        /// </summary>
        public static string ApiService
        {
            get
            {
                string configKey = "ApiService";
                return ConfigurationManager.AppSettings[configKey].ToString();

            }
        }
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
        /// 微信条码支付 地址
        /// </summary>
        public static string Api_Pay_WeChatBarcode
        {
            get
            {
                return PayServer + "/wechatpay/barcodepay";
            }
        }
        /// <summary>
        /// 微信密匙
        /// </summary>
        public static string SignKey_WeChatBarcode
        {
            get
            {
                string configKey = "SignKey_WeChatBarcode";
                return ConfigurationManager.AppSettings[configKey].ToString().Trim();               
            }
        }

        /// <summary>
        /// 支付宝条码支付 地址
        /// </summary>
        public static string Api_Pay_AliBarcode
        {
            get
            {
                return PayServer + "/Alipay/barcodepay";
            }
        }
        /// <summary>
        /// 支付宝密匙
        /// </summary>
        public static string SignKey_AliBarcode
        {
            get
            {
                string configKey = "SignKey_AliBarcode";
                return ConfigurationManager.AppSettings[configKey].ToString().Trim();
            }
        }

       /// <summary>
       /// 允许使用设置的角色
       /// </summary>
        public static int[] Allowsetting
        {
            get {
                string configKey = "Allowsetting";
                return ConfigurationManager.AppSettings[configKey].ToString().Trim().Split(',').ToIntArray();
            }
        }
       
    }
}
