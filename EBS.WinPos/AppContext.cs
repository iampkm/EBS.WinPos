using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using EBS.Infrastructure.Log;
namespace EBS.WinPos
{
    public static class AppContext
    {
        private static EBS.Infrastructure.Log.ILogger _log;

        static AppContext()
        {
            _log = new NLogWriter(NLog.LogManager.GetCurrentClassLogger());
        }
        public static EBS.Infrastructure.Log.ILogger Log
        {
            get
            {
                return _log;
            }
        }
    }
}
