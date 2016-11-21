using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EBS.WinPos.Domain.Entity;
namespace EBS.WinPos.Service.Dto
{   
    public class ShopCartItem
    {
        public ShopCartItem(Product product, int quantity, decimal discount = 1m)
        {
            this.Product = product;
            this.Quantity = quantity;
            this.Discount = discount;

        }
        public Product Product { get; set; }

        public int Quantity { get; set; }

        public decimal Discount { get; set; }
    }
}
