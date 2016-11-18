using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;
using EBS.Infrastructure.Log;
namespace EBS.Infrastructure.Helper
{
    /// <summary>
    /// 基于HTTP协议操作类
    /// </summary>
    public class HttpHelper
    {
        #region 异常日志
       // private static string _logDir = "HttpHelperError";
        private static string _logFormat = @"
Http请求异常！
异常描述：{0}
异常位置：{1}
请求地址：{2}
请求参数：{3}";
        /// <summary>
        /// 记录异常日志
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="url">请求地址</param>
        /// <param name="param">请求参数</param>
        private static void WriteLog(Exception ex, string url, string param)
        {
            ILogger log = new NLogWriter(NLog.LogManager.GetCurrentClassLogger());
            log.Error(ex, _logFormat, ex.Message, ex.StackTrace, url, param);
          //  LogWriter.WriteLog(string.Format(_logFormat, ex.Message, ex.StackTrace, url, param), _logDir, ExceptionHelper.ExceptionLevel.Exception);
        }
        #endregion

        #region 默认配置
        /// <summary>
        /// UTF8编码
        /// </summary>
        private static Encoding _encoding = Encoding.UTF8;
        #endregion

        #region HttpPost请求
        /// <summary>
        /// HttpPost请求
        /// </summary>
        /// <param name="url">请求url地址</param>
        /// <param name="param">请求参数</param>
        /// <param name="encode">编码方式</param>
        /// <returns></returns>
        public static string HttpPost(string url, string param, Encoding encode = null)
        {
            try
            {
                encode = encode == null ? _encoding : encode;
                byte[] data = encode.GetBytes(param);
                WebClient client = new WebClient();
                client.Credentials = CredentialCache.DefaultCredentials;
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                byte[] rsp = client.UploadData(new Uri(url), "POST", data);
                client.Dispose();
                string strRsp = HttpUtility.UrlDecode(encode.GetString(rsp));
                return strRsp;
            }
            catch (System.Exception ex)
            {
                WriteLog(ex, url, param);
            }
            return null;
        }
        #endregion

        #region HttpGet请求
        /// <summary>
        /// HttpGet请求
        /// </summary>
        /// <param name="url">请求url地址</param>
        /// <param name="encode">编码方式</param>
        /// <returns></returns>
        public static string HttpGet(string url, Encoding encode = null)
        {
            return HttpRequest("GET", url, encode: encode);
        }
        #endregion

        #region HttpRequest请求
        /// <summary>
        /// HttpRequest请求
        /// </summary>
        /// <param name="method">提交类型(POST GET)</param>
        /// <param name="url">请求地址</param>
        /// <param name="param">请求参数</param>
        /// <param name="endcode">编码方式</param>
        /// <param name="timeOut">超时时间(单位:毫秒)</param>
        /// <returns></returns>
        public static string HttpRequest(string method, string url, string param = null, Encoding encode = null, int timeOut = 0)
        {
            try
            {
                if (method != null && (method.ToUpper() == "POST" || method.ToUpper() == "GET"))
                {
                    WebRequest request = WebRequest.Create(url);
                    request.Method = method.ToUpper();
                    encode = encode == null ? _encoding : encode;
                    if (request.Method == "POST")
                    {
                        param = string.IsNullOrEmpty(param) ? string.Empty : param;
                        byte[] data = encode.GetBytes(param);
                        request.ContentLength = data.Length;
                        request.ContentType = "application/x-www-form-urlencoded";
                        Stream writer = request.GetRequestStream();
                        writer.Write(data, 0, data.Length);
                        writer.Close();
                    }
                    if (timeOut > 0)
                    {
                        request.Timeout = timeOut;
                    }
                    WebResponse response = request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream(), encode);
                    string responseStr = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                    return responseStr;
                }
            }
            catch (System.Exception ex)
            {
                WriteLog(ex, url, param);
            }
            return null;
        }
        #endregion

