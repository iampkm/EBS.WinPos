using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EBS.WinPos.Domain;
using EBS.WinPos.Domain.Entity;
namespace EBS.WinPos.Service
{
   public class VipCardService
    {
       // Repository _db;
       DapperContext _query;
        public VipCardService()
        {
          //  _db = new Repository();
            _query = new DapperContext();
        }

        public VipCard GetByCode(string code)
        {
            string sql = "select * from VipCard where Code=@Code";
            return _query.First<VipCard>(sql, new { Code = code });
           // return  _db.VipCards.FirstOrDefault<VipCard>(n => n.Code == code);
        }
    }
}
