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
using EBS.Infrastructure.Extension;
using EBS.Infrastructure;
using EBS.WinPos.Service.Task;
namespace EBS.WinPos.Service
{
    public class SaleOrderService
    {
        Repository _db;
        PrinterService _printService;
        SyncService _syncService;
        public SaleOrderService()
        {
            _db = new Repository();
            _printService = new PrinterService();
            _syncService = new SyncService(AppContext.Log);
        }
        public OrderInfo CreateOrder(ShopCart cat)
        {
            SaleOrder order = new SaleOrder()
            {
                StoreId = cat.StoreId,
                CreatedBy = cat.Editor,
                UpdatedBy = cat.Editor
            };
            order.GenerateNewCode();
            foreach (ShopCartItem item in cat.Items)
            {
                order.AddOrderItem(item.Product, item.Quantity, item.RealPrice);
            }
            this._db.Orders.Add(order);
            this._db.SaveChanges();
            var orderInfo = new OrderInfo()
            {
                OrderId = order.Id,
                OrderCode = order.Code,
                OrderAmount = order.OrderAmount
            };
            return orderInfo;
        }

        public void CancelOrder(int orderId, int editor)
        {
            var model = _db.Orders.FirstOrDefault(n => n.Id == orderId);
            if (model == null) { throw new AppException("订单不存在"); }
            model.Cancel(editor);
            _db.SaveChanges();
            //同步到服务器
            _syncService.Send(model);
        }

        public void CashPay(int orderId, decimal payAmount)
        {
            var model = _db.Orders.FirstOrDefault(n => n.Id == orderId);
            if (model == null) { throw new AppException("订单不存在"); }
            if (payAmount < model.OrderAmount)
            {
                throw new AppException("支付金额低于订单金额");
            }

            model.FinishPaid(payAmount);
            //保存交易记录
            _db.SaveChanges();

            //同步到服务器
            _syncService.Send(model);
        }

        public void WechatPay(int orderId, string payBarCode)
        {
            if (string.IsNullOrEmpty(payBarCode)) { throw new AppException("请录入微信付款码"); }
            var model = _db.Orders.FirstOrDefault(n => n.Id == orderId);
            if (model == null) { throw new AppException("订单不存在"); }
            if (model.OrderAmount <= 0) { throw new AppException("订单金额不能为0"); }
            // 发起微信支付
            string data = JsonHelper.GetJson(new { barcode = payBarCode, orderId = model.Code, paymentAmt = model.OrderAmount.ToString("F2"), systemId = "2" });
            string sign = EncryptHelpler.SignEncrypt(data, Config.SignKey_WeChatBarcode);
            string result = HttpHelper.HttpRequest("POST", Config.Api_Pay_WeChatBarcode, param: string.Format("sign={0}&data={1}", sign, data), timeOut: 90000);
            if (result == "1")
            {
                //支付成功
                model.FinishPaid(model.OrderAmount, PaymentWay.WechatPay);
                _db.SaveChanges();
                //同步到服务器
                _syncService.Send(model);
            }
            else {
                throw new AppException("支付失败，请稍后重试");
            }


        }

        public void AliPay(int orderId, string payBarCode)
        {

        }

        public void PrintBill(int orderId)
        {
            var model = _db.Orders.FirstOrDefault(n => n.Id == orderId);
            var store = _db.Stores.FirstOrDefault(n => n.Id == model.StoreId);
            //打印模板
            string posTemplate = FileHelper.ReadText("PosBillTemplate.txt");
            if (string.IsNullOrEmpty(posTemplate)) { throw new AppException("小票模板为空"); }
            var lineLocation =posTemplate.LastIndexOf("##item##");
            //分离商品item模板
            string billTemplate = posTemplate.ToLower().Substring(0, posTemplate.Length - lineLocation);          
            string itemTemplate = posTemplate.Substring(lineLocation);
            //开始替换
            billTemplate = billTemplate.Replace("{{storename}}", store.Name);
            billTemplate = billTemplate.Replace("{{createdate}}", model.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss"));
            billTemplate = billTemplate.Replace("{{ordercode}}", model.Code);
            billTemplate = billTemplate.Replace("{{createdby}}", model.CreatedBy.ToString());
            //明细
            string productItems = "";
            foreach (var item in model.Items)
            {
                string tempItem = itemTemplate;
                tempItem = tempItem.Replace("{{productname}}", item.ProductName);
                tempItem = tempItem.Replace("{{productcode}}", item.ProductCode);
                tempItem = tempItem.Replace("{{price}}", item.RealPrice.ToString());
                tempItem = tempItem.Replace("{{quantity}}", item.Quantity.ToString());
                decimal amount = item.RealPrice * item.Quantity;
                tempItem = tempItem.Replace("{{amount}}", amount.ToString());
                productItems += tempItem; ;
            }
            billTemplate = billTemplate.Replace("{{item}}", productItems);
            //应收应付
            billTemplate = billTemplate.Replace("{{orderamount}}", model.OrderAmount.ToString("C"));
            billTemplate = billTemplate.Replace("{{quantitytotal}}", model.GetQuantityTotal().ToString());
            billTemplate = billTemplate.Replace("{{discountamount}}", model.GetTotalDiscountAmount().ToString("C"));
            billTemplate = billTemplate.Replace("{{payamount}}", model.PayAmount.ToString("C"));
            billTemplate = billTemplate.Replace("{{chargeamount}}", model.GetChargeAmount().ToString("C") );
            billTemplate = billTemplate.Replace("{{paymentway}}", model.PaymentWay.Description());
            billTemplate = billTemplate.Replace("{{onlinepayamount}}", model.OnlinePayAmount.ToString("C"));
            
            _printService.PrintLine(billTemplate);
        }
    }
}
