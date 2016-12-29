using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EBS.WinPos.Domain.ValueObject;
using EBS.Infrastructure;
namespace EBS.WinPos.Domain.Entity
{
    public class SaleOrder : BaseEntity
    {
        public SaleOrder()
        {
            this.Items = new List<SaleOrderItem>();
            this.CreatedOn = DateTime.Now;
            this.UpdatedOn = DateTime.Now;
            this.Status = SaleOrderStatus.Create;
            this.OrderType = 1;
        }
        public string Code { get; set; }

        public int StoreId { get; set; }
        /// <summary>
        /// Pos 机ID
        /// </summary>
        public int PosId { get; set; }
        /// <summary>
        /// 订单类型：销售单1，销售退单2
        /// </summary>
        public int OrderType { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public PaymentWay PaymentWay { get; set; }
        /// <summary>
        /// 退款账户
        /// </summary>
        public string RefundAccount { get; set; }
        /// <summary>
        /// 支付日期
        /// </summary>
        public DateTime? PaidDate { get; set; }
        /// <summary>
        /// 订单金额 = 实际价格RealAmount * 数量
        /// </summary>
        public decimal OrderAmount { get; private set; }
        /// <summary>
        /// 现金支付金额
        /// </summary>
        public decimal PayAmount { get; set; }
        /// <summary>
        /// 刷卡支付，微信支付，阿里支付等在线支付金额
        /// </summary>
        public decimal OnlinePayAmount { get; set; }

        /// <summary>
        /// 销售单状态
        /// </summary>
        public SaleOrderStatus Status { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }
        /// <summary>
        /// 班次代码
        /// </summary>
        public string WorkScheduleCode { get; set; }

        public int IsSync { get; set; }

        public virtual List<SaleOrderItem> Items { get; set; }


        public void AddOrderItem(Product product, int quantity, decimal realPrice)
        {
            var item = this.Items.Where(n => n.ProductId == product.Id).FirstOrDefault();
            if (item != null)
            {
                item.Quantity += quantity;
            }
            else
            {
                item = new SaleOrderItem()
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    SalePrice = product.SalePrice,
                    Quantity = quantity,
                    ProductCode = product.Code,
                    SaleOrderId = this.Id,
                    RealPrice = realPrice,
                };
            }
            this.Items.Add(item);
            // 计算订单总金额
            this.OrderAmount += item.RealPrice * quantity;
        }

        public void GenerateNewCode()
        {
             //账号ID + 8 为日期+ 5 时间秒+2位随机数
            // 1+2014010100001
            var orderCodeMinLength = 17;
            string createdBy = this.CreatedBy.ToString();
            string orderType = this.OrderType.ToString();
            var code = Math.Abs(Guid.NewGuid().GetHashCode());
            var hashcode = code.ToString();
            StringBuilder sb = new StringBuilder();
            var date = this.CreatedOn;
            var ts = date - date.Date;
            var seconds = Math.Truncate(ts.TotalSeconds).ToString().PadLeft(5, '0');  // 5位
            // 账号1~N+日期8+时间数字5 
            sb.Append(orderType); //销售单据类型
            sb.Append(createdBy);
            sb.Append(date.ToString("yyyyMMdd"));
            sb.Append(seconds);
            code = Math.Abs(Guid.NewGuid().GetHashCode());
            hashcode = code.ToString().Substring(0, orderCodeMinLength - sb.Length);
            sb.Append(hashcode);
            this.Code = sb.ToString();
        }



        public void Cancel(int editor)
        {
            if (this.Status != SaleOrderStatus.Create)
            {
                throw new AppException("新建订单才能作废");
            }
            this.Status = SaleOrderStatus.Cancel;
            this.UpdatedBy = editor;
            this.UpdatedOn = DateTime.Now;
        }
        /// <summary>
        /// 完成支付
        /// </summary>
        /// <param name="payAmount"></param>
        /// <param name="onlinePayAmount"></param>
        /// <param name="payWay"></param>
        public void FinishPaid(decimal payAmount,decimal onlinePayAmount = 0m ,PaymentWay payWay = PaymentWay.Cash)
        {
            if (this.Status != SaleOrderStatus.Create) { throw new AppException("订单非待支付状态"); }
            this.Status = SaleOrderStatus.Paid;
            this.UpdatedOn = DateTime.Now;
            this.PaidDate = DateTime.Now;
            this.PaymentWay = payWay;
            this.PayAmount = payAmount;
            this.OnlinePayAmount = onlinePayAmount;

        }
        /// <summary>
        /// 待退款
        /// </summary>
        /// <param name="payAmount"></param>
        /// <param name="onlinePayAmount"></param>
        /// <param name="payWay"></param>
        public void WaitRefund(decimal payAmount, decimal onlinePayAmount = 0m, PaymentWay payWay = PaymentWay.Cash)
        {
            if (this.Status != SaleOrderStatus.Create) { throw new AppException("订单非待支付状态"); }
            this.Status = SaleOrderStatus.WaitPaid;
            this.UpdatedOn = DateTime.Now;
            this.PaymentWay = payWay;
            this.PayAmount = payAmount;
            this.OnlinePayAmount = onlinePayAmount;
        }

        public decimal GetChargeAmount()
        {          
            return this.PayAmount + this.OnlinePayAmount - this.OrderAmount;
        }
        /// <summary>
        /// 总优惠金额
        /// </summary>
        /// <returns></returns>
        public decimal GetTotalDiscountAmount()
        {
            return this.Items.Sum(n => n.SalePrice - n.RealPrice);
        }

        public int GetQuantityTotal()
        {
            return this.Items.Sum(n => n.Quantity);
        }
    }


}
