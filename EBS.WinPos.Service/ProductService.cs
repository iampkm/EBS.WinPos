using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EBS.WinPos.Domain;
using EBS.WinPos.Domain.Entity;
using EBS.Infrastructure;
using EBS.WinPos.Service.Dto;
namespace EBS.WinPos.Service
{
   public class ProductService
    {
       Repository _db;
       DapperContext _query;
       public ProductService()
       {
           _db = new Repository();
           _query = new DapperContext();
       }

       public Product GetProduct(string productCodeOrBarCode)
       {
           // 查找区域价格，门店价格，优先级按照 门店价 > 区域价 > 基础售价 
          // var model = _db.Products.FirstOrDefault(n => n.Code == productCodeOrBarCode || n.BarCode == productCodeOrBarCode);
           string sql = "select * from Product where Code=@ProductCodeOrBarCode Or BarCode=@ProductCodeOrBarCode";
           var model = _query.First<Product>(sql, new { ProductCodeOrBarCode = productCodeOrBarCode });
           if (model == null)
           {
               return null;
           }
           //区域价
           string sqlArea = "select * from ProductAreaPrice where ProductId=@ProductId";
           var areaProduct = _query.First<Product>(sqlArea, new { ProductId = model.Id });
           if (areaProduct != null)
           {
               model.SalePrice = areaProduct.SalePrice;
           }
           //门店价
           var sqlStore = "select * from ProductStorePrice where ProductId=@ProductId";
           var storeProduct = _query.First<Product>(sqlStore, new { ProductId = model.Id });
           if (storeProduct != null)
           {
               model.SalePrice = storeProduct.SalePrice;
           }
           return model;
       }

       public ProductPriceDto QueryProductPrice(string productCodeOrBarCode)
       {
           string sql = @"select p.Id,p.name,p.Code,p.BarCode,p.Specification,p.Unit,p.salePrice,a.SalePrice as AreaSalePrice,s.SalePrice as StoreSalePrice
,v.SalePrice as VipSalePrice from product p
left join productareaprice a on p.Id = a.ProductId
left join productstoreprice s on p.Id =s.ProductId
left join vipproduct v on p.Id = v.ProductId where p.Code=@ProductCodeOrBarCode Or p.BarCode=@ProductCodeOrBarCode";
           var model = _query.First<ProductPriceDto>(sql, new { ProductCodeOrBarCode = productCodeOrBarCode });
           return model;
       }
  
    }
}
