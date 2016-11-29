using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBS.WinPos.Domain.Entity
{
    /// <summary>
    /// 商品区域价
    /// </summary>
   public class ProductAreaPrice:BaseEntity
    {
       public int ProductId { get; set; }

       public string AreaId { get; set; }

       public decimal SalePrice { get; set; }
    }
}
