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
using EBS.Infrastructure.Log;
namespace EBS.WinPos.Service
{
    public class SaleOrderService
    {
        Repository _db;
        IPosPrinter _printService;
        SyncService _syncService;
        ILogger _log;
        DapperContext _dbContext;
        public SaleOrderService()
        {
            _log = AppContext.Log;
            _db = new Repository();
            _printService = new DriverPrinterService();
            _syncService = new SyncService(AppContext.Log);
            _dbContext = new DapperContext();
        }
        public void CreateOrder(ShopCart cat)
        {
            SaleOrder order = new SaleOrder()
            {
                StoreId = cat.StoreId,
                PosId = cat.PosId,
                CreatedBy = cat.Editor,
                UpdatedBy = cat.Editor,
                WorkScheduleCode = cat.WorkScheduleCode,
            };
            order.GenerateNewCode();
            foreach (ShopCartItem item in cat.Items)
            {
                order.AddOrderItem(item.Product, item.Quantity, item.RealPrice);
            }
            this._db.Orders.Add(order);
            this._db.SaveChanges();
            //设置订单信息
            cat.OrderId = order.Id;
            cat.OrderCode = order.Code;
        }

        public void CreateSaleRefund(ShopCart cat)
        {
            SaleOrder order = new SaleOrder()
            {
                StoreId = cat.StoreId,
                PosId = cat.PosId,
                CreatedBy = cat.Editor,
                UpdatedBy = cat.Editor,
                OrderType = 2,
                RefundAccount = cat.RefundAccount,
                WorkScheduleCode = cat.WorkScheduleCode,
            };
            order.GenerateNewCode();
            foreach (ShopCartItem item in cat.Items)
            {
                order.AddOrderItem(item.Product, item.Quantity, item.RealPrice);
            }
            this._db.Orders.Add(order);
            this._db.SaveChanges();
            //设置订单信息
            cat.OrderId = order.Id;
            cat.OrderCode = order.Code;
        }


        public void CancelOrder(int orderId, int editor)
        {
            var model = _db.Orders.Include(n => n.Items).FirstOrDefault(n => n.Id == orderId);
            if (model == null) { throw new AppException("订单不存在"); }

            model.Cancel(editor);
            _db.SaveChanges();
            //打印小票
            PrintOrderTicket(model);
            //同步到服务器
            _syncService.Send(model);
        }

        public void CashPay(int orderId, decimal payAmount)
        {
          var model = _db.Orders.Include(n=>n.Items).FirstOrDefault(n => n.Id == orderId);
            if (model == null) { throw new AppException("订单不存在"); }
            if (payAmount < model.OrderAmount)
            {
                throw new AppException("支付金额低于订单金额");
            }
            if (payAmount > model.OrderAmount + 100) {
                throw new AppException("收现金额录入过大");
            }

            model.FinishPaid(payAmount);
            //保存交易记录
            _db.SaveChanges();
            //打印小票
            PrintOrderTicket(model);
            //同步到服务器
            _syncService.Send(model);
        }

        public void CashRefund(int orderId, string licenseCode, decimal payAmount)
        {
            if (string.IsNullOrEmpty(licenseCode)) { throw new AppException("请输入店长授权码"); }
            var model = _db.Orders.Include(n => n.Items).FirstOrDefault(n => n.Id == orderId);
            if (model == null) { throw new AppException("订单不存在"); }
            var store = _db.Stores.FirstOrDefault(n => n.Id == model.StoreId);
            if (!store.VerifyLicenseCode(licenseCode)) { throw new AppException("店长授权码错误"); }
            model.PayAmount = payAmount;
            if (Math.Abs(model.PayAmount) > Math.Abs(model.OrderAmount))
            {
                throw new AppException("退款金额不能超过订单金额");
            }

            model.FinishPaid(payAmount);
            //保存交易记录
            _db.SaveChanges();
            //打印小票
            PrintOrderTicket(model);
            //同步到服务器
            _syncService.Send(model);
        }

        public void WechatPay(int orderId, string payBarCode, decimal payAmount)
        {
            if (string.IsNullOrEmpty(payBarCode)) { throw new AppException("请录入或扫微信付款条码"); }
            var model = _db.Orders.Include(n => n.Items).FirstOrDefault(n => n.Id == orderId);
            if (model == null) { throw new AppException("订单不存在"); }
            if (model.OrderAmount <= 0) { throw new AppException("订单金额不能为0"); }
            if (model.PayAmount > model.OrderAmount) { throw new AppException("请使用现金支付"); }
            // 发起微信支付
            model.OnlinePayAmount = model.OrderAmount - model.PayAmount;
            string data = JsonHelper.GetJson(new { barcode = payBarCode, orderId = model.Code, paymentAmt = model.OnlinePayAmount.ToString("F2"), systemId = "2" });
            string sign = EncryptHelpler.SignEncrypt(data, Config.SignKey_WeChatBarcode);
            string result = HttpHelper.HttpRequest("POST", Config.Api_Pay_WeChatBarcode, param: string.Format("sign={0}&data={1}", sign, data), timeOut: 90000);
            if (result == "1")
            {
                //支付成功
                model.FinishPaid(model.PayAmount, model.OnlinePayAmount, PaymentWay.WechatPay);
                _db.SaveChanges();
                //打印小票
                PrintOrderTicket(model);
                //同步到服务器
                _syncService.Send(model);
            }
            else
            {
                throw new AppException("支付失败！请检查网络是否正常，稍后重试。");
            }
        }

