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

        static Dictionary<Type, Form> _fromDic = new Dictionary<Type, Form>();
        public static void SetCurrentAccount(Account account,int storeId,int posId)
        {
            CurrentAccount = account;
            StoreId = storeId;
            PosId = posId;

        }
        public static void SignOut()
        {
            CurrentAccount = null;
            _fromDic.Clear();
        }

        public static void AddFrom(Form form)
        {
            var formType = form.GetType();
            if (!_fromDic.ContainsKey(formType))
            {
                _fromDic.Add(formType, form);
            }
        }
        public static void RemoveFrom(Type formType)
        {
            if (_fromDic.ContainsKey(formType))
            {
                _fromDic.Remove(formType);
            }
        }

        public static Form GetFrom(Type formType)
        {
            return _fromDic.ContainsKey(formType) ? _fromDic[formType] : null;
        }

        /// <summary>
        /// 父容器窗体
        /// </summary>
        public static frmMain ParentForm
        {
            get
            {
                if (_fromDic.ContainsKey(typeof(frmMain)))
                {
                    return (frmMain)_fromDic[typeof(frmMain)];
                }
                else
                {
                    return null;
                }
            }
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
