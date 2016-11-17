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
           return model;
       }
    }
}
