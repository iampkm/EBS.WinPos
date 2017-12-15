using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBS.WinPos.Service.WxPayAPI
{
   public class MicropayMock:IMicropay
    {
        public void Run(string orderCode, string body, int total_fee, string auth_code)
        {
            
        }
    }
}
