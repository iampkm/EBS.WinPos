using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBS.WinPos.Domain.Entity
{
   public class SaleOrder
    {
       public SaleOrder()
       {
           this.Items = new List<SaleOrderItem>();
       }
       public int Id { get; set; }

       public string Code { get; set; }

       public DateTime CreatedOn { get; set; }

       public int CreatedBy { get; set; }

       public virtual List<SaleOrderItem> Items { get; set; }
    }

    
}
