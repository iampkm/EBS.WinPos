using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EBS.Infrastructure;
using EBS.WinPos.Domain.Entity;
using EBS.WinPos.Service.Task;
using EBS.WinPos.Service;
namespace EBS.WinPos.Test
{
    [TestClass]
    public class SyncServiceTest
    {
        SyncService _syncService;

        SaleOrderService _saleService;

        [TestInitialize]
        public void Init()
        {
            _syncService = new SyncService(AppContext.Log);
            _saleService = new SaleOrderService();
        }

        [TestMethod]
        public void PostWorkScheduleTest()
        {
            // _syncService.Send()

            _saleService.CashPay(98, 10);

            Assert.AreEqual(1, 1);
        }
    }
}
