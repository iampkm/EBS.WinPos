using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBS.WinPos.Domain.Entity
{
   public class SaleOrderItem
    {
       public int Id { get; set; }

       public int ProductId { get; set; }

       public virtual int SaleOrderId { get; set; }
    }
}
