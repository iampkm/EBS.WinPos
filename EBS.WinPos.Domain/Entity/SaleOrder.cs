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
        }
        public string Code { get; set; }

        public int StoreId { get; set; }

        public SaleOrderStatus Status { get; set; }

        public SaleOrderPaidStatus PaidStatus { get; set; }
        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public virtual List<SaleOrderItem> Items { get; set; }


        public void AddOrderItem(Product product, int quantity)
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
                    SaleOrderId = this.Id
                };
            }
            this.Items.Add(item);
        }

        public void GenerateNewCode()
        {
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

        public void FinishPaid()
        {
            if (this.Status != SaleOrderStatus.Create) { throw new Exception("订单非待支付状态"); }
            this.Status = SaleOrderStatus.Paid;
            this.UpdatedOn = DateTime.Now;
        }

        public decimal GetOrderAmount()
        {
            return this.Items.Sum(n => n.SalePrice * n.Quantity);
        }
    }


}
