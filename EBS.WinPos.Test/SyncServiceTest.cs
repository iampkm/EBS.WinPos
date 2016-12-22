using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EBS.Infrastructure;
using EBS.WinPos.Domain.Entity;
using EBS.WinPos.Service.Task;
namespace EBS.WinPos.Test
{
    [TestClass]
    public class SyncServiceTest
    {
        SyncService _syncService;

        [TestInitialize]
        public void Init()
        {
            _syncService = new SyncService(AppContext.Log);
        }

        [TestMethod]
        public void PostWorkScheduleTest()
        {
           // _syncService.Send()
        }
    }
}
