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
namespace EBS.WinPos.Service.Task
{
    public class SyncService
    {
        string _serverUrl;
        ILogger _log;
        int pageSize = 1000;
        DapperContext _db ;

        public SyncService(ILogger log)
        {
            _serverUrl = Config.ApiService;
            this._log = log;
            _db = new DapperContext();
        }
        public void DownloadData()
        {
            //启动线程池来进行多线程下载数据
            ThreadPool.QueueUserWorkItem(new WaitCallback(DownloadAccount));
            ThreadPool.QueueUserWorkItem(new WaitCallback(DownloadProduct));
            ThreadPool.QueueUserWorkItem(new WaitCallback(DownloadStore));
            ThreadPool.QueueUserWorkItem(new WaitCallback(DownloadVipCard));
            ThreadPool.QueueUserWorkItem(new WaitCallback(DownloadVipProduct));
            ThreadPool.QueueUserWorkItem(new WaitCallback(DownloadProductAreaPrice));
            ThreadPool.QueueUserWorkItem(new WaitCallback(DownloadProductStorePrice));
          
           
        }
        private void DownloadAccount(object table)
        {           
            try
            {
                int count = 0;
                int pageIndex = 1;
                do
                {                   
                    string url = string.Format("{0}/PosSync/AccountByPage?pageSize={1}&pageIndex={2}", _serverUrl, pageSize, pageIndex);
                    // 下载数据
                    var result = HttpHelper.HttpGet(url);
                    if (!string.IsNullOrEmpty(result))
                    {
                        var rows = JsonConvert.DeserializeObject<List<Account>>(result);
                        //入库                     
                        string sql = "INSERT INTO Account (Id,UserName,Password,StoreId,Status,RoleId,NickName)VALUES (@Id,@UserName,@Password,@StoreId,@Status,@RoleId,@NickName)";
                        _db.ExecuteSql(sql, rows.ToArray());
                        count = rows.Count();
                    }
                    else {
                        count = 0;
                    }
                    pageIndex += 1;
                }
                while (count == pageSize);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }
        private void DownloadProduct(object table)
        {
            try
            {               
                int pageIndex = 1;
                int count = 0;
                do
                {                   
                    string url = string.Format("{0}/PosSync/ProductByPage?pageSize={1}&pageIndex={2}", _serverUrl, pageSize, pageIndex);
                    // 下载数据
                    var result = HttpHelper.HttpGet(url);
                    if (!string.IsNullOrEmpty(result))
                    {
                        var rows = JsonConvert.DeserializeObject<List<Product>>(result);
                        //入库                      
                        string sql = "INSERT INTO Product (Id,Code,Name,BarCode,Specification,Unit,SalePrice,UpdatedOn) values (@Id,@Code,@Name,@BarCode,@Specification,@Unit,@SalePrice,@UpdatedOn)";
                        _db.ExecuteSql(sql, rows.ToArray());
                        count = rows.Count();
                    }
                    else
                    {
                        count = 0;
                    }
                    pageIndex += 1;
                }
                while (count == pageSize);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }
        private void DownloadStore(object table)
        {
            try
            {
                int pageIndex = 1;
                int count = 0;
                do
                {
                    // 下载数据
                    string url = string.Format("{0}/PosSync/StoreByPage?pageSize={1}&pageIndex={2}", _serverUrl, pageSize, pageIndex);                   
                    var result = HttpHelper.HttpGet(url);
                    if (!string.IsNullOrEmpty(result))
                    {
                        var rows = JsonConvert.DeserializeObject<IEnumerable<Store>>(result);
                        //入库                      
                        string sql = "INSERT INTO Store (Id,Code,Name,LicenseCode)VALUES (@Id,@Code,@Name,@LicenseCode)";
                        _db.ExecuteSql(sql, rows.ToArray());
                        count = rows.Count();
                    }
                    else
                    {
                        count = 0;
                    }
                    pageIndex += 1;
                }
                while (count == pageSize);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }
        private void DownloadVipCard(object table)
        {
            try
            {
                int pageIndex = 1;
                int count = 0;
                do
                {
                    // 下载数据
                    string url = string.Format("{0}/PosSync/VipCardByPage?pageSize={1}&pageIndex={2}", _serverUrl, pageSize, pageIndex);
                    var result = HttpHelper.HttpGet(url);
                    if (!string.IsNullOrEmpty(result))
                    {
                        var rows = JsonConvert.DeserializeObject<IEnumerable<VipCard>>(result);  //入库                     
                        string sql = "INSERT INTO VipCard (Id,Code,Discount)VALUES (@Id,@Code,@Discount)";
                        _db.ExecuteSql(sql, rows.ToArray());
                        count = rows.Count();
                    }
                    else
                    {
                        count = 0;
                    }
                    pageIndex += 1;
                }
                while (count == pageSize);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }
        private void DownloadVipProduct(object table)
        {
            try
            {
                int pageIndex = 1;
                int count = 0;
                do
                {
                    // 下载数据
                    string url = string.Format("{0}/PosSync/VipProductByPage?pageSize={1}&pageIndex={2}", _serverUrl, pageSize, pageIndex);
                    var result = HttpHelper.HttpGet(url);
                    if (!string.IsNullOrEmpty(result))
                    {
                        var rows = JsonConvert.DeserializeObject<IEnumerable<VipProduct>>(result);  //入库                     
                        string sql = "INSERT INTO VipProduct (Id,ProductId,SalePrice)VALUES (@Id,@ProductId,@SalePrice)";
                        _db.ExecuteSql(sql, rows.ToArray());
                        count = rows.Count();
                    }
                    else
                    {
                        count = 0;
                    }
                    pageIndex += 1;
                }
                while (count == pageSize);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }

        private void DownloadProductAreaPrice(object table)
        {
            try
            {
                int pageIndex = 1;
                int count = 0;
                do
                {
                    // 下载数据
                    string url = string.Format("{0}/PosSync/ProductAreaPriceByPage?pageSize={1}&pageIndex={2}", _serverUrl, pageSize, pageIndex);
                    var result = HttpHelper.HttpGet(url);
                    if (!string.IsNullOrEmpty(result))
                    {
                        var rows = JsonConvert.DeserializeObject<IEnumerable<VipProduct>>(result);  //入库                     
                        string sql = "INSERT INTO ProductAreaPrice (Id,ProductId,SalePrice)VALUES (@Id,@ProductId,@SalePrice)";
                        _db.ExecuteSql(sql, rows.ToArray());
                        count = rows.Count();
                    }
                    else
                    {
                        count = 0;
                    }
                    pageIndex += 1;
                }
                while (count == pageSize);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }

        private void DownloadProductStorePrice(object table)
        {
            try
            {
                int pageIndex = 1;
                int count = 0;
                do
                {
                    // 下载数据
                    string url = string.Format("{0}/PosSync/ProductStorePriceByPage?pageSize={1}&pageIndex={2}", _serverUrl, pageSize, pageIndex);
                    var result = HttpHelper.HttpGet(url);
                    if (!string.IsNullOrEmpty(result))
                    {
                        var rows = JsonConvert.DeserializeObject<IEnumerable<VipProduct>>(result);  //入库                     
                        string sql = "INSERT INTO ProductStorePrice (Id,ProductId,SalePrice)VALUES (@Id,@ProductId,@SalePrice)";
                        _db.ExecuteSql(sql, rows.ToArray());
                        count = rows.Count();
                    }
                    else
                    {
                        count = 0;
                    }
                    pageIndex += 1;
                }
                while (count == pageSize);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }

        public void Send(SaleOrder model)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(SendSaleOrder), model);
        }
        public void Send(WorkSchedule model)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(SendWorkSchedule), model);
        }

        public void Send(InputCashAmount  model)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(SendInputCashAmount), model);
        }

        private void SendInputCashAmount(object data)
        {
            var model = data as InputCashAmount;
            try
            {

                string url = string.Format("{0}/PosSync/Hander", _serverUrl);
                string body = JsonConvert.SerializeObject(model);
                string eventName = "CreatedWorkSchedule";
                string param = string.Format("body={0}&eventName={1}", body, eventName);
                string result = HttpHelper.HttpPost(url, param);
                if (result == "1")
                {
                    string sql = @"Update WorkSchedule set IsSyncAmount=1 where @Id=@Id";
                    _db.ExecuteSql(sql, new { Id = model.Id });
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);

            }
        }

        private void SendWorkSchedule(object data)
        {
            var model = data as WorkSchedule;
            try
            {

                string url = string.Format("{0}/PosSync/Hander", _serverUrl);
                string body = JsonConvert.SerializeObject(model);
                string eventName = "CreatedWorkSchedule";
                string param = string.Format("body={0}&eventName={1}", body, eventName);
                string result = HttpHelper.HttpPost(url, param);
                if (result == "1")
                {
                    string sql = @"Update WorkSchedule set IsSync=1 where @Id=@Id";
                    _db.ExecuteSql(sql, new { Id = model.Id });
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
                string url = string.Format("{0}/PosSync/SaleOrderSync",_serverUrl);
                var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                string body = JsonConvert.SerializeObject(model, dateTimeConverter);
                string param = string.Format("body={0}", body);
                string result = HttpHelper.HttpPost(url, param);
                if(result=="1")
                {
                    string sql = @"Update SaleOrder set IsSync=1 where @Id=@Id";
                    _db.ExecuteSql(sql, new { Id = model.Id});
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
             
            } 
        }
    }
}