        public void WechatRefund(int orderId, string licenseCode, decimal payAmount, string refundAccount)
        {
            if (string.IsNullOrEmpty(licenseCode)) { throw new AppException("请输入店长授权码"); }
            var model = _db.Orders.Include(n => n.Items).FirstOrDefault(n => n.Id == orderId);
            if (model == null) { throw new AppException("订单不存在"); }
            if (string.IsNullOrEmpty(refundAccount)) { throw new AppException("请输入支付宝退款账户"); }
            model.RefundAccount = refundAccount;
            var store = _db.Stores.FirstOrDefault(n => n.Id == model.StoreId);
            if (!store.VerifyLicenseCode(licenseCode)) { throw new AppException("店长授权码错误"); }
            if (model.OrderAmount >= 0) { throw new AppException("订单金额不能大于0"); }
            model.OnlinePayAmount = model.OrderAmount - model.PayAmount;
            if (Math.Abs(model.PayAmount) + Math.Abs(model.OnlinePayAmount) > Math.Abs(model.OrderAmount)) { throw new AppException("退款现金不能超过应退金额"); }
            // 发起微信支付
            model.WaitRefund(model.PayAmount, model.OnlinePayAmount, PaymentWay.WechatPay);
            _db.SaveChanges();
            //打印小票
            PrintOrderTicket(model);
            //同步到服务器
            _syncService.Send(model);
        }

        public void AliPay(int orderId, string payBarCode, decimal payAmount)
        {
            if (string.IsNullOrEmpty(payBarCode)) { throw new AppException("请录入或扫支付宝付款条码"); }
            var model = _db.Orders.Include(n => n.Items).FirstOrDefault(n => n.Id == orderId);
            if (model == null) { throw new AppException("订单不存在"); }
            if (model.OrderAmount <= 0) { throw new AppException("订单金额不能为0"); }
            if (Math.Abs(model.PayAmount) > Math.Abs(model.OrderAmount)) { throw new AppException("请使用现金支付"); }
            // 发起微信支付
            model.OnlinePayAmount = model.OrderAmount - model.PayAmount;
            string data = JsonHelper.GetJson(new { barcode = payBarCode, orderId = model.Code, paymentAmt = model.OnlinePayAmount.ToString("F2"), systemId = "2" });
            string sign = EncryptHelpler.SignEncrypt(data, Config.SignKey_AliBarcode);
            string result = HttpHelper.HttpRequest("POST", Config.Api_Pay_AliBarcode, param: string.Format("sign={0}&data={1}", sign, data), timeOut: 90000);
            if (result == "1")
            {
                //支付成功
                model.FinishPaid(model.PayAmount, model.OnlinePayAmount, PaymentWay.WechatPay);
                _db.SaveChanges();
                //打印小票
                PrintOrderTicket(model);
                //同步到服务器
                _syncService.Send(model);
            }
            else
            {
                throw new AppException("支付失败！请检查网络是否正常，稍后重试。");
            }
        }

        public void AliRefund(int orderId, string licenseCode, decimal payAmount, string refundAccount)
        {
            if (string.IsNullOrEmpty(licenseCode)) { throw new AppException("请输入店长授权码"); }
            var model = _db.Orders.Include(n => n.Items).FirstOrDefault(n => n.Id == orderId);
            if (model == null) { throw new AppException("订单不存在"); }
            if (string.IsNullOrEmpty(refundAccount)) { throw new AppException("请输入支付宝退款账户"); }
            model.RefundAccount = refundAccount;
            var store = _db.Stores.FirstOrDefault(n => n.Id == model.StoreId);
            if (!store.VerifyLicenseCode(licenseCode)) { throw new AppException("店长授权码错误"); }
            if (model.OrderAmount >= 0) { throw new AppException("订单金额不能大于0"); }
            model.OnlinePayAmount = model.OrderAmount - model.PayAmount;
            if (Math.Abs(model.PayAmount) + Math.Abs(model.OnlinePayAmount) > Math.Abs(model.OrderAmount)) { throw new AppException("退款现金不能超过应退金额"); }
            model.WaitRefund(model.PayAmount, model.OnlinePayAmount, PaymentWay.AliPay);
            _db.SaveChanges();
            //打印小票
            PrintOrderTicket(model);
            //同步到服务器
            _syncService.Send(model);
        }

