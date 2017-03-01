using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EBS.Infrastructure.Task;
using EBS.WinPos.Domain;
using EBS.WinPos.Domain.Entity;
using EBS.WinPos.Domain.ValueObject;
using EBS.WinPos.Service.Task;
using EBS.WinPos.Service.Dto;
using EBS.Infrastructure;
namespace EBS.WinPos.Service.Task
{
   public class WorkScheduleSyncTask :ITask
    {
        DapperContext _db;
        SyncService _syncService;
        public WorkScheduleSyncTask()
        {
            _db = new DapperContext();
            _syncService = new SyncService(AppContext.Log);
        }
        public void Execute()
        {
            AppContext.Log.Info("开始班次自动上传任务");
            string sql = "select * from WorkSchedule Where IsSync = @IsSync";
            var result = _db.Query<WorkSchedule>(sql, new { IsSync = 0});
            foreach (var model in result)
            {
                _syncService.Send(model);
            }
            AppContext.Log.Info("结束班次自动上传任务");
        }
    }
}
