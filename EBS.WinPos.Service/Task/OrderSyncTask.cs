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
            AppContext.Log.Info("开始销售自动上传任务");
            var today = DateTime.Now.AddMinutes(-10); // 自动任务同步10分钟以前的历史订单，避免与当前订单产生并发冲突            
            string sql = "select * from SaleOrder Where IsSync = @IsSync and (Status =@Paid or Status=@Cancel) and datetime(CreatedOn)<@CreatedOn";
            var result = _db.Query<SaleOrder>(sql, new { IsSync = 0, Paid = (int)SaleOrderStatus.Paid, Cancel = (int)SaleOrderStatus.Cancel, CreatedOn=today });
            Thread.Sleep(3000);  //延迟3秒 发送，避免和 及时订单产生并发
            foreach (var model in result)
            {
                string sqlitem = "select * from SaleOrderItem where SaleOrderId=@SaleOrderId";
                var items = _db.Query<SaleOrderItem>(sqlitem, new { SaleOrderId = model.Id}).ToList();
                model.Items = items;
                Thread.Sleep(100);
                _syncService.Send(model);
              
            }
            AppContext.Log.Info("结束销售自动上传任务");
            // 上传销售对账数据
            AppContext.Log.Info("开始销售对账自动上传任务");
            _syncService.UploadSaleSync(DateTime.Now);
            AppContext.Log.Info("结束销售对账自动上传任务");
        }
    }
}
