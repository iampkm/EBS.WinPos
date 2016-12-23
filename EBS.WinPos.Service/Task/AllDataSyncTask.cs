using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EBS.Infrastructure.Task;
using EBS.WinPos.Domain;
using EBS.WinPos.Domain.Entity;
using EBS.WinPos.Domain.ValueObject;
using EBS.WinPos.Service.Task;
using EBS.Infrastructure;
namespace EBS.WinPos.Service.Task
{
    public class AllDataSyncTask : ITask
    {
        DapperContext _db;
        SyncService _syncService;
        public AllDataSyncTask()
        {
            _db = new DapperContext();
            _syncService = new SyncService(AppContext.Log);
        }

        public void Execute()
        {
            _syncService.DownloadData();
        }
    }
}
