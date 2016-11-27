using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EBS.WinPos.Domain;
using EBS.WinPos.Domain.Entity;
namespace EBS.WinPos.Service
{
   public class ProductService
    {
       Repository _db;
       public ProductService()
       {
           _db = new Repository();
       }

       public Product GetProduct(string productCodeOrBarCode)
       {
           var model = _db.Products.Where(n => n.Code == productCodeOrBarCode || n.BarCode == productCodeOrBarCode).FirstOrDefault();
            // 查找区域价格，门店价格，优先级按照 门店价 > 区域价 > 基础售价  
           return model;
       }
    }
}
