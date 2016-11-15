using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
namespace EBS.Infrastructure.Log
{
   public class NLogWriter :ILogger
    {

       NLog.ILogger _log;
       public NLogWriter( NLog.ILogger log)
       {
           this._log = log;
       }

       public void Info(string message)
       {
           this._log.Info(message);
       }
       public void Info(string formateMessage, params object[] args)
       {
           this._log.Info(formateMessage, args);
       }
       public void Error(string message)
       {
           this._log.Error(message);
       }
       public void Error(Exception exception, string message)
       {
         
           this._log.Error(exception, message,null);
       }
       public void Error(Exception exception, string formateMessage, params object[] args)
       {
           this._log.Error(exception,formateMessage,args);
       }
       public void Error(Exception exception)
       {
           this._log.Error( exception,"原因:{0},堆栈：{1}",exception.Message,exception.StackTrace);
           if (exception.InnerException != null)
           {
               var ex = exception.InnerException;
               this._log.Error(ex, "内部异常原因:{0},堆栈：{1}", ex.Message, ex.StackTrace); 
           }
       }
    }
}
