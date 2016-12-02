using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EBS.Infrastructure;
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
           Code = Guid.NewGuid().ToString().Replace("-", "");
       }
       public string Code { get; private set; }
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
       /// <summary>
       /// 是否同步记录
       /// </summary>
       public int IsSync { get; set; }
       /// <summary>
       /// 是否同步金额
       /// </summary>
       public int IsSyncAmount { get; set; }

       public void EndWork(Account account)
       {
           this.EndBy = account.Id;
           this.EndByName = account.NickName;
           this.EndDate = DateTime.Now;
       }

       public void InputCashAmount(decimal amount)
       {
           TimeSpan ts= DateTime.Now-StartDate;
           if (ts.TotalDays > 7)
           {
               throw new AppException("上班记录已经超过了7天，不能修改收现金额");
           }
           this.CashAmount = amount;
       }

       
    }
}
