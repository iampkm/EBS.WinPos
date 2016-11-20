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
       public static void SetCurrentAccount(Account account)
       {
           CurrentAccount = account;
       }

       public static void SignOut()
       {
           CurrentAccount = null;
       }

       public static void AddFrom(Form form)
       {
           var formType = form.GetType();
           if (!_fromDic.ContainsKey(formType))
           { 
              _fromDic.Add(formType,form);
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
       public static frmMain ParentForm { get {
           return (frmMain)_fromDic[typeof(frmMain)];
       } }
       /// <summary>
       /// 当前账户
       /// </summary>
       public static Account CurrentAccount { get; private set; }
    }
}
