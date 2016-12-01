using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EBS.WinPos.Domain;
using EBS.WinPos.Domain.Entity;
namespace EBS.WinPos.Service
{
    
   public class StoreService
    {
       DapperContext _query;
       public StoreService()
       {
           _query = new DapperContext();
       }

       public List<Store> GetAll()
       {
           string sql = "select * from Store";
          return _query.Query<Store>(sql, null).ToList();
       }
    }
}
