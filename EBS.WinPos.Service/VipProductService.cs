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
        Repository _db;
        public VipProductService()
        {
            _db = new Repository();
        }

        public VipProduct GetByProductId(int id)
        {
           return  _db.VipProducts.FirstOrDefault(n => n.ProductId == id);
        }
    }
}
