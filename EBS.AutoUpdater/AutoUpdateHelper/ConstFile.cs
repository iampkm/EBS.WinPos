using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBS.AutoUpdater
{
    public class ConstFile
    {
        public const string TEMPFOLDERNAME = "TempFolder";
        public const string CONFIGFILEKEY = "config_";
        public const string FILENAME = "AutoUpdater.config";
        public const string ROOLBACKFILE = "EBS.WinPos.exe";
        public const string MESSAGETITLE = "自动升级程序";
        public const string CANCELORNOT = "更新程序正在运行. 你确定要取消吗?";
        public const string APPLYTHEUPDATE = "程序需要重新启动才能进行升级,请点击确定开始升级!";
        public const string NOTNETWORK = "程序更新未成功，程序将自动重启，请重新开始升级.";
    }
}
