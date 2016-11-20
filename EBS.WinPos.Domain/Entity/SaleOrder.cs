using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EBS.WinPos.Domain.ValueObject;
namespace EBS.WinPos.Domain.Entity
{
    public class SaleOrder:BaseEntity
    {
        public SaleOrder()
        {
            this.Items = new List<SaleOrderItem>();
            this.CreatedOn = DateTime.Now;
            this.UpdatedOn = DateTime.Now;
            this.Status = SaleOrderStatus.Create;
            this.Discount = 1m;
        }
        public string Code { get; set; }

        public int StoreId { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>

        public PaymentWay PaymentWay { get; set; }
        /// <summary>
        /// 支付日期
        /// </summary>
        public DateTime PaidDate { get; set; }
        /// <summary>
        /// 订单金额 = 实际价格RealAmount * 数量
        /// </summary>
        public decimal OrderAmount { get; private set; }
        /// <summary>
        /// 实际销售金额 = 订单金额 * 折扣
        /// </summary>
        public decimal RealAmount { get; private set; }
        /// <summary>
        /// 订单折扣(整单折扣,默认为1,即 不打折)
        /// </summary>
        public decimal Discount { get; set; }
        /// <summary>
        /// 客户支付金额
        /// </summary>
        public decimal PayAmount { get; set; }
        /// <summary>
        /// 销售单状态
        /// </summary>

        public SaleOrderStatus Status { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public virtual List<SaleOrderItem> Items { get; set; }


        public void AddOrderItem(Product product, int quantity,decimal discount)
        {
            var item = this.Items.Where(n => n.ProductId == product.Id).FirstOrDefault();
            if (item != null)
            {
                item.Quantity += quantity;
            }
            else {
                item = new SaleOrderItem()
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    SalePrice = product.SalePrice,
                    Quantity = quantity,
                    ProductCode = product.Code,
                    SaleOrderId = this.Id,
                    Discount = discount,
                    RealPrice = product.SalePrice * discount
                };
            }
            this.Items.Add(item);
            // 计算订单总金额
            this.OrderAmount += item.SalePrice * quantity;
            //计算订单折后金额（实收金额）
            this.RealAmount += item.RealPrice * quantity;
        }

        public void GenerateNewCode()
        {

            // 0161120 86400 001 15
            string billType = "15";
            string createdBy = this.CreatedBy.ToString();
            var code = Math.Abs(Guid.NewGuid().GetHashCode());
            var hashcode = code.ToString();
            StringBuilder sb = new StringBuilder();
            sb.Append(billType);
            sb.Append(createdBy);
            // hascode  取 8位，超过3位自增,2位类型订单
            // 订单长度
            var orderCodeLength = 13;
            if (createdBy.Length > 3)
            {
                hashcode = hashcode.Substring(0, 8);  // hashCode 固定取8位
                sb.Append(hashcode);
            }
            else {
                var hashcodeLength = orderCodeLength - billType.Length - createdBy.Length;
                var lastNumber = hashcode.Length > hashcodeLength ? hashcode.Substring(0, hashcodeLength) : hashcode.PadLeft(hashcodeLength, '0');
                sb.Append(lastNumber);
            }
            this.Code = sb.ToString();

            //ts.TotalSeconds;
            //var date = this.CreatedOn;
            //var ts = date - DateTime.Parse("2016-01-01");
            //var seconds = Math.Truncate(ts.TotalSeconds).ToString().PadLeft(6, '0');  // 5位
            //return string.Format("{0}{1}{2}{3}", (int)BillIdentity.SaleOrder, createdBy, orderYear.ToString().PadLeft(3, '0'), seconds);
        }

        public void Cancel(int editor)
        {
            if(this.Status!= SaleOrderStatus.Create){
              throw new Exception("新建订单才能作废");
            }
            this.Status = SaleOrderStatus.Create;
            this.UpdatedBy = editor;
            this.UpdatedOn = DateTime.Now;
        }

        public void FinishPaid(PaymentWay payWay = PaymentWay.Cash)
        {
            if (this.Status != SaleOrderStatus.Create) { throw new Exception("订单非待支付状态"); }
            this.Status = SaleOrderStatus.Paid;
            this.UpdatedOn = DateTime.Now;
            this.PaidDate = DateTime.Now;
            this.PaymentWay = payWay;

        }       
    }


}
