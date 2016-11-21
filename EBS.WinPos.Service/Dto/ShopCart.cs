using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBS.WinPos.Service.Dto
{
    public class ShopCart
    {
        public ShopCart()
        {
            Items = new List<ShopCartItem>();
        }
        public int StoreId { get; set; }

        public int Editor { get; set; }

        public List<ShopCartItem> Items { get; set; }

        public int OrderId { get; set; }

        /// <summary>
        /// 客户支付金额
        /// </summary>
        public decimal PayAmount { get; set; }
        public decimal OrderAmount
        {
            get
            {
                return this.Items.Sum(n => n.Product.SalePrice * n.Discount * n.Quantity);
            }
        }

        public int QuantityTotal
        {
            get
            {
                return this.Items.Sum(n => n.Quantity);
            }
        }

        public decimal ChargeAmount
        {
            get
            {
                return this.PayAmount - OrderAmount;
            }
        }

        public bool CheckCanPay()
        {
            return !this.Items.Exists(n => n.Quantity == 0);
        }

    }
}
