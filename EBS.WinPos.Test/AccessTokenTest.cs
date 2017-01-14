using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;
using EBS.Infrastructure.Extension;
namespace EBS.WinPos.Test
{
    [TestClass]
    public class AccessTokenTest
    {
        [TestMethod]
        public void GenerateAccessToken()
        {
            var storeId = 2;
            var posId = 301;
            var cdkey = "6d43d931e66bd88038aa384ddde7bdc8";
            MD5 md5Prider = MD5.Create();
            string clientCDKEY = string.Format("{0}{1}{2}", storeId, posId, cdkey);
            //加密    
            string clientCDKeyMd5 = md5Prider.GetMd5Hash(clientCDKEY);
        }

        [TestMethod]
        public void GenerateCDKeyMd5()
        {

            var cdkeySource = "20170111@b";
            MD5 md5Prider = MD5.Create();
           // string clientCDKEY = string.Format("{0}{1}{2}", storeId, posId, cdkeySource);
            //加密    
            string md5cdkey = md5Prider.GetMd5Hash(cdkeySource);

            Assert.AreEqual("1", md5cdkey);
        }
    }
}
