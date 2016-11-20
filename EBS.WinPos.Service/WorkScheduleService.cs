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

namespace EBS.WinPos.Service
{
   public class WorkScheduleService
    {
        Repository _db;
        public WorkScheduleService()
        {
            _db = new Repository();
        }
        
        /// <summary>
        /// 查询当班记录
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public WorkSchedule GetWorking(int storeId,int posId)
        {
            return _db.WorkSchedules.Where(n => n.StoreId == storeId && n.EndBy == 0 && n.PosId == posId).OrderByDescending(n=>n.Id).FirstOrDefault();           
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
                throw new Exception("当前机器没有人上班");
            }
            work.EndWork(account);
            _db.SaveChanges();
        }

        public void InputCashAmount(int id,decimal cashAmount)
        {
            var work = _db.WorkSchedules.Where(n => n.Id == id).FirstOrDefault();
            work.CashAmount = cashAmount;
            _db.SaveChanges();
        }
    }
}
