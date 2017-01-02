using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBS.WinPos.Domain.Entity
{
   public class SaleSync:BaseEntity
    {
        public SaleSync()
        {
            this.SaleDate = DateTime.Now.ToString("yyyy-MM-dd");
            this.ClientUpdatedOn = DateTime.Now;
        }
        /// <summary>
        /// yyyy-MM-dd
        /// </summary>
        public string SaleDate { get; set; }
        public int StoreId { get; set; }
        public int PosId { get; set; }
        /// <summary>
        /// 订单笔数
        /// </summary>
        public int OrderCount { get; set; }
        /// <summary>
        /// 销售总金额
        /// </summary>
        public decimal OrderTotalAmount { get; set; }
        /// <summary>
        /// 客户端更新时间
        /// </summary>
        public DateTime ClientUpdatedOn { get; set; }
    }
}
