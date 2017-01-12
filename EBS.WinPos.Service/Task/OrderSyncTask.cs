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
using System.Threading;
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
            Thread.Sleep(3000);  //延迟3秒 发送，避免和 及时订单产生并发
            foreach (var model in result)
            {
                string sqlitem = "select * from SaleOrderItem where SaleOrderId=@SaleOrderId";
                var items = _db.Query<SaleOrderItem>(sqlitem, new { SaleOrderId = model.Id}).ToList();
                model.Items = items;
                Thread.Sleep(100);
                _syncService.Send(model);
              
            }

            // 上传销售对账数据
            _syncService.UploadSaleSync(DateTime.Now);
        }
    }
}
