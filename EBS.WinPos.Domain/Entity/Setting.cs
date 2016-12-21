using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBS.WinPos.Domain.Entity
{
    /// <summary>
    /// 系统参数设置
    /// </summary>
   public class Setting:BaseEntity
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 键
        /// </summary>
        public string Key { get; set; }       
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }
}
