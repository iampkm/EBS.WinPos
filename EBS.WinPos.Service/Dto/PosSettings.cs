using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EBS.WinPos.Service.Dto
{
   public class PosSettings
    {
        /// <summary>
        /// 当前门店
        /// </summary>
        [DisplayNameAttribute("CommonSetting.Store.StoreId")]
       public int StoreId { get; set; }
        /// <summary>
        /// pos机编号
        /// </summary>
        [DisplayNameAttribute("CommonSetting.Store.PosId")]

       public int PosId { get; set; }
    }
}
