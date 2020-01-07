using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using EBS.Infrastructure.Helper;
using System.Drawing;

namespace EBS.Infrastructure.Extension
{
   public static class stringExtension
   {
        /// <summary>
        /// 条形码
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="height">条码高度，默认60</param>
        /// <returns></returns>
        public static string CreateBarCode(this string content,int height=60)
        {
            BarCodeHelper barCodeHelper = new BarCodeHelper() {  Height = (uint)height};
            var bitmap = barCodeHelper.GetCodeImage(content, BarCodeHelper.Encode.Code128A);
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            string barCodeString = "data:image/jpeg;base64," + Convert.ToBase64String(ms.ToArray());
            bitmap.Dispose();
            ms.Dispose();
            return barCodeString;
        }
    }
}
