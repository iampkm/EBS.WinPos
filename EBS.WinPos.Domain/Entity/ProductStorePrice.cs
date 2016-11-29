using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBS.WinPos.Domain.Entity
{
    /// <summary>
    /// 商品门店价
    /// </summary>
    public class ProductStorePrice : BaseEntity
    {
        public int ProductId { get; set; }

        public int StoreId { get; set; }

        public decimal SalePrice { get; set; }
    }
}
