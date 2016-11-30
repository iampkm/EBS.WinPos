using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using EBS.Infrastructure.Extension;
namespace EBS.WinPos.Domain.Entity
{
   public class Store:BaseEntity
    {
       public string Code { get; set; }

       public string Name { get; set; }
       /// <summary>
       /// 店长授权码
       /// </summary>
       public string LicenseCode { get; set; }

       /// <summary>
       /// 加密密码
       /// </summary>
       public void EncryptionLicenseCode()
       {
           MD5 md5Prider = MD5.Create();
           this.LicenseCode = md5Prider.GetMd5Hash(this.LicenseCode);
       }
       /// <summary>
       /// 验证授权码
       /// </summary>
       /// <param name="licenseCode"></param>
       /// <returns></returns>
       public bool VerifyLicenseCode(string licenseCode)
       {
           MD5 md5Prider = MD5.Create();
           return md5Prider.VerifyMd5Hash(licenseCode, this.LicenseCode);
       }
    }
}
