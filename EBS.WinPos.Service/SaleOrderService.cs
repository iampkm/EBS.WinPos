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
using EBS.WinPos.Service.WxPayAPI;
using WxPayAPI;
using Newtonsoft.Json;
namespace EBS.WinPos.Service
{
    public class SaleOrderService
    {
        Repository _db;
        IPosPrinter _printService;
        SyncService _syncService;
        ILogger _log;
        DapperContext _dbContext;
        IMicropay _wechatPay;
        public SaleOrderService()
        {
            _log = AppContext.Log;
            _db = new Repository();
            _printService = new DriverPrinterService();
            _syncService = new SyncService(AppContext.Log);
            _dbContext = new DapperContext();
            _wechatPay = new Micropay();

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
                OrderLevel = cat.OrderLevel
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
                OrderLevel = cat.OrderLevel
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
            var model = _db.Orders.Include(n => n.Items).FirstOrDefault(n => n.Id == orderId);
            if (model == null) { throw new AppException("订单不存在"); }
            if (payAmount < model.OrderAmount)
            {
                throw new AppException("支付金额低于订单金额");
            }
            if (payAmount > model.OrderAmount + 100)
            {
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

        public void ScanPay(int orderId, PaymentWay paymentWay)
        {
            var model = _db.Orders.Include(n => n.Items).FirstOrDefault(n => n.Id == orderId);
            if (model == null) { throw new AppException("订单不存在"); }
            model.OnlinePayAmount = model.OrderAmount;
            model.PayAmount = 0;

            model.FinishPaid(model.PayAmount, model.OnlinePayAmount, paymentWay);
            //保存交易记录
            _db.SaveChanges();
            //打印小票
            PrintOrderTicket(model);
            //同步到服务器
            _syncService.Send(model);
        }

        //public void CashRefund(int orderId, string licenseCode, decimal payAmount,string sourceSaleOrderCode)
        public void CashRefund(ShopCart cart, string licenseCode)
        {
            if (string.IsNullOrEmpty(licenseCode)) { throw new AppException("请输入店长授权码"); }
            var model = _db.Orders.Include(n => n.Items).FirstOrDefault(n => n.Id == cart.OrderId);
            if (model == null) { throw new AppException("订单不存在"); }
            var store = _db.Stores.FirstOrDefault(n => n.Id == model.StoreId);
            if (!store.VerifyLicenseCode(licenseCode)) { throw new AppException("店长授权码错误"); }

            UpdateRefundItem(model, cart);

            model.FinishPaid(cart.OrderAmount);
            //保存交易记录
            _db.SaveChanges();
            //打印小票
            PrintOrderTicket(model);
            //同步到服务器
            _syncService.Send(model);
        }

        public void ScanRefund(ShopCart cart, string licenseCode)
        {
            if (string.IsNullOrEmpty(licenseCode)) { throw new AppException("请输入店长授权码"); }
            var model = _db.Orders.Include(n => n.Items).FirstOrDefault(n => n.Id == cart.OrderId);
            if (model == null) { throw new AppException("订单不存在"); }
            var store = _db.Stores.FirstOrDefault(n => n.Id == model.StoreId);
            if (!store.VerifyLicenseCode(licenseCode)) { throw new AppException("店长授权码错误"); }

            UpdateRefundItem(model, cart);

            model.FinishPaid(0, cart.OrderAmount, cart.PaymentWay);
            //保存交易记录
            _db.SaveChanges();
            //打印小票
            PrintOrderTicket(model);
            //同步到服务器
            _syncService.Send(model);
        }

        /// <summary>
        ///  根据购物车明细，现修改退单和商品价格（按原单退）
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cart"></param>
        private void UpdateRefundItem(SaleOrder model, ShopCart cart)
        {
            model.SourceSaleOrderCode = cart.SourceSaleOrderCode;
            // 修改明细价格
            foreach (var item in model.Items)
            {
                var line = cart.Items.FirstOrDefault(n => n.ProductId == item.ProductId);
                if (line != null)
                {
                    item.SalePrice = line.SalePrice;
                    item.RealPrice = line.RealPrice;
                }
            }
        }

        #region 微信支付

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="payBarCode"></param>
        /// <param name="payAmount">现金付款金额</param>
        public void WechatPay(int orderId, string payBarCode, decimal payAmount = 0)
        {
            if (string.IsNullOrEmpty(payBarCode)) { throw new AppException("请录入或扫微信付款条码"); }
            var model = _db.Orders.Include(n => n.Items).FirstOrDefault(n => n.Id == orderId);
            if (model == null) { throw new AppException("订单不存在"); }
            if (model.OrderAmount <= 0) { throw new AppException("订单金额不能为0"); }
            if (model.PayAmount > model.OrderAmount) { throw new AppException("请使用现金支付"); }
            // 发起微信支付
            model.OnlinePayAmount = model.OrderAmount;
            //string data = JsonHelper.GetJson(new { barcode = payBarCode, orderId = model.Code, paymentAmt = model.OnlinePayAmount.ToString("F2") });
            //string sign = EncryptHelpler.SignEncrypt(data, Config.SignKey_WeChatBarcode);
            //string result = HttpHelper.HttpRequest("POST", Config.Api_Pay_WeChatBarcode, param: string.Format("appId=GGX&sign={0}&data={1}", sign, data), timeOut: 90000);

            var fee = (model.OrderAmount * 100).ToString(); // 金额单位为分，不能带小数
                                                            // var store = _db.Stores.FirstOrDefault(n => n.Id == model.StoreId);
            var body = model.Items[0].ProductName;
            // 发起微信支付
            string bizContent = JsonHelper.GetJson(new { body = body, total_amount = fee, out_trade_no = model.Code, auth_code = payBarCode });
            PayRequest request = new PayRequest()
            {
                Method = "wechat.trade.barcode.pay",
                StoreId = model.StoreId.ToString(),
                BizContent = bizContent
            };
            var result = PostPayReqeust(request);
            if (result.WechatTransactionSuccess())
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

        public PayResponse WechatTradeQuery(string orderCode, int storeId)
        {
            string bizContent = JsonHelper.GetJson(new { trade_no = "", out_trade_no = orderCode });            //                                         
            PayRequest request = new PayRequest()
            {
                Method = "wechat.trade.query",
                StoreId = storeId.ToString(),
                BizContent = bizContent
            };
            var result = PostPayReqeust(request); 
            return result;
        }

        /// <summary>
        /// 修正订单状态
        /// </summary>
        public void CorrectWechatOrderStatus(SaleOrder order, PayResponse response)
        {
            if (order.Status == Domain.ValueObject.SaleOrderStatus.WaitPaid && response.WechatTransactionSuccess()&&response.WechatTradeStatusSuccess())
            {
                order.FinishPaid(order.PayAmount,order.OnlinePayAmount,order.PaymentWay);
                order.PaidDate = DateTime.ParseExact(response.data["TimeEnd"], "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture); 
                _db.SaveChanges();

                // 同步后端服务器
                _syncService.Send(order);
            }
        }

        public void WechatRefund(ShopCart cart, string licenseCode)
        {
            if (string.IsNullOrEmpty(licenseCode)) { throw new AppException("请输入店长授权码"); }
            var model = _db.Orders.Include(n => n.Items).FirstOrDefault(n => n.Id == cart.OrderId);
            if (model == null) { throw new AppException("订单不存在"); }

            var store = _db.Stores.FirstOrDefault(n => n.Id == model.StoreId);
            if (!store.VerifyLicenseCode(licenseCode)) { throw new AppException("店长授权码错误"); }

            model.OnlinePayAmount = model.OrderAmount;
            UpdateRefundItem(model, cart);
            // 发起微信支付
            model.WaitRefund(0, model.OnlinePayAmount, PaymentWay.WechatPay);
            _db.SaveChanges();

            var body = "退货:" + model.Items[0].ProductName;
            string bizContent = JsonHelper.GetJson(new { trade_no = "", out_trade_no = cart.SourceSaleOrderCode, total_amount = Math.Abs(model.OrderAmount) * 100, refund_amount = Math.Abs(model.OrderAmount) * 100, refund_reason = body, out_refund_no = model.Code });
            //                                          trade_no = "", out_trade_no = "", total_amount=0, refund_amount=0, refund_desc="", out_refund_no =""
            PayRequest request = new PayRequest()
            {
                Method = "wechat.trade.refund",
                StoreId = model.StoreId.ToString(),
                BizContent = bizContent
            };
            var result = PostPayReqeust(request);
            if (result.WechatTransactionSuccess())
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

        public PayResponse WechatRefundQuery(string refundCode, int storeId)
        {
            string bizContent = JsonHelper.GetJson(new { trade_no = "", out_trade_no = "", refund_no = "", out_refund_no = refundCode });            //                                         
            PayRequest request = new PayRequest()
            {
                Method = "wechat.trade.refund.query",
                StoreId = storeId.ToString(),
                BizContent = bizContent
            };
            var result = PostPayReqeust(request);           

            return result;
        }

        public void CorrectWechatRefundStatus(SaleOrder order, PayResponse response)
        {
            if (order.Status == Domain.ValueObject.SaleOrderStatus.WaitPaid && response.WechatTransactionSuccess() && response.WechatTradeStatusSuccess())
            {
                order.FinishPaid(order.PayAmount, order.OnlinePayAmount, order.PaymentWay);
               // order.PaidDate = DateTime.ParseExact(response.data["TimeEnd"], "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                _db.SaveChanges();

                // 同步后端服务器
                _syncService.Send(order);
            }
        }

        #endregion
        #region alipay 支付
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="payBarCode"></param>
        /// <param name="payAmount">现金付款金额</param>
        public void AliPay(int orderId, string payBarCode, decimal payAmount = 0)
        {
            if (string.IsNullOrEmpty(payBarCode)) { throw new AppException("请录入或扫支付宝付款条码"); }
            var model = _db.Orders.Include(n => n.Items).FirstOrDefault(n => n.Id == orderId);
            if (model == null) { throw new AppException("订单不存在"); }
            if (model.OrderAmount <= 0) { throw new AppException("订单金额不能为0"); }
            if (Math.Abs(model.PayAmount) > Math.Abs(model.OrderAmount)) { throw new AppException("请使用现金支付"); }
            // 发起支付
            model.OnlinePayAmount = model.OrderAmount - model.PayAmount;

            // var fee = int.Parse((model.OnlinePayAmount * 100).ToString()); // 金额单位为分，不能带小数
            //  var store = _db.Stores.FirstOrDefault(n => n.Id == model.StoreId);
            var body = model.Items[0].ProductName;
            string bizContent = JsonHelper.GetJson(new { body = body, total_amount = model.OnlinePayAmount.ToString("F2"), out_trade_no = model.Code, auth_code = payBarCode, subject = body });
            PayRequest request = new PayRequest()
            {
                Method = "alipay.trade.barcode.pay",
                StoreId = model.StoreId.ToString(),
                BizContent = bizContent
            };

            var result = PostPayReqeust(request);
            if (result.AlipayTransactionSuccess())
            {
                //支付成功
                model.FinishPaid(model.PayAmount, model.OnlinePayAmount, PaymentWay.AliPay);
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

        public PayResponse AliPayTradeQuery(string orderCode, int storeId)
        {
            string bizContent = JsonHelper.GetJson(new { trade_no = "", out_trade_no = orderCode });            //                                         
            PayRequest request = new PayRequest()
            {
                Method = "aliPay.trade.query",
                StoreId = storeId.ToString(),
                BizContent = bizContent
            };
            var result = PostPayReqeust(request);        
            return result;
        }

        public void CorrentAlipayOrderStatus(SaleOrder order, PayResponse response)
        {
            if (order.Status == Domain.ValueObject.SaleOrderStatus.WaitPaid && response.AlipayTransactionSuccess() && response.AlipayTradeStatusSuccess())
            {
                order.FinishPaid(order.PayAmount, order.OnlinePayAmount, order.PaymentWay);
                order.PaidDate = DateTime.ParseExact(response.data["SendPayDate"], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                _db.SaveChanges();

                //同步后端服务器
                _syncService.Send(order);
            }
        }
        public void CorrentAlipayRefundStatus(SaleOrder order, PayResponse response)
        {
            if (order.Status == Domain.ValueObject.SaleOrderStatus.WaitPaid && response.AlipayTransactionSuccess() && response.AlipayTradeStatusSuccess())
            {
                order.FinishPaid(order.PayAmount, order.OnlinePayAmount, order.PaymentWay);
              //  order.PaidDate = DateTime.ParseExact(response.data["SendPayDate"], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                _db.SaveChanges();

                //同步后端服务器
                _syncService.Send(order);
            }
        }

        public void AliRefund(ShopCart cart, string licenseCode)
        {
            if (string.IsNullOrEmpty(licenseCode)) { throw new AppException("请输入店长授权码"); }
            var model = _db.Orders.Include(n => n.Items).FirstOrDefault(n => n.Id == cart.OrderId);
            if (model == null) { throw new AppException("订单不存在"); }

            // 修改价格，标记退款单为待退款
            var store = _db.Stores.FirstOrDefault(n => n.Id == model.StoreId);
            if (!store.VerifyLicenseCode(licenseCode)) { throw new AppException("店长授权码错误"); }
            UpdateRefundItem(model, cart);
            model.WaitRefund(0, model.OrderAmount, PaymentWay.AliPay);
            _db.SaveChanges();

            // 向后端服务发起退款
            var body = "退货:" + model.Items[0].ProductName;
            string bizContent = JsonHelper.GetJson(new { trade_no = "", out_trade_no = cart.SourceSaleOrderCode, refund_amount = Math.Abs(model.OrderAmount), refund_reason = body, out_refund_no = model.Code });
            PayRequest request = new PayRequest()
            {
                Method = "alipay.trade.refund",
                StoreId = model.StoreId.ToString(),
                BizContent = bizContent
            };
            var result = PostPayReqeust(request);
            if (result.AlipayTransactionSuccess())
            {
                //支付成功
                model.FinishPaid(model.PayAmount, model.OnlinePayAmount, PaymentWay.AliPay);
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

        public PayResponse AliPayRefundQuery(string orderCode, int storeId)
        {
            string bizContent = JsonHelper.GetJson(new { trade_no = "", out_trade_no = orderCode, out_refund_no = "" });            //                                         
            PayRequest request = new PayRequest()
            {
                Method = "aliPay.trade.refund.query",
                StoreId = storeId.ToString(),
                BizContent = bizContent
            };
            var result = PostPayReqeust(request);
            var queryList = new List<string>();
            

            return result;
        }

        private PayResponse PostPayReqeust(PayRequest request)
        {
            string data = JsonHelper.GetJson(request);
            string sign = EncryptHelpler.SignEncrypt(data, Config.SignKey_AliBarcode);
            var paramsDics = request.toDic();
            paramsDics.Add("sign", sign);
            var postData = request.getKeyValueString(paramsDics);
            //string result = HttpHelper.HttpRequest("POST", Config.ApiService+ "/Pay/Geteway", param: string.Format("appId=GGX&sign={0}&data={1}", sign, data), timeOut: 90000);
            string result = HttpHelper.HttpRequest("POST", Config.ApiService + "/Pay/Geteway", param: postData, timeOut: 90000);
            var payResponse = JsonConvert.DeserializeObject<PayResponse>(result);
            return payResponse;
        }

        #endregion

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
                if (Config.IsPrintTicket)
                {
                    PrintTicket(model);
                }
                else
                {
                    _log.Info("配置参数：IsPrintTicket 为false，将不打印小票");
                }
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
            // 追加小票二维码
            billTemplate = billTemplate.Replace("{{orderbarcode}}", model.Code.CreateBarCode());
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

        /// <summary>
        ///  查询已支付销售单，订单不存在返回null
        /// </summary>
        /// <param name="code">订单或退单号</param>
        /// <returns></returns>
        public SaleOrder QuerySaleOrder(string code)
        {
            string sql = "select * from SaleOrder Where Code=@Code ";
            var order = _dbContext.First<SaleOrder>(sql, new { Code = code });
            if (order == null)
            {
                return null;
            }
            // 查询明细
            string sqlitem = "select i.* from SaleOrderItem i where i.SaleOrderId =@SaleOrderId ";
            var items = _dbContext.Query<SaleOrderItem>(sqlitem, new { SaleOrderId = order.Id }).ToList();
            order.Items = items;
            return order;
        }

        /// <summary>
        /// 查询已退款明细
        /// </summary>
        /// <param name="sourceSaleOrderCode"></param>
        /// <returns></returns>
        public List<SaleOrderItem> QueryRefundOrderItems(string sourceSaleOrderCode)
        {
            //string sql = "select * from SaleOrder Where Status =@Paid And SourceSaleOrderCode=@SourceSaleOrderCode";
            //var orders = _dbContext.Query<SaleOrder>(sql, new { Paid = (int)SaleOrderStatus.Paid, SourceSaleOrderCode = sourceSaleOrderCode }).ToList();
            //if (orders.Count == 0)
            //{
            //    return orders;
            //}
            // 查询明细
            string sqlitem = "select i.* from SaleOrderItem i inner join SaleOrder o on i.SaleOrderId=o.Id where Status =@Paid And SourceSaleOrderCode=@SourceSaleOrderCode ";
            var items = _dbContext.Query<SaleOrderItem>(sqlitem, new { Paid = (int)SaleOrderStatus.Paid, SourceSaleOrderCode = sourceSaleOrderCode }).ToList();

            return items;
        }

    }
}
