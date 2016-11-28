using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EBS.WinPos.Domain.Entity;
namespace EBS.WinPos.Service.Dto
{
    public class ShopCart
    {
        public ShopCart(int storeId,int editor)
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
                return this.Items.Sum(n => n.RealPrice  * n.Quantity);
            }
        }

        public int TotalQuantity
        {
            get
            {
                return this.Items.Sum(n => n.Quantity);
            }
        }

        public decimal TotalDiscountAmount
        {
            get {
                return this.Items.Sum(n => n.SalePrice - n.RealPrice); 
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

        public void AddShopCart(ShopCartItem item)
        {
            var lastItem = this.Items.LastOrDefault();
            if (lastItem == null)
            {
                this.Items.Add(item);
            }
            else {
                if (lastItem.ProductId == item.ProductId)
                {
                    lastItem.Quantity += 1;
                }
                else {
                    this.Items.Add(item);
                }
            }           
        }

        public void ChangeQuantity(int productId, int quantity)
        {
            var item = this.Items.FirstOrDefault(n => n.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity;
            }           
        }

    }
}
