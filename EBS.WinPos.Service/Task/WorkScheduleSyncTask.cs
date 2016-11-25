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
            string sql = "select * from WorkSchedule Where IsSync = @IsSync or IsSyncAmount=@IsSyncAmount)";
            var result = _db.Query<WorkSchedule>(sql, new { IsSync = 0,IsSyncAmount= 0});
            foreach (var model in result)
            {
                if (model.IsSync == 0)
                {
                    _syncService.Send(model);
                }
                if (model.IsSyncAmount==0)
                {
                    _syncService.Send(new InputCashAmount(model.Id, model.CashAmount,model.PosId,model.StoreId));
                }
            }
        }
    }
}
