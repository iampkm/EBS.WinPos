using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EBS.WinPos.Domain.Entity;
using EBS.WinPos.Domain;
using System.Diagnostics;
using System.Data.SQLite;
using EBS.WinPos.Service.Dto;
using EBS.WinPos.Domain.ValueObject;
using EBS.Infrastructure;
using EBS.WinPos.Service.Task;
using EBS.Infrastructure.Helper;
using EBS.Infrastructure.Extension;
namespace EBS.WinPos.Service
{
   public class WorkScheduleService
    {
        Repository _db;
        SyncService _syncService;
        DapperContext _query;
        IPosPrinter _printService;
        public WorkScheduleService()
        {
            _db = new Repository();
            _syncService = new SyncService(AppContext.Log);
            _query = new DapperContext();
            _printService = new DriverPrinterService();
        }
        
        /// <summary>
        /// 查询当班记录
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public WorkSchedule GetWorking(int storeId,int posId)
        {
           // return _db.WorkSchedules.Where(n => n.StoreId == storeId && n.EndBy == 0 && n.PosId == posId).FirstOrDefault();   
            string sql = "select * from WorkSchedule where StoreId = @StoreId and EndBy = 0 and PosId = @PosId order by Id desc";
            return _query.First<WorkSchedule>(sql, new { StoreId= storeId,PosId =posId});
        }

        public List<WorkSchedule> GetWorkList(DateTime date, int storeId, int posId, int CreatedBy)
        {
            string sql = "select * from workschedule where storeId=@StoreId and posId=@PosId and  date(StartDate) =@StartDate";
            if (CreatedBy > 0)
            {
                sql += " and CreatedBy=" + CreatedBy;
            }
            var data = _query.Query<WorkSchedule>(sql, new { StoreId = storeId, PosId = posId, StartDate = date.ToString("yyyy-MM-dd") }).ToList();
            return data;
            //var data = _db.WorkSchedules.Where(n => n.StoreId == storeId && n.PosId == posId && n.CreatedBy == CreatedBy).OrderByDescending(n => n.Id).ToList();
            //var result= data.Where(n => n.StartDate.Date <= date && CheckEndDate(n.EndDate,date)).ToList();
            //return result;          
        }

        private bool CheckEndDate(DateTime? endDate,DateTime date)
        {
            if (endDate.HasValue)
            {
                return endDate.Value.Date > date;
            }
            else {
                return true;
            }
        }

        public void BeginWork(Account account,int storeId,int posId)
        {
            // 验证是否有未结束的上班记录
            var work = GetWorking(storeId, posId);
            if (work != null)
            {
                throw new AppException("必须交班后才能开始下一个班次");
            }

            WorkSchedule model = new WorkSchedule();
            model.StoreId = storeId;
            model.CreatedBy = account.Id;
            model.CreatedByName = account.NickName;
            model.PosId = posId;           
            
            _db.WorkSchedules.Add(model);
            _db.SaveChanges();
            // 同步到服务器
            _syncService.Send(model);
        }

        public void EndWork(Account account,int storeId,int posId)
        {
            var work = _db.WorkSchedules.Where(n => n.StoreId == storeId && n.EndBy == 0 && n.PosId == posId).OrderByDescending(n => n.Id).FirstOrDefault();
            if (work == null)
            {
                throw new AppException("当前机器没有人上班");
            }
            work.EndWork(account);
            _db.SaveChanges();
            _syncService.Send(work);
        }

        public void InputCashAmount(int id,decimal cashAmount)
        {
            var work = _db.WorkSchedules.Where(n => n.Id == id).FirstOrDefault();
            work.InputCashAmount(cashAmount);
            _db.SaveChanges();
            // 同步收现金额到服务器
            _syncService.Send(work);
        }

        public void PrintWorkSaleSummary(int id)
        {
            string sql = @"select w.CreatedByName,s.Name as StoreName,w.PosId,w.StartDate,w.EndDate,t.TotalAmount,t.TotalOnlineAmount,t.paymentWay from WorkSchedule w left join (
select o.WorkScheduleCode,sum(OrderAmount) as TotalAmount,sum(OnlinePayAmount) as TotalOnlineAmount,paymentWay from  saleorder o where o.Status = 3 group by o.WorkScheduleCode,o.paymentWay
) t on t.WorkScheduleCode = w.Code
inner join Store s on s.Id = w.StoreId
where w.Id=@Id";
           var models=  _query.Query<WorkSaleSummaryDto>(sql, new { Id = id }).ToList();
            if (models.Count == 0)
            {
                throw new Exception("没有数据");
            }
            string template = FileHelper.ReadText("WorkSaleSummaryTemplate.txt");
            if (string.IsNullOrEmpty(template)) { throw new AppException("收银汇总模板为空"); }
            template = template.ToLower();
            var lineLocation = template.LastIndexOf("##itemtemplate##");
            //分离商品item模板
            string billTemplate = template.Substring(0, lineLocation);
            var itemStr = template.Substring(lineLocation);
            var len = itemStr.IndexOf("{");  //去掉　##itemtemplate##　以及换行符　从{{productname}}
            string itemTemplate = itemStr.Substring(len);
            var model = models[0];


            billTemplate = billTemplate.ToLower();
            billTemplate = billTemplate.Replace("{{storename}}", model.StoreName);
            billTemplate = billTemplate.Replace("{{createdbyname}}", model.CreatedByName);
            billTemplate = billTemplate.Replace("{{posid}}",  model.PosId.ToString());
            billTemplate = billTemplate.Replace("{{startdate}}", model.StartDate.ToString("yyyy-MM-dd HH:mm:ss"));
            billTemplate = billTemplate.Replace("{{enddate}}", model.EndDate.HasValue? model.EndDate.Value.ToString("yyyy-MM-dd HH:mm:ss"):"");
            billTemplate = billTemplate.Replace("{{today}}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            // 金额
            string productItems = "";
            foreach (var item in models)
            {
                string tempItem = itemTemplate;
                tempItem = tempItem.Replace("{{totalamount}}", item.TotalAmount.ToString("F2"));
                tempItem = tempItem.Replace("{{totalonlineamount}}", item.TotalOnlineAmount.ToString("F2"));
                tempItem = tempItem.Replace("{{paymentway}}", item.PaymentWay.Description());
                productItems += tempItem; 
            }
            billTemplate = billTemplate.Replace("{{items}}", productItems);
            _printService.Print(billTemplate);
        }
    }
}
