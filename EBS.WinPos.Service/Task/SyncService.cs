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
using EBS.Infrastructure;
using System.Reflection;
namespace EBS.WinPos.Service.Task
{
    public class SyncService
    {
        string _serverUrl;
        ILogger _log;
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

        private string BuildAccessToken()
        {
           // if (string.IsNullOrEmpty(_setting.CDKey)) { _log.Info("没有cdkey，构建参数失败，处理终止!"); throw new AppException("没有cdkey，构建参数失败，处理终止!"); }
            string paramters = string.Format("StoreId={0}&PosId={1}&CDKey={2}", _setting.StoreId, _setting.PosId, _setting.CDKey);
            return paramters;            
        }

        public bool NeedSyncData()
        {
            string sql = "select count(*) from account";
            var rows = _db.ExecuteScalar<int>(sql, null);
            return rows == 0;  // 没有数据，就需要同步
        }

        public bool CheckStoreSetting()
        {
            if (_setting == null) return false;
            if(_setting.StoreId==0 || string.IsNullOrEmpty(_setting.CDKey))
            {
                return false;
            }
            return true;
        }

        public void DownloadAccount()
        {           
            try
            {
                string url = string.Format("{0}/PosSync/QueryAccount?{1}", _serverUrl, BuildAccessToken());
                // 下载数据
                _log.Info("开始下载账户数据，请求{0}", url);
                var result = HttpHelper.HttpGet(url);
                if (!string.IsNullOrEmpty(result))
                {
                    var rows = JsonConvert.DeserializeObject<List<Account>>(result);
                    _log.Info("已下载账户数据{0}条", rows.Count);                    
                    var sql = @"Replace INTO Account (Id,UserName,Password,StoreId,Status,RoleId,NickName)VALUES (@Id,@UserName,@Password,@StoreId,@Status,@RoleId,@NickName)";
                    _db.ExecuteSql(sql, rows.ToArray());
                    _log.Info("结束账号数据下载");
                }
                else
                {
                    _log.Info("下载账号数据为空，下载失败.");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "下载数据失败");
            }           
        }

        public void DownloadProduct()
        {
            Console.WriteLine("product success");

            string url = string.Format("{0}/PosSync/QueryProduct?{1}", _serverUrl, BuildAccessToken());
            // 下载数据
            _log.Info("开始下载商品数据，请求{0}", url);
            var result = HttpHelper.HttpGet(url);
            if (!string.IsNullOrEmpty(result))
            {
                var rows = JsonConvert.DeserializeObject<List<Product>>(result);
                //入库   
                _log.Info("已下载商品数据{0}条", rows.Count);
                string sql = "Replace INTO Product (Id,Code,Name,BarCode,Specification,Unit,SalePrice,UpdatedOn) values (@Id,@Code,@Name,@BarCode,@Specification,@Unit,@SalePrice,@UpdatedOn)";
                _db.ExecuteSql(sql, rows.ToArray());
                _log.Info("结束下载商品数据");
            }
            else
            {
                _log.Info("下载数据为空，下载失败");
            }        

        }
        public void DownloadStore()
        {            
            try
            {
                // 下载数据
                string url = string.Format("{0}/PosSync/QueryStore?{1}", _serverUrl, BuildAccessToken());
                _log.Info("开始下载门店数据，请求{0}", url);
                var result = HttpHelper.HttpGet(url);
                if (!string.IsNullOrEmpty(result))
                {
                    var rows = JsonConvert.DeserializeObject<List<Store>>(result);
                    //入库 
                    _log.Info("已下载门店数据{0}条", rows.Count);
                    string sql = "Replace INTO Store (Id,Code,Name,LicenseCode)VALUES (@Id,@Code,@Name,@LicenseCode)";  
                    _db.ExecuteSql(sql, rows.ToArray());
                    _log.Info("结束下载门店数据");

                }
                else
                {
                    _log.Info("门店数据为空，下载失败");
                }

            }
            catch (Exception ex)
            {
                _log.Error(ex, "门店数据下载失败");
            }         
        }
        public void DownloadVipCard()
        {           
            try
            {
                // 下载数据
                string url = string.Format("{0}/PosSync/QueryVipCard?{1}", _serverUrl, BuildAccessToken());
                _log.Info("开始下载会员卡数据，请求{0}", url);
                var result = HttpHelper.HttpGet(url);
                if (!string.IsNullOrEmpty(result))
                {
                    var rows = JsonConvert.DeserializeObject<List<VipCard>>(result);  //入库 
                    _log.Info("已下载会员卡数据{0}条", rows.Count);
                    string sql = "Replace INTO VipCard (Id,Code,Discount)VALUES (@Id,@Code,@Discount)";                  
                    _db.ExecuteSql(sql, rows.ToArray());
                    _log.Info("结束下载会员卡数据");
                }
                else
                {
                    _log.Info("下载数据为空，下载会员卡数据失败");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "下载会员卡数据失败");
            }
          
        }
        public void DownloadVipProduct()
        {           
            try
            {
                // 下载数据
                string url = string.Format("{0}/PosSync/QueryVipProduct?{1}", _serverUrl, BuildAccessToken());
                _log.Info("开始下载会员商品数据，请求{0}", url);
                var result = HttpHelper.HttpGet(url);
                if (!string.IsNullOrEmpty(result))
                {
                    var rows = JsonConvert.DeserializeObject<List<VipProduct>>(result);  //入库 
                    _log.Info("已下载会员商品数据{0}条", rows.Count);
                    string sql = "Replace INTO VipProduct (Id,ProductId,SalePrice)VALUES (@Id,@ProductId,@SalePrice)";
                    var sqlDel = "delete from VipProduct";
                    _db.ExecuteSql(sqlDel, null);
                    _db.ExecuteSql(sql, rows.ToArray());
                    _log.Info("结束下载会员商品数据");
                }
                else
                {
                    _log.Info("下载数据为空，下载会员商品数据失败");
                }

            }
            catch (Exception ex)
            {
                _log.Error(ex, "下载会员商品数据失败");
            }
          
        }