        public void PrintTicket(int orderId)
        {
            try
            {
                var model = _db.Orders.Include(n => n.Items).FirstOrDefault(n => n.Id == orderId);
                PrintTicket(model);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "小票打印失败");
            }

        }

        private void PrintOrderTicket(SaleOrder model)
        {
            try
            {               
                PrintTicket(model);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "小票打印失败");
            }
        }


        public void PrintTicket(SaleOrder model)
        {
            var store = _db.Stores.FirstOrDefault(n => n.Id == model.StoreId);
            //打印模板
            string posTemplate = FileHelper.ReadText("PosBillTemplate.txt");
            if (string.IsNullOrEmpty(posTemplate)) { throw new AppException("小票模板为空"); }
            posTemplate = posTemplate.ToLower();
            var lineLocation = posTemplate.LastIndexOf("##itemtemplate##");
            //分离商品item模板
            string billTemplate = posTemplate.ToLower().Substring(0, lineLocation);
            var itemStr = posTemplate.Substring(lineLocation);
            var len = itemStr.IndexOf("{");  //去掉　##itemtemplate##　以及换行符　从{{productname}}
            string itemTemplate = itemStr.Substring(len);
            //开始替换
            billTemplate = billTemplate.Replace("{{storename}}", store == null ? " " : store.Name);
            billTemplate = billTemplate.Replace("{{createdate}}", model.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss"));
            billTemplate = billTemplate.Replace("{{ordercode}}", model.Code);
            billTemplate = billTemplate.Replace("{{createdby}}", model.CreatedBy.ToString());
            billTemplate = billTemplate.Replace("{{status}}", model.Status.Description());
            //明细
            string productItems = "";
            foreach (var item in model.Items)
            {
                string tempItem = itemTemplate;
                tempItem = tempItem.Replace("{{productname}}", item.ProductName);
                tempItem = tempItem.Replace("{{productcode}}", item.ProductCode);
                tempItem = tempItem.Replace("{{price}}", item.RealPrice.ToString("F2"));
                tempItem = tempItem.Replace("{{quantity}}", item.Quantity.ToString());
                decimal amount = item.RealPrice * item.Quantity;
                tempItem = tempItem.Replace("{{amount}}", amount.ToString("F2"));
                productItems += tempItem; ;
            }
            billTemplate = billTemplate.Replace("{{items}}", productItems);
            //应收应付
            billTemplate = billTemplate.Replace("{{orderamount}}", model.OrderAmount.ToString("F2"));
            billTemplate = billTemplate.Replace("{{quantitytotal}}", model.GetQuantityTotal().ToString());
            billTemplate = billTemplate.Replace("{{discountamount}}", model.GetTotalDiscountAmount().ToString("F2"));
            billTemplate = billTemplate.Replace("{{payamount}}", model.PayAmount.ToString("F2"));
            billTemplate = billTemplate.Replace("{{chargeamount}}", model.GetChargeAmount().ToString("F2"));
            billTemplate = billTemplate.Replace("{{paymentway}}", model.PaymentWay.Description());
            billTemplate = billTemplate.Replace("{{onlinepayamount}}", model.OnlinePayAmount.ToString("F2"));

            _printService.Print(billTemplate);
        }


        public List<SaleOrder> QueryUploadSaleOrders(DateTime today)
        {
            var day = today.ToString("yyyy-MM-dd");
            string sql = "select * from SaleOrder Where (Status =@Paid or Status=@Cancel) and date(updatedOn) =@SyncDate ";
            var orders = _dbContext.Query<SaleOrder>(sql, new { Paid = (int)SaleOrderStatus.Paid, Cancel = (int)SaleOrderStatus.Cancel, SyncDate = day }).ToList();
            if (orders.Count == 0)
            {
                return orders;
            }
            // 查询明细
            string sqlitem = "select i.* from SaleOrderItem i inner join SaleOrder o on i.SaleOrderId=o.Id where (o.Status =@Paid or o.Status=@Cancel) and date(o.updatedOn) =@SyncDate ";
            var items = _dbContext.Query<SaleOrderItem>(sqlitem, new { Paid = (int)SaleOrderStatus.Paid, Cancel = (int)SaleOrderStatus.Cancel, SyncDate = day }).ToList();
            foreach (var order in orders)
            {
                order.Items.AddRange(items.Where(n => n.SaleOrderId == order.Id));
            }
            return orders;
        }

    }
}
