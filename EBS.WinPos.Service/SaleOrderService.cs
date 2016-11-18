using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using EBS.WinPos.Domain.Entity;
using EBS.WinPos.Domain;
using System.Diagnostics;
using System.Data.SQLite;
using EBS.WinPos.Service.Dto;
using EBS.WinPos.Domain.ValueObject;
using EBS.Infrastructure.Helper;
namespace EBS.WinPos.Service
{
    public class SaleOrderService
    {
        Repository _db;
        public SaleOrderService()
        {
            _db = new Repository();
        }
        public OrderInfo CreateOrder(ShopCart cat)
        {
            SaleOrder order = new SaleOrder()
            {
                StoreId = cat.StoreId,
                CreatedBy = cat.Editor,
                UpdatedBy = cat.Editor,
            };
            order.GenerateNewCode();
            foreach (ShopCartItem item in cat.Items)
            {
                order.AddOrderItem(item.Product, item.Quantity);
            }
            this._db.Orders.Add(order);
            this._db.SaveChanges();
            var orderInfo = new OrderInfo()
            {
                OrderId = order.Id,
                OrderCode = order.Code,
                OrderAmount = order.Items.Sum(n => n.SalePrice * n.Quantity)
            };
            return orderInfo;
        }

        public void CashPay(int orderId, decimal payAmount)
        {
            var model = _db.Orders.FirstOrDefault(n => n.Id == orderId);
            if (model == null) { throw new Exception("订单不存在"); }
            if (payAmount < model.GetOrderAmount())
            {
                throw new Exception("支付金额低于订单金额");
            }          
           
            model.FinishPaid();
            //保存交易记录
            PaidHistory history = new PaidHistory(model,payAmount,PaymentWay.Cash);
            _db.PaidHistorys.Add(history);
            _db.SaveChanges();

            //触发订单支付完成事件，推送到服务器
           
        }

        public void WechatPay(int orderId, string payBarCode)
        {
            if (string.IsNullOrEmpty(payBarCode)) { throw new Exception("请录入微信付款码"); }
            var model = _db.Orders.FirstOrDefault(n => n.Id == orderId);
            if (model == null) { throw new Exception("订单不存在"); }
            if (model.GetOrderAmount() <= 0) { throw new Exception("订单金额不能为0"); }
            // 发起微信支付
            string data = JsonHelper.GetJson(new { barcode = payBarCode, orderId = model.Code, paymentAmt = model.GetOrderAmount().ToString("F2"), systemId = "2" });
            string sign = EncryptHelpler.SignEncrypt(data, Config.SignKey_WeChatBarcode);
            string result = HttpHelper.HttpRequest("POST", Config.Api_Pay_WeChatBarcode, param: string.Format("sign={0}&data={1}", sign, data), timeOut: 90000);
            if (result == "1")
            {
                //支付成功
                model.FinishPaid();
                PaidHistory history = new PaidHistory(model, model.GetOrderAmount(), PaymentWay.Cash);
                _db.PaidHistorys.Add(history);
                _db.SaveChanges();
            }
            else {
                throw new Exception("支付失败，请稍后重试");
            }
          

        }

        public void AliPay(int orderId, string payBarCode)
        { 
            
        }
    }
}