        public void DownloadProductAreaPrice()
        {           
            try
            {

                // 下载数据
                string url = string.Format("{0}/PosSync/QueryProductAreaPrice?{1}", _serverUrl, BuildAccessToken());
                _log.Info("开始下载商品区域价数据，请求{0}", url);
                var result = HttpHelper.HttpGet(url);
                if (!string.IsNullOrEmpty(result))
                {
                    var rows = JsonConvert.DeserializeObject<List<ProductAreaPrice>>(result);  //入库 
                    _log.Info("已下载商品区域价数据{0}条", rows.Count);
                    string sql = "Replace INTO ProductAreaPrice (Id,ProductId,AreaId,SalePrice)VALUES (@Id,@ProductId,@AreaId,@SalePrice)";
                    var sqlDel = "delete from ProductAreaPrice";
                    _db.ExecuteSql(sqlDel, null);
                    _db.ExecuteSql(sql, rows.ToArray());
                    _log.Info("结束下载商品区域价数据");
                }
                else
                {
                    _log.Info("下载数据为空，下载商品区域价失败");
                }

            }
            catch (Exception ex)
            {
                _log.Error(ex, "下载失败");
            }
          
        }

        public void DownloadProductStorePrice()
        {         
            try
            {
                // 下载数据
                string url = string.Format("{0}/PosSync/QueryProductStorePrice?{1}", _serverUrl, BuildAccessToken());
                _log.Info("开始下载商品门店价数据，请求{0}", url);
                var result = HttpHelper.HttpGet(url);
                if (!string.IsNullOrEmpty(result))
                {
                    var rows = JsonConvert.DeserializeObject<List<ProductStorePrice>>(result);  //入库  
                    _log.Info("已下载商品门店价数据{0}条", rows.Count);
                    string sql = "Replace INTO ProductStorePrice (Id,ProductId,StoreId,SalePrice)VALUES (@Id,@ProductId,@StoreId,@SalePrice)";
                    var sqlDel = "delete from ProductStorePrice";
                    _db.ExecuteSql(sqlDel, null);
                    _db.ExecuteSql(sql, rows.ToArray());
                   
                    _log.Info("结束下载商品门店价数据");
                }
                else
                {
                    _log.Info("下载数据为空，下载商品门店价失败");
                }

            }
            catch (Exception ex)
            {
                _log.Error(ex, "下载商品门店价失败");
            }          
        }

