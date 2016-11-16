using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using EBS.WinPos.Domain.Entity;
using EBS.WinPos.Domain;
using System.Diagnostics;
using System.Data.SQLite;
namespace EBS.WinPos.Service
{
    public class SaleOrderService
    {
        Repository db;
        public SaleOrderService()
        {
            db = new Repository();
        }
        public double InsertRange()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            List<Account> list = new List<Account>();
           // db.ToggleAutoDeleteChangeOrSave();
            for (int i = 0; i < 10000; i++)
            {
                Account order = new Account()
                {
                    NickName = "nickName" + i.ToString(),
                    Password = DateTime.Now.Millisecond.ToString(),
                    RoleId = 1,
                    Status = 1,
                    StoreId = 1,
                    UserName = "userName" + i.ToString()

                };
                list.Add(order);
            }
            DbContextTransaction tran = db.Database.BeginTransaction();
            try
            {
               
                db.Accounts.AddRange(list);
                db.SaveChanges();
                tran.Commit();
            }
            catch (Exception)
            {

                tran.Rollback();
            }
           
           // db.ToggleAutoDeleteChangeOrSave();
            watch.Stop();

            return watch.Elapsed.TotalSeconds;
        }

        public void Create(SaleOrder model)
        {

        }
    }
}
