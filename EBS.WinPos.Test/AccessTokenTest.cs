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
            var storeId = 15;
            var posId = 201;
            var cdkey = "9e591dfa860110bc9b3974d391dbba05";
            MD5 md5Prider = MD5.Create();
            string clientCDKEY = string.Format("{0}{1}{2}", storeId, posId, cdkey);
            //加密    
            string clientCDKeyMd5 = md5Prider.GetMd5Hash(clientCDKEY);
            Assert.AreEqual("1", clientCDKeyMd5);

        }

        [TestMethod]
        public void GenerateCDKeyMd5()
        {

            var cdkeySource = "20170301@qaz";
            MD5 md5Prider = MD5.Create();
           // string clientCDKEY = string.Format("{0}{1}{2}", storeId, posId, cdkeySource);
            //加密    
            string md5cdkey = md5Prider.GetMd5Hash(cdkeySource);

            Assert.AreEqual("1", md5cdkey);
        }
    }
}
