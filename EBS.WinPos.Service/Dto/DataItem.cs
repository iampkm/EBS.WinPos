using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBS.WinPos.Service.Dto
{
   public class DataItem
    {
       public DataItem(string displayName, string value)
       {
           this.DisplayName = displayName;
           this.Value = value;
       }
       public string DisplayName { get; set; }

       public string Value { get; set; }
    }
}