        public void Send(SaleOrder model)
        {
            if (model.Items.Count == 0)
            {
                _log.Info("销售单{0},明细为空,终止上传", model.Code);
                return;
            }
            try
            {

                _log.Info("销售单{0},开始同步", model.Code);
                string url = string.Format("{0}/PosSync/SaleOrderSync", _serverUrl);
                var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                string body = JsonConvert.SerializeObject(model, dateTimeConverter);
                body = HttpHelper.UrlEncode(body); //Url编码
                string param = string.Format("{0}&body={1}", BuildAccessToken(), body);
                string result = HttpHelper.HttpPost(url, param);
                if (result == "1")
                {
                    // string sql = @"Update SaleOrder set IsSync=1 where @Id=@Id";
                    // _db.ExecuteSql(sql, new { Id = model.Id });
                    _log.Info("销售单{0},同步成功", model.Code);
                }
                else
                {
                    _log.Info("销售单{0},同步失败:{1}", model.Code, result);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);

            }
        }
        public void Send(WorkSchedule model)
        {
            try
            {
                _log.Info("班次{0},开始同步", model.Code);
                string url = string.Format("{0}/PosSync/WorkScheduleSync", _serverUrl);
                var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                string body = JsonConvert.SerializeObject(model, dateTimeConverter);
                body = HttpHelper.UrlEncode(body); //Url编码
                string param = string.Format("{0}&body={1}", BuildAccessToken(), body);
                string result = HttpHelper.HttpPost(url, param);
                if (result == "1")
                {
                    if (model.EndBy > 0 && model.CashAmount > 0)
                    {
                        string sql = @"Update WorkSchedule set IsSync=1 where @Id=@Id";
                        _db.ExecuteSql(sql, new { Id = model.Id });
                        _log.Info("班次{0}设置金额已同步", model.Code);
                    }
                    _log.Info("班次{0}数据已上传", model.Code);
                }
                else
                {
                    _log.Info("班次{0},同步失败:{1}", model.Code, result);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);

            }
        }
        

        /// <summary>
        /// 上传指定日期全部销售数据
        /// </summary>
        /// <param name="today">today  yyyy-MM-dd</param>
        public void SaleSyncDaily(DateTime today)
        {
            string sql = "select * from SaleOrder Where (Status =@Paid or Status=@Cancel) and date(updatedOn) =@SyncDate ";
            var result = _db.Query<SaleOrder>(sql, new { Paid = (int)SaleOrderStatus.Paid, Cancel = (int)SaleOrderStatus.Cancel, SyncDate = today.ToString("yyyy-MM-dd") });
            if (result.Count() == 0)
            {
                _log.Info("销售数据为空，终止上传");
                return;
            }
            foreach (var model in result)
            {
                string sqlitem = "select * from SaleOrderItem where SaleOrderId=@SaleOrderId";
                var items = _db.Query<SaleOrderItem>(sqlitem, new { SaleOrderId = model.Id }).ToList();
                model.Items = items;
                Send(model);
                Thread.Sleep(5);
            }
        }


        /// <summary>
        /// 上传销售对账
        /// </summary>
        /// <param name="today">上传日期： 格式：yyyy-MM-dd</param>
        public void UploadSaleSync(DateTime today)
        {
            string sql = @"select s.StoreId,s.PosId,date(updatedOn) as SaleDate,count(*) as orderCount,total(orderAmount) as OrderTotalAmount
 from saleorder s where date(updatedOn) = @UpdatedOn and Status in (-1,3) 
  group by s.StoreId,s.PosId, date(updatedOn) ";
            var rows = _db.Query<SaleSync>(sql, new { UpdatedOn = today.ToString("yyyy-MM-dd") }).ToList();
            if (rows.Count == 0)
            {
                _log.Info("销售数据为空，终止上传");
                return;
            }
            _log.Info("上传销售对账");
            string url = string.Format("{0}/PosSync/UpdateSaleSync", _serverUrl);
            var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            string body = JsonConvert.SerializeObject(rows, dateTimeConverter);
            string param = string.Format("{0}&body={1}",BuildAccessToken(), body);
            string result = HttpHelper.HttpPost(url, param);
            if (result == "1")
            {
                _log.Info("{0}销售对账上传成功", today.ToString("yyyy-MM-dd"));
            }
            else
            {
                _log.Info("{0}销售对账上传失败:{1}", today.ToString("yyyy-MM-dd"), result);
            }
        }

        /// <summary>
        /// 根据方法命名，下载数据
        /// </summary>
        /// <param name="methodName">方法名为空，表示下载所有</param>
        public void LoadDataByName(string methodName="")
        {
            var t = typeof(SyncService);
            if (string.IsNullOrEmpty(methodName))
            {
                var allDownloadFunctions =t.GetMethods().Where(n => n.Name.StartsWith("download", StringComparison.OrdinalIgnoreCase)).ToList();
                foreach (var mi in allDownloadFunctions)
                {
                    mi.Invoke(this, null);
                }
            }
            else {
                var mi = t.GetMethod("Download"+methodName);
                mi.Invoke(this, null);
            }           
        }
      
    }
}
