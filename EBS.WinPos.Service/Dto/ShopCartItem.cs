using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EBS.WinPos.Domain.Entity;
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

       public  List<ShopCartItem> Items { get; set; }

        /// <summary>
        /// 客户支付金额
        /// </summary>
       public decimal PayAmount { get; set; }
       public decimal OrderAmount { get {
           return this.Items.Sum(n => n.Product.SalePrice * n.Quantity);
       } }

       public int QuantityTotal {
           get {
               return this.Items.Sum(n => n.Quantity);
           }
       }

       public decimal ChargeAmount {
           get {
               return this.PayAmount - OrderAmount;
           }
       }

    }
   public class ShopCartItem
    {
       public ShopCartItem(Product product,int quantity)
       {
           this.Product = product;
           this.Quantity = quantity;
       }
       public Product Product { get; set; }

       public int Quantity { get; set; }
    }
}
