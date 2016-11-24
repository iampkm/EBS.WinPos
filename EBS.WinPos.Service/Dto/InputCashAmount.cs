using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBS.WinPos.Service.Dto
{
    /// <summary>
    /// 录入收现金额
    /// </summary>
   public class InputCashAmount
    {
       private InputCashAmount() { }
       public InputCashAmount(int id, decimal money)
       {
           this.Id = id;
           this.Money = money;
       }

       /// <summary>
       /// 班次Id
       /// </summary>
       public int Id { get;private set; }
       /// <summary>
       /// 收现金额
       /// </summary>
       public decimal Money { get; private set; }
    }
}
