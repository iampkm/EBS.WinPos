using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EBS.WinPos.Domain.Entity;
using EBS.WinPos.Domain.ValueObject;
namespace EBS.WinPos.Service.Dto
{
    public class ShopCart
    {
        public ShopCart(int storeId,int posId, int editor, int orderType = 1)
        {
            Items = new List<ShopCartItem>();
            this.StoreId = storeId;
            this.Editor = editor;
            this.OrderType = orderType;
            this.PosId = posId;
            this.OrderLevel =  SaleOrderLevel.General; //普通订单

        }
        /// <summary>
        /// 门店ID
        /// </summary>
        public int StoreId { get; set; }
        /// <summary>
        /// Pos 机ID
        /// </summary>
        public int PosId { get; set; }

        public int Editor { get; set; }
        /// <summary>
        /// 1  销售单，2 销售退单
        /// </summary>
        public int OrderType { get; set; }
        /// <summary>
        /// 退款账户
        /// </summary>
        public string RefundAccount { get; set; }
        /// <summary>
        /// 订单等级
        /// </summary>
        public SaleOrderLevel OrderLevel { get; set; }

        public List<ShopCartItem> Items { get; set; }

        public string OrderCode { get; set; }

        public int OrderId { get; set; }
        /// <summary>
        /// 班次代码
        /// </summary>
        public string WorkScheduleCode { get; set; }

        /// <summary>
        /// 现金支付金额
        /// </summary>
        public decimal PayAmount { get; set; }
        /// <summary>
        /// /在线支付金额
        /// </summary>
        public decimal OnlinePayAmount { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal OrderAmount
        {
            get
            {
                return this.Items.Sum(n => n.RealPrice * n.Quantity);
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
            get
            {
                return this.Items.Sum(n => n.SalePrice - n.RealPrice);
            }
        }

        public decimal ChargeAmount
        {
            get
            {
                return this.PayAmount + this.OnlinePayAmount - OrderAmount;
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
            else
            {
                if (lastItem.ProductId == item.ProductId)
                {
                    lastItem.Quantity += 1;
                }
                else
                {
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
