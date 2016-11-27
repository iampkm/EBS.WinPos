using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBS.WinPos.Domain.Entity
{
    public class Product : BaseEntity
    {
        /// <summary>
        /// SKU编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 条码
        /// </summary>
        public string BarCode { get; set; }
        public string Name { get; set; }

        public string Specification { get; set; }

        public string Unit { get; set; }
        public decimal SalePrice { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
