using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using EBS.Infrastructure.Helper;
using EBS.WinPos.Domain.Entity;
using EBS.WinPos.Domain;
using Newtonsoft.Json;
using EBS.Infrastructure.Log;
using EBS.WinPos.Service.Dto;
using Newtonsoft.Json.Converters;
using EBS.WinPos.Domain.ValueObject;

namespace EBS.WinPos.Service.Task
{
    public class SyncService
    {
        string _serverUrl;
        ILogger _log;
        int pageSize = 3000;
        DapperContext _db;
        SettingService _settingService;
        PosSettings _setting;

        public SyncService(ILogger log)
        {
            _serverUrl = Config.ApiService;
            this._log = log;
            _db = new DapperContext();
            _settingService = new SettingService();
            _setting = _settingService.GetSettings();
        }
        public void DownloadData()
        {
            var taskCount = 7;
            MultiThreadResetEvent threadEvent = new MultiThreadResetEvent(taskCount);

            //启动线程池来进行多线程下载数据
            ThreadPool.QueueUserWorkItem(new WaitCallback(DownloadAccount), threadEvent);
            ThreadPool.QueueUserWorkItem(new WaitCallback(DownloadProduct), threadEvent);
            ThreadPool.QueueUserWorkItem(new WaitCallback(DownloadStore), threadEvent);
            ThreadPool.QueueUserWorkItem(new WaitCallback(DownloadVipCard), threadEvent);
            ThreadPool.QueueUserWorkItem(new WaitCallback(DownloadVipProduct), threadEvent);
            ThreadPool.QueueUserWorkItem(new WaitCallback(DownloadProductAreaPrice), threadEvent);
            ThreadPool.QueueUserWorkItem(new WaitCallback(DownloadProductStorePrice), threadEvent);

            threadEvent.WaitAll();
            threadEvent.Dispose();
        }

        public bool NeedSyncData()
        {
            string sql = "select count(*) from account";
            var rows = _db.ExecuteScalar<int>(sql, null);
            return rows == 0;  // 没有数据，就需要同步
        }

