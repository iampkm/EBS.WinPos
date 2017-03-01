using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using EBS.Infrastructure.Log;
using EBS.Infrastructure.Task;
namespace EBS.Infrastructure
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
        /// <summary>
        /// 启动后台任务
        /// </summary>
        public static void StartTask()
        {
            ScheduleContext.TaskConfigPath = "Task.Config";
            ScheduleContext.Start();
        }

        // 服务器数据变动，写入同步表
        //  SyncData
        // Id,  TableName,KeyId,CreatedOn 

        public static void CloseTask()
        {
            ScheduleContext.Close();
        }
    }
}
