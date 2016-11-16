using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EBS.WinPos.Domain.ValueObject;
namespace EBS.WinPos.Domain.Entity
{
    public class SaleOrder
    {
        public SaleOrder()
        {
            this.Items = new List<SaleOrderItem>();
            this.CreatedOn = DateTime.Now;
            this.UpdatedOn = DateTime.Now;
        }
        public int Id { get; set; }

        public int StoreId { get; set; }

        public SaleOrderStatus Status { get; set; }

        public SaleOrderPaidStatus PaidStatus { get; set; }

        public string Code { get; set; }

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
                    BarCode = product.BarCode,
                    SaleOrderId = this.Id,
                    Specification = product.Specification

                };

            }
        }

        public void GenerateNewCode()
        {
            string billType = "15";
            string createdBy = this.CreatedBy.ToString();
            var code = Math.Abs(Guid.NewGuid().GetHashCode());
            var hashcode = code.ToString();
            var hashcodeLength = 15 - billType.Length - createdBy.Length;
            StringBuilder sb = new StringBuilder();
            sb.Append(billType);
            sb.Append(createdBy);
            var lastNumber = hashcode.Length > hashcodeLength ? hashcode.Substring(0, hashcodeLength) : hashcode.PadLeft(hashcodeLength, '0');
            sb.Append(lastNumber);
            this.Code = sb.ToString();
            //ts.TotalSeconds;
            //var date = this.CreatedOn;
            //var ts = date - DateTime.Parse("2016-01-01");
            //var seconds = Math.Truncate(ts.TotalSeconds).ToString().PadLeft(6, '0');  // 5位
            //return string.Format("{0}{1}{2}{3}", (int)BillIdentity.SaleOrder, createdBy, orderYear.ToString().PadLeft(3, '0'), seconds);
        }
    }


}
