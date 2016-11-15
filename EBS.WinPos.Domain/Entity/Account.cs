using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBS.WinPos.Domain.Entity
{
   public class Account:BaseEntity
    {
        public string UserName { get; set; }
        /// <summary>
        /// MD5 加密密码
        /// </summary>
        public string Password { get; set; }

        public string NickName { get; set; }

        public int RoleId { get; set; }
        /// <summary>
        /// 门店，值为 0
        /// </summary>
        public int StoreId { get; set; }

        public int Status { get; private set; }
    }
}
