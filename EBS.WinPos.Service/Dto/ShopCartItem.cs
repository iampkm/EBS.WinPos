using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EBS.WinPos.Domain.Entity;
namespace EBS.WinPos.Service.Dto
{
    public class ShopCartItem
    {
        public ShopCartItem(Product product, int quantity, decimal realPrice)
        {
            this.Product = product;
            this.Quantity = quantity;
            this.RealPrice = realPrice;
            this.ProductCode = product.Code;
            this.ProductName = product.Name;
            this.BarCode = product.BarCode;
            this.Specification = product.Specification;
            this.Unit = product.Unit;
            this.SalePrice = product.SalePrice;
            this.ProductId = product.Id;
        }
        public int ProductId { get; private set; }
        /// <summary>
        /// SKU编码
        /// </summary>
        public string ProductCode { get; private set; }
        /// <summary>
        /// 条码
        /// </summary>
        public string BarCode { get; private set; }
        public string ProductName { get; private set; }

        public string Specification { get; private set; }

        public string Unit { get; private set; }
        /// <summary>
        /// 销售价
        /// </summary>
        public decimal SalePrice { get; private set; }
        /// <summary>
        /// 优惠价
        /// </summary>
        public decimal RealPrice { get; private set; }

        public DateTime UpdatedOn { get; private set; }

        public int Quantity { get; set; }
        /// <summary>
        /// 优惠金额
        /// </summary>
        public decimal DiscountAmount
        {
            get
            {
                return SalePrice - RealPrice;
            }
        }
        /// <summary>
        /// 小计金额
        /// </summary>
        public decimal Amount
        {
            get
            {
                return RealPrice * Quantity;
            }
        }

        public Product Product { get; private set; }

        public void ChangeRealPrice(decimal realPrice)
        {
            this.RealPrice = realPrice;
        }

    }
}
