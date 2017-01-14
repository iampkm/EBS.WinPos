using EBS.WinPos.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBS.WinPos.Service.Dto
{
    public class ProductPriceDto : Product
    {
        /// <summary>
        /// 区域价
        /// </summary>
        public decimal AreaSalePrice { get; set; }
        /// <summary>
        /// 门店自定义售价
        /// </summary>
        public decimal StoreSalePrice { get; set; }

        /// <summary>
        /// 会员价
        /// </summary>
        public decimal VipSalePrice { get; set; }
    }
}
