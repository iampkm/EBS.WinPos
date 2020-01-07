//using ICSharpCode.SharpZipLib.GZip;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Threading.Tasks;

//namespace EBS.Infrastructure.Helper
//{
//   public class HttpRequestHelper
//    {
//        /// <summary>
//        /// 使用post方法异步请求
//        /// </summary>
//        /// <param name="url">目标链接</param>
//        /// <param name="json">发送的参数字符串，只能用json</param>
//        /// <returns>返回的字符串</returns>
//        public static async Task<string> PostAsyncJson(string url, string json)
//        {
//            HttpClient client = new HttpClient();
//            HttpContent content = new StringContent(json);
//            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
//            HttpResponseMessage response = await client.PostAsync(url, content);
//            response.EnsureSuccessStatusCode();
//            string responseBody = await response.Content.ReadAsStringAsync();
//            return responseBody;
//        }

//        /// <summary>
//        /// 使用post方法异步请求
//        /// </summary>
//        /// <param name="url">目标链接</param>
//        /// <param name="data">发送的参数字符串</param>
//        /// <returns>返回的字符串</returns>
//        public static async Task<string> PostAsync(string url, string data, Dictionary<string, string> header = null, bool Gzip = false)
//        {
//            HttpClient client = new HttpClient(new HttpClientHandler() { UseCookies = false });
//            HttpContent content = new StringContent(data);
//            if (header != null)
//            {
//                client.DefaultRequestHeaders.Clear();
//                foreach (var item in header)
//                {
//                    client.DefaultRequestHeaders.Add(item.Key, item.Value);
//                }
//            }
//            HttpResponseMessage response = await client.PostAsync(url, content);
//            response.EnsureSuccessStatusCode();
//            string responseBody = "";
//            if (Gzip)
//            {
//                GZipInputStream inputStream = new GZipInputStream(await response.Content.ReadAsStreamAsync());
//                responseBody = new StreamReader(inputStream).ReadToEnd();
//            }
//            else
//            {
//                responseBody = await response.Content.ReadAsStringAsync();

//            }
//            return responseBody;
//        }

//        /// <summary>
//        /// 使用get方法异步请求
//        /// </summary>
//        /// <param name="url">目标链接</param>
//        /// <returns>返回的字符串</returns>
//        public static async Task<string> GetAsync(string url, Dictionary<string, string> header = null, bool Gzip = false)
//        {

//            HttpClient client = new HttpClient(new HttpClientHandler() { UseCookies = false });
//            if (header != null)
//            {
//                client.DefaultRequestHeaders.Clear();
//                foreach (var item in header)
//                {
//                    client.DefaultRequestHeaders.Add(item.Key, item.Value);
//                }
//            }
//            HttpResponseMessage response = await client.GetAsync(url);
//            response.EnsureSuccessStatusCode();//用来抛异常的
//            string responseBody = "";
//            if (Gzip)
//            {
//                GZipInputStream inputStream = new GZipInputStream(await response.Content.ReadAsStreamAsync());
//                responseBody = new StreamReader(inputStream).ReadToEnd();
//            }
//            else
//            {
//                responseBody = await response.Content.ReadAsStringAsync();

//            }
//            return responseBody;
//        }

//        /// <summary>
//        /// 使用post返回异步请求直接返回对象
//        /// </summary>
//        /// <typeparam name="T">返回对象类型</typeparam>
//        /// <typeparam name="T2">请求对象类型</typeparam>
//        /// <param name="url">请求链接</param>
//        /// <param name="obj">请求对象数据</param>
//        /// <returns>请求返回的目标对象</returns>
//        public static async Task<T> PostObjectAsync<T, T2>(string url, T2 obj)
//        {
//            String json = JsonConvert.SerializeObject(obj);
//            string responseBody = await PostAsyncJson(url, json); //请求当前账户的信息
//            return JsonConvert.DeserializeObject<T>(responseBody);//把收到的字符串序列化
//        }

//        /// <summary>
//        /// 使用Get返回异步请求直接返回对象
//        /// </summary>
//        /// <typeparam name="T">请求对象类型</typeparam>
//        /// <param name="url">请求链接</param>
//        /// <returns>返回请求的对象</returns>
//        public static async Task<T> GetObjectAsync<T>(string url)
//        {
//            string responseBody = await GetAsync(url); //请求当前账户的信息
//            return JsonConvert.DeserializeObject<T>(responseBody);//把收到的字符串序列化
//        }

//    }
//}
