using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBS.WinPos.Service.Dto
{
   public class OrderInfo
    {
       public OrderInfo()
       {
           CanPay = false;
       }

       public int OrderId { get; set; }

       public string OrderCode { get; set; }

       public decimal OrderAmount { get; set; }
        public decimal PayAmount { get; set; }

       public decimal ChargeAmount { get {
           return PayAmount - OrderAmount;
       } }

       public bool CanPay { get; set; }

       
    }
}