        #region Http下载文件
        /// <summary>
        /// Http下载文件
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="filePath">文件保存路径</param>
        public static bool HttpDownLoadFile(string url, string filePath)
        {
            try
            {
                #region 文件下载方法一
                //WebClient client = new WebClient();
                //client.DownloadFile(new Uri(url), filePath);
                //client.Dispose();
                #endregion

                #region 文件下载方法二
                WebRequest request = WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                Stream reader = response.GetResponseStream();
                FileStream writer = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                byte[] buff = new byte[512];
                int c = 0; //实际读取的字节数
                while ((c = reader.Read(buff, 0, buff.Length)) > 0)
                {
                    writer.Write(buff, 0, c);
                }
                writer.Close();
                reader.Close();
                response.Close();
                #endregion
                return true;
            }
            catch (System.Exception ex)
            {
                WriteLog(ex, url, filePath);
            }
            return false;
        }
        #endregion

        #region Http下载网页
        /// <summary>
        /// Http下载网页
        /// </summary>
        /// <param name="url">url地址</param>
        /// <returns></returns>
        public static string HttpDownLoadHtml(string url, Encoding encode = null)
        {
            try
            {
                WebClient client = new WebClient();
                byte[] data = client.DownloadData(new Uri(url));
                client.Dispose();
                encode = encode == null ? _encoding : encode;
                string html = encode.GetString(data);
                return html;
            }
            catch (System.Exception ex)
            {
                WriteLog(ex, url, string.Empty);
            }
            return null;
        }
        #endregion

        #region 当前请求的IP
        /// <summary>
        /// 获取当前请求的IP
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentIP()
        {
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (!string.IsNullOrEmpty(ip))
            {
                //可能有代理
                if (ip.IndexOf(".") == -1)//没有“.”肯定是非IPv4格式
                    ip = null;
                else if (ip.IndexOf(",") != -1)
                {
                    //有“,”，估计多个代理，取第一个不是内网的IP
                    ip = ip.Replace(" ", "").Replace("'", "");
                    string[] temparyip = ip.Split(",;".ToCharArray());
                    for (int i = 0; i < temparyip.Length; i++)
                    {
                        if (IsIpAddress(temparyip[i]) && temparyip[i].Substring(0, 3) != "10." && temparyip[i].Substring(0, 7) != "192.168" && temparyip[i].Substring(0, 7) != "172.16.")
                            return temparyip[i];//找到不是内网的地址
                    }
                }
                else if (IsIpAddress(ip))//代理即是IP格式
                    return ip;
                else
                    ip = null;//代理中的内容 非IP，取IP
            }

            if (string.IsNullOrEmpty(ip))
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            if (string.IsNullOrEmpty(ip))
                ip = System.Web.HttpContext.Current.Request.UserHostAddress;

            ip = ip == "::1" ? "127.0.0.1" : ip;//本机开启了ipv6，依然返回本机的ipv4值
            return ip;
        }

        /// <summary>
        ///  判断是否是IP地址
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private static bool IsIpAddress(string ip)
        {
            if (ip == null || ip == string.Empty || ip.Length < 7 || ip.Length > 15) return false;
            string regformat = @"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$";
            Regex regex = new Regex(regformat, RegexOptions.IgnoreCase);
            return regex.IsMatch(ip);
        }
        #endregion

        /// <summary>
        /// HttpPost SOAP
        /// </summary>
        /// <param name="requestUriString">请求地址</param>
        /// <param name="soapRequest">SOAP Request Body</param>
        /// <param name="contentType">SOAP Request ContentType</param>
        /// <returns>SOAP Response</returns>
        public static string HttpPostSoap(string requestUriString, string soapRequest, string contentType = "text/xml;charset=UTF-8")
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(requestUriString);
            webRequest.Proxy = null;
            //发送请求
            webRequest.Method = "POST";
            //编码
            webRequest.ContentType = contentType;

            Stream writer = webRequest.GetRequestStream();
            byte[] bytes = Encoding.UTF8.GetBytes(soapRequest);
            writer.Write(bytes, 0, bytes.Length);
            writer.Flush();
            writer.Close();
            string result = "";
            
            HttpWebResponse webResponse = null;
            try
            {
                webResponse = webRequest.GetResponse() as HttpWebResponse;
                //流读取
                Stream rStream = webResponse.GetResponseStream();
                StreamReader sr = new StreamReader(rStream, Encoding.UTF8);
                result = sr.ReadToEnd();
                sr.Close();
                rStream.Close();
            }
            catch (System.Net.WebException ex)
            {
                result = ex.Message;
            }
            finally
            {
                if (webResponse != null)
                    webResponse.Close();
            }
            return result;
        }
    }
}
