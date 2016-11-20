using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBS.WinPos.Domain.Entity
{
    /// <summary>
    /// 班次表
    /// </summary>
   public class WorkSchedule:BaseEntity
    {
       public WorkSchedule()
       {
           this.StartDate = DateTime.Now;
       }
       public int StoreId { get; set; }

       public int PosId { get; set; }

       public decimal CashAmount { get; set; }
       /// <summary>
       /// 上班人
       /// </summary>
       public int CreatedBy { get; set; }
       public string CreatedByName { get; set; }
       public DateTime StartDate { get; set; }
       public DateTime? EndDate { get; set; }
       /// <summary>
       /// 交班人
       /// </summary>
       public int EndBy { get; set; }
       public string EndByName { get; set; }

       public void EndWork(Account account)
       {
           this.EndBy = account.Id;
           this.EndByName = account.NickName;
           this.EndDate = DateTime.Now;
       }

       
    }
}