        private void DownloadAccount(object state)
        {
            MultiThreadResetEvent are = (MultiThreadResetEvent)state;
            try
            {
                int count = 0;
                int pageIndex = 1;
                do
                {
                    string url = string.Format("{0}/PosSync/AccountByPage?pageSize={1}&pageIndex={2}", _serverUrl, pageSize, pageIndex);
                    // 下载数据
                    _log.Info("开始下载账户数据，请求{0}", url);
                    var result = HttpHelper.HttpGet(url);
                    if (!string.IsNullOrEmpty(result))
                    {
                        var rows = JsonConvert.DeserializeObject<List<Account>>(result);
                        _log.Info("已下载账户数据{0}条", rows.Count);
                        var sql = "INSERT INTO Account (Id,UserName,Password,StoreId,Status,RoleId,NickName)VALUES (@Id,@UserName,@Password,@StoreId,@Status,@RoleId,@NickName)";
                        var usql = "update Account set UserName=@UserName,Password=@Password,StoreId=@StoreId,Status=@Status,RoleId=@RoleId,NickName=@NickName where Id=@Id";
                        foreach (var entity in rows)
                        {
                            if (_db.ExecuteScalar<int>("select count(*) from Account where Id=@Id", new { Id = entity.Id }) > 0)
                            {
                                _db.ExecuteSql(usql, entity);
                            }
                            else
                            {
                                _db.ExecuteSql(sql, entity);
                            }
                        }
                        //入库
                        count = rows.Count();
                    }
                    else
                    {
                        count = 0;
                    }
                    pageIndex += 1;
                }
                while (count == pageSize);
                _log.Info("结束下载账户数据");
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            are.Set();
        }

        public void DownloadProductSync()
        {
            var taskCount = 1;
            MultiThreadResetEvent threadEvent = new MultiThreadResetEvent(taskCount);
            ThreadPool.QueueUserWorkItem(new WaitCallback(DownloadProduct), threadEvent);
            threadEvent.WaitAll();
            threadEvent.Dispose();
        }

        public void DownloadProduct(object state)
        {
            MultiThreadResetEvent are = (MultiThreadResetEvent)state;
            int pageIndex = 1;
            int count = 0;
            do
            {
                string url = string.Format("{0}/PosSync/ProductByPage?pageSize={1}&pageIndex={2}&storeId={3}", _serverUrl, pageSize, pageIndex, _setting.StoreId);
                // 下载数据
                _log.Info("开始下载商品数据，请求{0}", url);
                var result = HttpHelper.HttpGet(url);
                if (!string.IsNullOrEmpty(result))
                {
                    var rows = JsonConvert.DeserializeObject<List<Product>>(result);
                    //入库   
                    _log.Info("已下载商品数据{0}条", rows.Count);
                    string sql = "INSERT INTO Product (Id,Code,Name,BarCode,Specification,Unit,SalePrice,UpdatedOn) values (@Id,@Code,@Name,@BarCode,@Specification,@Unit,@SalePrice,@UpdatedOn)";
                    var usql = "update Product set Code=@Code,Name=@Name,BarCode=@BarCode,Specification=@Specification,Unit=@Unit,SalePrice=@SalePrice,UpdatedOn=@UpdatedOn where Id=@Id";
                    foreach (var entity in rows)
                    {
                        if (_db.ExecuteScalar<int>("select count(*) from Product where Id=@Id", new { Id = entity.Id }) > 0)
                        {
                            _log.Info("更新:id={0},code={1},barcode={2},saleprice={3}", entity.Id, entity.Code, entity.BarCode, entity.SalePrice);
                            _db.ExecuteSql(usql, entity);
                        }
                        else
                        {
                            _log.Info("添加:id={0},code={1},barcode={2},saleprice={3}", entity.Id, entity.Code, entity.BarCode, entity.SalePrice);
                            _db.ExecuteSql(sql, entity);
                        }
                    }
                    count = rows.Count();
                }
                else
                {
                    count = 0;
                }
                pageIndex += 1;
            }
            while (count == pageSize);
            _log.Info("结束下载商品数据");

            // 标记线程执行完毕
            are.Set();

        }
        private void DownloadStore(object state)
        {
            MultiThreadResetEvent are = (MultiThreadResetEvent)state;
            try
            {
                int pageIndex = 1;
                int count = 0;
                do
                {
                    // 下载数据
                    string url = string.Format("{0}/PosSync/StoreByPage?pageSize={1}&pageIndex={2}", _serverUrl, pageSize, pageIndex);
                    _log.Info("开始下载门店数据，请求{0}", url);
                    var result = HttpHelper.HttpGet(url);
                    if (!string.IsNullOrEmpty(result))
                    {
                        var rows = JsonConvert.DeserializeObject<List<Store>>(result);
                        //入库 
                        _log.Info("已下载门店数据{0}条", rows.Count);
                        string sql = "INSERT INTO Store (Id,Code,Name,LicenseCode)VALUES (@Id,@Code,@Name,@LicenseCode)";
                        var usql = "update Store set Code=@Code,Name=@Name,LicenseCode=@LicenseCode where Id=@Id";
                        foreach (var entity in rows)
                        {
                            if (_db.ExecuteScalar<int>("select count(*) from Store where Id=@Id", new { Id = entity.Id }) > 0)
                            {
                                _db.ExecuteSql(usql, entity);
                            }
                            else
                            {
                                _db.ExecuteSql(sql, entity);
                            }
                        }
                        count = rows.Count();
                    }
                    else
                    {
                        count = 0;
                    }
                    pageIndex += 1;
                }
                while (count == pageSize);
                _log.Info("结束下载门店数据");
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            are.Set();
        }
        private void DownloadVipCard(object state)
        {
            MultiThreadResetEvent are = (MultiThreadResetEvent)state;
            try
            {
                int pageIndex = 1;
                int count = 0;
                do
                {
                    // 下载数据
                    string url = string.Format("{0}/PosSync/VipCardByPage?pageSize={1}&pageIndex={2}", _serverUrl, pageSize, pageIndex);
                    _log.Info("开始下载会员卡数据，请求{0}", url);
                    var result = HttpHelper.HttpGet(url);
                    if (!string.IsNullOrEmpty(result))
                    {
                        var rows = JsonConvert.DeserializeObject<List<VipCard>>(result);  //入库 
                        _log.Info("已下载会员卡数据{0}条", rows.Count);
                        string sql = "INSERT INTO VipCard (Id,Code,Discount)VALUES (@Id,@Code,@Discount)";
                        var usql = "update VipCard set Code=@Code,Discount=@Discount where Id=@Id";
                        foreach (var entity in rows)
                        {
                            if (_db.ExecuteScalar<int>("select count(*) from VipCard where Id=@Id", new { Id = entity.Id }) > 0)
                            {
                                _db.ExecuteSql(usql, entity);
                            }
                            else
                            {
                                _db.ExecuteSql(sql, entity);
                            }
                        }
                        count = rows.Count();
                    }
                    else
                    {
                        count = 0;
                    }
                    pageIndex += 1;
                }
                while (count == pageSize);
                _log.Info("结束下载会员卡数据");
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            are.Set();
        }
        private void DownloadVipProduct(object state)
        {
            MultiThreadResetEvent are = (MultiThreadResetEvent)state;
            try
            {
                int pageIndex = 1;
                int count = 0;
                do
                {
                    // 下载数据
                    string url = string.Format("{0}/PosSync/VipProductByPage?pageSize={1}&pageIndex={2}", _serverUrl, pageSize, pageIndex);
                    _log.Info("开始下载会员商品数据，请求{0}", url);
                    var result = HttpHelper.HttpGet(url);
                    if (!string.IsNullOrEmpty(result))
                    {
                        var rows = JsonConvert.DeserializeObject<List<VipProduct>>(result);  //入库 
                        _log.Info("已下载会员商品数据{0}条", rows.Count);
                        string sql = "INSERT INTO VipProduct (Id,ProductId,SalePrice)VALUES (@Id,@ProductId,@SalePrice)";
                        var usql = "update VipProduct set ProductId=@ProductId,SalePrice=@SalePrice where Id=@Id";
                        foreach (var entity in rows)
                        {
                            if (_db.ExecuteScalar<int>("select count(*) from VipProduct where Id=@Id", new { Id = entity.Id }) > 0)
                            {
                                _db.ExecuteSql(usql, entity);
                            }
                            else
                            {
                                _db.ExecuteSql(sql, entity);
                            }
                        }
                        count = rows.Count();
                    }
                    else
                    {
                        count = 0;
                    }
                    pageIndex += 1;
                }
                while (count == pageSize);
                _log.Info("结束下载会员商品数据");
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            are.Set();
        }

        private void DownloadProductAreaPrice(object state)
        {
            MultiThreadResetEvent are = (MultiThreadResetEvent)state;
            try
            {
                int pageIndex = 1;
                int count = 0;
                do
                {
                    // 下载数据
                    string url = string.Format("{0}/PosSync/ProductAreaPriceByPage?pageSize={1}&pageIndex={2}", _serverUrl, pageSize, pageIndex);
                    _log.Info("开始下载商品区域价数据，请求{0}", url);
                    var result = HttpHelper.HttpGet(url);
                    if (!string.IsNullOrEmpty(result))
                    {
                        var rows = JsonConvert.DeserializeObject<List<ProductAreaPrice>>(result);  //入库 
                        _log.Info("已下载商品区域价数据{0}条", rows.Count);
                        string sql = "INSERT INTO ProductAreaPrice (Id,ProductId,AreaId,SalePrice)VALUES (@Id,@ProductId,@AreaId,@SalePrice)";
                        var usql = "update ProductAreaPrice set ProductId=@ProductId,SalePrice=@SalePrice,AreaId=@AreaId where Id=@Id";
                        foreach (var entity in rows)
                        {
                            if (_db.ExecuteScalar<int>("select count(*) from ProductAreaPrice where Id=@Id", new { Id = entity.Id }) > 0)
                            {
                                _db.ExecuteSql(usql, entity);
                            }
                            else
                            {
                                _db.ExecuteSql(sql, entity);
                            }
                        }
                        count = rows.Count();
                    }
                    else
                    {
                        count = 0;
                    }
                    pageIndex += 1;
                }
                while (count == pageSize);
                _log.Info("结束下载商品区域价数据");
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            are.Set();
        }

        private void DownloadProductStorePrice(object state)
        {
            MultiThreadResetEvent are = (MultiThreadResetEvent)state;
            try
            {
                int pageIndex = 1;
                int count = 0;
                do
                {
                    // 下载数据
                    string url = string.Format("{0}/PosSync/ProductStorePriceByPage?pageSize={1}&pageIndex={2}", _serverUrl, pageSize, pageIndex);
                    _log.Info("开始下载商品门店价数据，请求{0}", url);
                    var result = HttpHelper.HttpGet(url);
                    if (!string.IsNullOrEmpty(result))
                    {
                        var rows = JsonConvert.DeserializeObject<List<ProductStorePrice>>(result);  //入库  
                        _log.Info("已下载商品门店价数据{0}条", rows.Count);
                        string sql = "INSERT INTO ProductStorePrice (Id,ProductId,StoreId,SalePrice)VALUES (@Id,@ProductId,@StoreId,@SalePrice)";
                        var usql = "update ProductStorePrice set ProductId=@ProductId,StoreId=@StoreId,SalePrice=@SalePrice where Id=@Id";
                        foreach (var entity in rows)
                        {
                            if (_db.ExecuteScalar<int>("select count(*) from ProductStorePrice where Id=@Id", new { Id = entity.Id }) > 0)
                            {
                                _db.ExecuteSql(usql, entity);
                            }
                            else
                            {
                                _db.ExecuteSql(sql, entity);
                            }
                        }
                        count = rows.Count();
                    }
                    else
                    {
                        count = 0;
                    }
                    pageIndex += 1;
                }
                while (count == pageSize);
                _log.Info("结束下载商品门店价数据");
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            are.Set();
        }

        public void Send(SaleOrder model)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(SendSaleOrder), model);
        }
        public void Send(WorkSchedule model)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(SendWorkSchedule), model);
        }

        private void SendWorkSchedule(object data)
        {
            var model = data as WorkSchedule;
            try
            {
                _log.Info("班次{0},开始同步", model.Code);
                string url = string.Format("{0}/PosSync/WorkScheduleSync", _serverUrl);
                var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                string body = JsonConvert.SerializeObject(model, dateTimeConverter);
                string param = string.Format("body={0}", body);
                string result = HttpHelper.HttpPost(url, param);
                if (result == "1")
                {
                    string sql = @"Update WorkSchedule set IsSync=1 where @Id=@Id";
                    _db.ExecuteSql(sql, new { Id = model.Id });
                    _log.Info("班次{0},同步成功", model.Code);
                }
                else
                {
                    _log.Info("班次{0},同步失败", model.Code);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);

            }
        }



        private void SendSaleOrder(object data)
        {
            var model = data as SaleOrder;           
            try
            {
                _log.Info("销售单{0},开始同步", model.Code);
                string url = string.Format("{0}/PosSync/SaleOrderSync", _serverUrl);
                var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                string body = JsonConvert.SerializeObject(model, dateTimeConverter);
                string param = string.Format("body={0}", body);
                string result = HttpHelper.HttpPost(url, param);
                if (result == "1")
                {
                    string sql = @"Update SaleOrder set IsSync=1 where @Id=@Id";
                    _db.ExecuteSql(sql, new { Id = model.Id });
                    _log.Info("销售单{0},同步成功", model.Code);
                }
                else
                {
                    _log.Info("销售单{0},同步失败", model.Code);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);

            }
            if (model.GetAre() != null)
            {
                model.GetAre().Set(); // 线程同步
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="day">today  yyyy-MM-dd</param>
        public void SaleSyncDaily(string day)
        {
            string sql = "select * from SaleOrder Where (Status =@Paid or Status=@Cancel) and date(updatedOn) =@SyncDate ";
            var result = _db.Query<SaleOrder>(sql, new { Paid = (int)SaleOrderStatus.Paid, Cancel = (int)SaleOrderStatus.Cancel, SyncDate = day });
            if (result.Count() == 0)
            {
                _log.Info("销售数据为空，终止上传");
                return;
            }
            var taskCount = result.Count();
            MultiThreadResetEvent threadEvent = new MultiThreadResetEvent(taskCount);
            foreach (var model in result)
            {
                string sqlitem = "select * from SaleOrderItem where SaleOrderId=@SaleOrderId";
                var items = _db.Query<SaleOrderItem>(sqlitem, new { SaleOrderId = model.Id }).ToList();
                model.Items = items;
                model.SetAre(threadEvent);  // 线程同步
                Send(model);
                Thread.Sleep(5);
            }

            threadEvent.WaitAll();
            threadEvent.Dispose();
        }


        /// <summary>
        /// 上传销售对账
        /// </summary>
        /// <param name="today">格式：yyyy-MM-dd</param>
        public void UploadSaleSync(string today)
        {
            string sql = @"select s.StoreId,s.PosId,count(*) as orderCount,sum(orderAmount) as OrderTotalAmount
 from saleorder s where date(updatedOn) = @UpdatedOn and Status in (-1,3) 
 group by s.StoreId,s.PosId ";
            var rows = _db.Query<SaleSync>(sql, new { UpdatedOn = today }).ToList();
            if (rows.Count == 0) {
                _log.Info("销售数据为空，终止上传");
                return;
            }
            rows.ForEach(n => n.SaleDate = today);
            _log.Info("上传销售对账");
            string url = string.Format("{0}/PosSync/UpdateSaleSync", _serverUrl);
            var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            string body = JsonConvert.SerializeObject(rows, dateTimeConverter);
            string param = string.Format("body={0}", body);
            string result = HttpHelper.HttpPost(url, param);
            if (result == "1")
            {
                _log.Info("销售对账上传成功");
            }
            else
            {
                _log.Info("销售对账上传失败");
            }
        }
    }
}
