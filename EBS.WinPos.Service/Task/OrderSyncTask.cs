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
    public class OrderSyncTask : ITask
    {
        DapperContext _db;
        SyncService _syncService;
        public OrderSyncTask()
        {
            _db = new DapperContext();
            _syncService = new SyncService(AppContext.Log);
        }
        public void Execute()
        {
            string sql = "select * from SaleOrder Where IsSync = @IsSync and (Status =@Paid or Status=@Cancel)";
            var result = _db.Query<SaleOrder>(sql, new { IsSync = 0, Paid = (int)SaleOrderStatus.Paid, Cancel = (int)SaleOrderStatus.Cancel });
            foreach (var model in result)
            {
                _syncService.Send(model);
            }
        }
    }
}
