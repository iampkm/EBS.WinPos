using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBS.WinPos.Domain.Entity
{
   public class VipProduct :BaseEntity
    {
       public int ProductId { get; set; }

       public decimal SalePrice { get; set; }
    }
}
