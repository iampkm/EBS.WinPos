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

       public string ProductName { get; set; }
       public string ProductCode { get; set; }

       public string BarCode { get; set; }
       public string Specification { get; set; }

       public decimal SalePrice { get; set; }

       public int Quantity { get; set; }

       public decimal Amount { get {
           return SalePrice * Quantity;
       } }
       public virtual int SaleOrderId { get; set; }
    }
}
