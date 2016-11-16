using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using EBS.Infrastructure.Extension;
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

        public int Status { get; set; }

        /// <summary>
        /// 加密密码
        /// </summary>
        public void EncryptionPassword()
        {
            MD5 md5Prider = MD5.Create();
            this.Password = md5Prider.GetMd5Hash(this.Password);
        }
        public bool VerifyPassword(string password)
        {           
            MD5 md5Prider = MD5.Create();
            return md5Prider.VerifyMd5Hash(password, this.Password);           
        }
    }
}
