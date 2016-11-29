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
namespace EBS.WinPos.Service
{
   public class WorkScheduleService
    {
        Repository _db;
        SyncService _syncService;
        DapperContext _query;
        public WorkScheduleService()
        {
            _db = new Repository();
            _syncService = new SyncService(AppContext.Log);
            _query = new DapperContext();
        }
        
        /// <summary>
        /// 查询当班记录
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public WorkSchedule GetWorking(int storeId,int posId)
        {
           // return _db.WorkSchedules.Where(n => n.StoreId == storeId && n.EndBy == 0 && n.PosId == posId).FirstOrDefault();   
            string sql = "select * from WorkSchedule where StoreId = @StoreId and EndBy = 0 and PosId = @PostId order by Id desc";
            return _query.First<WorkSchedule>(sql, new { StoreId= storeId,PostId =posId});
        }

        public List<WorkSchedule> GetWorkList(DateTime date, int storeId, int posId, int CreatedBy)
        {
            var data = _db.WorkSchedules.Where(n => n.StoreId == storeId && n.PosId == posId && n.CreatedBy == CreatedBy).OrderByDescending(n => n.Id).ToList();
            var result= data.Where(n => n.StartDate.Date <= date && CheckEndDate(n.EndDate,date)).ToList();
            return result;          
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

        public void BeginWork(Account account,int posId)
        {
            WorkSchedule model = new WorkSchedule();
            model.StoreId = account.StoreId;
            model.CreatedBy = account.Id;
            model.CreatedByName = account.NickName;
            model.PosId = posId;
            _db.WorkSchedules.Add(model);
            _db.SaveChanges();
        }

        public void EndWork(Account account,int posId)
        {
            var work = _db.WorkSchedules.Where(n => n.StoreId == account.StoreId && n.EndBy == 0 && n.PosId == posId).OrderByDescending(n => n.Id).FirstOrDefault();
            if (work == null)
            {
                throw new AppException("当前机器没有人上班");
            }
            work.EndWork(account);
            _db.SaveChanges();
        }

        public void InputCashAmount(int id,decimal cashAmount)
        {
            var work = _db.WorkSchedules.Where(n => n.Id == id).FirstOrDefault();
            work.CashAmount = cashAmount;
            _db.SaveChanges();
            // 同步收现金额到服务器
            _syncService.Send(work);
        }
    }
}
