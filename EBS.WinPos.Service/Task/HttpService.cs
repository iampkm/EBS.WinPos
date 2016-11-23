using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using EBS.Infrastructure.Helper;
using EBS.WinPos.Domain.Entity;
using EBS.WinPos.Domain;
using Newtonsoft.Json;
namespace EBS.WinPos.Service.Task
{
    public class HttpService
    {
        string _serverUrl;
      
        int pageSize = 1000;
        public HttpService()
        {
            _serverUrl = "http://www.cqlezhan.com/";
            //http://112.74.83.55/Home/DashBoard
        }
        public void DownloadData()
        {

            //启动线程池来进行多线程下载数据

            ThreadPool.QueueUserWorkItem(new WaitCallback(DownloadAccount));
           // ThreadPool.QueueUserWorkItem(new WaitCallback(DownloadStore));
        }
        public void DownloadAccount(object table)
        {           
            try
            {
                int count = 0;
                do
                {
                    int pageIndex = 1;
                    string url = string.Format("{0}/PosSync/QueryAccountSync?pageSize={1}&pageIndex={2}", _serverUrl, pageSize, pageIndex);
                    // 下载数据
                    var result = HttpHelper.HttpGet(url);
                    var rows = JsonConvert.DeserializeObject<IEnumerable<Account>>(result);
                    //入库
                    DapperContext db = new DapperContext();
                    string sql = "INSERT INTO Account (Id,UserName,Password,StoreId,Status,RoleId,NickName)VALUES (@Id,@UserName,@Password,@StoreId,@Status,@RoleId,@NickName)";
                    db.ExecuteSql(sql, rows.ToArray());

                    count = rows.Count();
                    pageIndex += 1;
                }
                while (count == pageSize);
            }
            catch (Exception ex)
            {                
                throw;
            }
        }
        public void DownloadStore(object table)
        {
            try
            {
                int count = 0;
                do
                {
                    int pageIndex = 1;
                    string url = string.Format("{0}/PosSync/QueryStoreSync?pageSize={1}&pageIndex={2}", _serverUrl, pageSize, pageIndex);
                    // 下载数据
                    var result = HttpHelper.HttpGet(url);
                    var rows = JsonConvert.DeserializeObject<IEnumerable<Store>>(result);
                    //入库
                    DapperContext db = new DapperContext();
                    string sql = "INSERT INTO Store (Id,Code,Name)VALUES (@Id,@Code,@Name)";
                    db.ExecuteSql(sql, rows.ToArray());

                    count = rows.Count();
                    pageIndex += 1;
                }
                while (count == pageSize);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
