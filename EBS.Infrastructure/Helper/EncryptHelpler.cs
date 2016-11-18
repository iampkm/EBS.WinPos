using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace EBS.Infrastructure.Helper
{
    public static class EncryptHelpler
    {
        /// <summary>
        /// 生成签名
        /// </summary>
        /// <param name="data">生成签名的数据(json字符串)</param>
        /// <returns></returns>
        public static string SignEncrypt(string data, string signKey)
        {
            JObject json = JObject.Parse(data);

            //遍历json的节点，并获取节点的值，并根据属性排升序
            var dict = new SortedDictionary<string, string>();
            var properties = json.Properties().ToList();
            foreach (var item in properties)
            {
                dict.Add(item.Name, item.Value.ToString());
            }

            //组装被加密的字符串
            string signString = string.Empty;
            foreach (var item in dict)
            {
                signString += string.Format("{0}={1}&", item.Key, item.Value);
            }
            signString += signKey;

            //对数据进行MD5加密
            string sign = MD5Encrypt(signString);
            return sign;
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="strText">加密内容</param>
        /// <param name="toLower">是否转换成小写</param>
        /// <param name="encoding">编码方式</param>
        /// <returns></returns>
        public static string MD5Encrypt(string strText, bool toLower = false, Encoding encoding = null)
        {
            string code = string.Empty;
            if (encoding == null)
                encoding = Encoding.UTF8;

            var md5 = System.Security.Cryptography.MD5.Create();//实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择
            byte[] s = md5.ComputeHash(encoding.GetBytes(strText));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
                if (toLower)
                    code += s[i].ToString("x2");
                else
                    code += s[i].ToString("X2");
            }
            return code;
        }
    }
}
