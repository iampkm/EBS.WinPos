using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using EBS.Infrastructure.Log;
using EBS.Infrastructure.Task;
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

        public static void CheckVersion()
        {
            // 开启自动更新
            EBS.AutoUpdater.AutoUpdateService updateService = new AutoUpdater.AutoUpdateService();
            updateService.CheckUpdate();
        }
        /// <summary>
        /// 下载 POS 数据
        /// </summary>
        public static void DownloadPosDB()
        { 
            //下载  账户，门店，商品，会员卡，会员商品数据
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
    }
}
