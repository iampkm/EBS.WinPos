using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EBS.WinPos.Domain.Entity;
using System.Windows.Forms;
namespace EBS.WinPos
{
    public static class ContextService
    {
        public static void SetCurrentAccount(Account account,int storeId,int posId)
        {
            CurrentAccount = account;
            StoreId = storeId;
            PosId = posId;

        }
        public static void SignOut()
        {
            CurrentAccount = null;           
        }
       
        /// <summary>
        /// 当前账户
        /// </summary>
        public static Account CurrentAccount { get; private set; }

        /// <summary>
        /// 当前门店编号，此编号作为记录门店销售的依据，而不根据账号所属门店ID 来
        /// </summary>
        public static int StoreId { get;private set; }
        /// <summary>
        /// 当前pos机编号
        /// </summary>
        public static int PosId { get; private set; }
    }
}
