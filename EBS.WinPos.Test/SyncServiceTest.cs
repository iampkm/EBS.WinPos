using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EBS.Infrastructure;
using EBS.WinPos.Domain.Entity;
using EBS.WinPos.Service.Task;
using EBS.WinPos.Service;
using System.Security.Cryptography;
using EBS.Infrastructure.Extension;
namespace EBS.WinPos.Test
{
    [TestClass]
    public class SyncServiceTest
    {
        //SyncService _syncService;

        //SaleOrderService _saleService;

        [TestInitialize]
        public void Init()
        {
            //_syncService = new SyncService(AppContext.Log);
            //_saleService = new SaleOrderService();
        }

        [TestMethod]
        public void PostWorkScheduleTest()
        {
            // _syncService.Send()

          //  _saleService.CashPay(98, 10);

            Assert.AreEqual(1, 1);
        }
         [TestMethod]
        public void md5_test()
        {
            int storeId = 2;
            int posId = 301;
            string ClientCDKey = "c3089eaeb92880e92e1c1545a2ddf3d6";
            MD5 md5Prider = MD5.Create();
            string clientCDKEY = string.Format("{0}{1}{2}", storeId, posId, ClientCDKey);
            //加密    
            string clientCDKeyMd5 = md5Prider.GetMd5Hash(clientCDKEY);
        }
         [TestMethod]
        public void md5_test_2()
        {
            string SourceKey = "20170101@a";
            MD5 md5Prider = MD5.Create();            //加密    
            string clientCDKeyMd5 = md5Prider.GetMd5Hash(SourceKey);
        }
    }
}
