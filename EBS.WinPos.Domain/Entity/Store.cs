using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBS.WinPos.Domain.Entity
{
   public class Store:BaseEntity
    {
       public string Code { get; set; }

       public string Name { get; set; }
    }
}
