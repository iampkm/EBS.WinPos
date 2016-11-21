using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using EBS.WinPos.Domain.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
namespace EBS.WinPos.Domain
{
    public class Repository : DbContext
    {
        public DbSet<SaleOrder> Orders { get; set; }
        public DbSet<SaleOrderItem> OrderItems { get; set; }
        public DbSet<Account> Accounts { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<WorkSchedule> WorkSchedules { get; set; }

        public DbSet<PaidHistory> PaidHistorys { get; set; }
        public DbSet<VipCard> VipCards { get; set; }

        public DbSet<Store> Stores { get; set; }
        


        public Repository()
            : base("SqliteTest")
        {
            System.Diagnostics.Debug.WriteLine("--实例化DbContext--");
            Database.Log = x => { System.Diagnostics.Debug.WriteLine(x); };

           
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
            
        }

        public void ToggleAutoDeleteChangeOrSave()
        {
            base.Configuration.AutoDetectChangesEnabled = !base.Configuration.AutoDetectChangesEnabled;
            base.Configuration.ValidateOnSaveEnabled = !base.Configuration.AutoDetectChangesEnabled;
        }
    }
}
