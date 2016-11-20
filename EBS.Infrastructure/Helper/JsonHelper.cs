#region << 注 释 >>
/*
 * ========================================================================
 * 
 * *****************************【程序集引用】*****************************
 * System.Web
 * System.Web.Extensions
 * System.ServiceModel.Web
 * System.Runtime.Serialization
 * 
 * ========================================================================
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;
//using System.Runtime.Serialization.Json;
//using System.Web.Script.Serialization;
using Newtonsoft.Json;
namespace EBS.Infrastructure.Helper
{
    /// <summary>
    /// JSON序列化和反序列化辅助类
    /// </summary>
    public class JsonHelper
    {
        #region 私有属性
        //private static JavaScriptSerializer _javaScriptSerializer = null;
        ///// <summary>
        ///// 提供 JavaScript 序列化和反序列化功能.
        ///// </summary>
        //private static JavaScriptSerializer JavaScriptSerializer
        //{
        //    get
        //    {
        //        if (_javaScriptSerializer == null)
        //        {
        //            _javaScriptSerializer = new JavaScriptSerializer();
        //        }
        //        return _javaScriptSerializer;
        //    }
        //}
        #endregion

        #region 对象序列化成Json对象
        /// <summary>
        /// 获取对象的JSON序列化字符串.
        /// </summary>
        /// <param name="obj">要序列化的对象.</param>
        /// <returns>对象的JSON序列化字符串.</returns>
        public static string GetJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
           // return JavaScriptSerializer.Serialize(obj);
        }

        /// <summary>
        /// JSON序列化
        /// </summary>
        /// <param name="t"></param>
        public static string JsonSerializer<T>(T t)
        {
            //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            //MemoryStream ms = new MemoryStream();
            //ser.WriteObject(ms, t);
            //string jsonString = Encoding.UTF8.GetString(ms.ToArray());
            //ms.Close();
            string jsonString = JsonConvert.SerializeObject(t);
            //替换Json的Date字符串
            string p = @"\\/Date\((\d+)\+\d+\)\\/";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            return jsonString;
        }

        /// <summary>
        /// 将Json序列化的时间由/Date(1294499956278+0800)转为字符串
        /// </summary>
        /// <param name="m"></param>
        private static string ConvertJsonDateToDateString(Match m)
        {
            string result = string.Empty;
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
            dt = dt.ToLocalTime();
            result = dt.ToString("yyyy-MM-dd HH:mm:ss.fff");
            return result;
        }
        #endregion

        #region Json对象反序列化成对象
        /// <summary>
        /// JSON反序列化
        /// </summary>
        /// <param name="jsonString"></param>
        public static T JsonDeserialize<T>(string jsonString)
        {
            //将"yyyy-MM-dd HH:mm:ss"格式的字符串转为"\/Date(1294499956278+0800)\/"格式
            string p = @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            //MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            //T obj = (T)ser.ReadObject(ms); return obj;
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        /// <summary>
        /// 将时间字符串转为Json时间
        /// </summary>
        /// <param name="m"></param>
        private static string ConvertDateStringToJsonDate(Match m)
        {
            string result = string.Empty;
            DateTime dt = DateTime.Parse(m.Groups[0].Value);
            dt = dt.ToUniversalTime();
            TimeSpan ts = dt - DateTime.Parse("1970-01-01");
            result = string.Format("\\/Date({0}+0800)\\/", ts.TotalMilliseconds);
            return result;
        }
        #endregion

        #region DataTable转换为Json对象
        /// <summary>
        /// DataTable转换为Json对象
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetJson(DataTable dt)
        {
            string json = "";
            if (dt == null || dt.Rows.Count == 0)
            {
                return "[]";
            }
            foreach (DataRow dr in dt.Rows)
            {
                string objectJson = "";
                foreach (DataColumn dc in dt.Columns)
                {
                    string propertyJson = string.Format("{0}:{1}", dc.ColumnName, JsonSerializer(dr[dc.ColumnName].ToString()));
                    objectJson += "," + propertyJson;
                }
                objectJson = objectJson.Substring(1);
                objectJson = "{" + objectJson + "}";
                json += "," + objectJson;
            }
            json = string.Format("[{0}]", json.Substring(1));
            return json;
        }
        #endregion

        /*
        public static string GetJsonNode(string jsonString, string nodeName)
        {
            try
            {
                var json = JObject.Parse(jsonString);
                var node = json[nodeName];
                if (node != null && node.Parent.HasValues)
                    if (node.HasValues)
                        return JsonConvert.SerializeObject(node);
                    else
                        return node.ToString();
            }
            catch { }
            return null;
        }*/
    }
}
