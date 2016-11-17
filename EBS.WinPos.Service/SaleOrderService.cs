using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using EBS.WinPos.Domain.Entity;
using EBS.WinPos.Domain;
using System.Diagnostics;
using System.Data.SQLite;
using EBS.WinPos.Service.Dto;
namespace EBS.WinPos.Service
{
    public class SaleOrderService
    {
        Repository _db;
        public SaleOrderService()
        {
            _db = new Repository();
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
            DbContextTransaction tran = _db.Database.BeginTransaction();
            try
            {
               
                _db.Accounts.AddRange(list);
                _db.SaveChanges();
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

        public SaleOrder CreateOrder(ShopCart cat)
        {
            SaleOrder order = new SaleOrder()
            {
                StoreId = cat.StoreId,
                CreatedBy = cat.Editor,
                UpdatedBy = cat.Editor,
            };
            order.GenerateNewCode();
            foreach (ShopCartItem item in cat.Items)
            {
                order.AddOrderItem(item.Product, item.Quantity);
            }
            this._db.Orders.Add(order);
            this._db.SaveChanges();
            return order;
        }
    }
}
