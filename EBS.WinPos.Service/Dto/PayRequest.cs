using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace EBS.WinPos.Service.Dto
{
    /// <summary>
    ///  支付接口公共请求
    /// </summary>
    public class PayRequest
    {

        public PayRequest()
        {
            this.AppId = "ebs-pos";
            this.Version = "1.0";
            this.charset = "UTF-8";
            this.SignType = "MD5";
            this.Format = "json";
            this.Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        ///  分配的可连接支付APP
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        ///  链接支付门店ID
        /// </summary>
        public string StoreId { get; set; }
        /// <summary>
        /// 接口名： wechat.trade.barcode.pay
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        ///  版本 1.0
        /// </summary>
        public string Version { get; set; }

        public string Format { get; set; }
        /// <summary>
        /// 字符集：默认 UTF-8 
        /// </summary>
        public string charset { get; set; }
        /// <summary>
        /// 加密方式：默认 MD5
        /// </summary>
        public string SignType { get; set; }       
                      
        /// <summary>
        ///  发送请求的时间，格式"yyyy-MM-dd HH:mm:ss"
        /// </summary>
        public string Timestamp { get; set; }
       
        /// <summary>
        ///  json业务请求参数
        /// </summary>
        public string BizContent { get; set; }


        public Dictionary<string, string> toDic()
        {
            var type = this.GetType();
            object obj = this;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var prop in type.GetProperties())
            {
                if (!dic.ContainsKey(prop.Name))
                {
                    dic.Add(prop.Name, prop.GetValue(obj,null).ToString());
                }
            }
            return dic;
        }

        public string getKeyValueString(Dictionary<string, string> dics)
        {
            var result = "";
            bool first = true;
            foreach (var item in dics)
            {
                if (first)
                {
                    result += string.Format("{0}={1}", item.Key, item.Value);
                    first = false;
                }
                else {
                    result += string.Format("&{0}={1}", item.Key, item.Value);
                }               
            }
            return result;
        }

        public int GetStoreId()
        {
            return string.IsNullOrEmpty(this.StoreId) ? 0 : Convert.ToInt32(this.StoreId);
        }

    }
}
