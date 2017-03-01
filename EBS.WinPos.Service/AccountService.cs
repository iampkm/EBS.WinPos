using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EBS.WinPos.Domain;
using EBS.WinPos.Domain.Entity;
using EBS.Infrastructure;
namespace EBS.WinPos.Service
{
   public class AccountService
    {
       Repository _db;
       DapperContext _query;
       public AccountService()
       {
           _db = new Repository();
           _query = new DapperContext();
       }

       public Account Login(string userName, string passwrod)
       {
           if (userName == "") { throw new AppException("用户名为空"); }
           if (passwrod == "") { throw new AppException("密码为空"); }
           int userId = 0;
           int.TryParse(userName, out userId);       
          // var model = _db.Accounts.FirstOrDefault(n => n.Id == userId || n.UserName == userName);
           var model = _query.First<Account>("select * from Account where Id=@Id or UserName=@UserName", new { Id = userId, UserName = userName });
         
           if(model==null||!model.VerifyPassword(passwrod))
           {
             throw new AppException("用户名或密码错误"); 
           }
           if (model.Status != 1)
           {
               throw new AppException("该账号已被禁用"); 
           }
           return model;           
       }
    }
}
