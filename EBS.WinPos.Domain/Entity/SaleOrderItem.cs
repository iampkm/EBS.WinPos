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
        /// <summary>
        /// 商品售价
        /// </summary>
       public decimal SalePrice { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        public decimal Discount { get; set; }
        /// <summary>
        /// 实际折后价
        /// </summary>
        public decimal RealPrice { get; set; }
        public int Quantity { get; set; }    
       public virtual int SaleOrderId { get; set; }
    }
}
