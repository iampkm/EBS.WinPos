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
       public AccountService()
       {
           _db = new Repository();
       }

       public Account Login(string userName, string passwrod)
       {
           if (userName == "") { throw new AppException("用户名为空"); }
           if (passwrod == "") { throw new AppException("密码为空"); }
           int userId = 0;
           int.TryParse(userName, out userId);       
           var model = _db.Accounts.FirstOrDefault(n => n.Id == userId || n.UserName == userName);
         
           if(model==null||!model.VerifyPassword(passwrod))
           {
             throw new AppException("用户名或密码错误"); 
           }
           return model;           
       }
    }
}
