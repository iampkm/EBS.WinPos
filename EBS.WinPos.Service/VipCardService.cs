using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EBS.WinPos.Domain;
using EBS.WinPos.Domain.Entity;
namespace EBS.WinPos.Service
{
   public class VipCardService
    {
        Repository _db;
        public VipCardService()
        {
            _db = new Repository();
        }

        public VipCard GetByCode(string code)
        {
            return  _db.VipCards.FirstOrDefault<VipCard>(n => n.Code == code);
        }
    }
}
