using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EBS.WinPos.Domain;
using EBS.WinPos.Domain.Entity;
namespace EBS.WinPos.Service
{
    public class VipProductService
    {
       // Repository _db;
        DapperContext _query;
        public VipProductService()
        {
          //  _db = new Repository();
            _query = new DapperContext();
        }

        public VipProduct GetByProductId(int id)
        {
            string sql = "select * from VipProduct where ProductId= @ProductId";
            return _query.First<VipProduct>(sql, new { ProductId = id });
        }
    }
}
