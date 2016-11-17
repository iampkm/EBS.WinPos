using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBS.WinPos.Domain.Entity
{
   public class SaleOrderItem:BaseEntity
    {
       public int ProductId { get; set; }
       public string ProductName { get; set; }
       public string ProductCode { get; set; }
       public decimal SalePrice { get; set; }
       public int Quantity { get; set; }    
       public virtual int SaleOrderId { get; set; }
    }
}
