using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBS.WinPos.Domain.Entity
{
   public class VipCard:BaseEntity
    {
         public string Code { get; set; }
        
         public decimal Discount { get; set; } 
    }
}
