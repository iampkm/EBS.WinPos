using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
//using System.Threading;
namespace EBS.Infrastructure.Task
{
    public class ScheduleContext
    {
     
       static Timer _timer;
       static ISchedule _schedule;
        /// <summary>
        /// 任务配置文件Task.config根路径
        /// </summary>
       public static string TaskConfigPath;
       public static void Start(){
           if (string.IsNullOrEmpty(TaskConfigPath)) { throw new Exception("自动任务配置文件根路径未配置TaskConfigPath"); }
           _schedule = new DefaultSchedule(ScheduleContext.TaskConfigPath);
          
           _timer = new Timer(1000);
           _timer.Elapsed += timer_Elapsed;
           _timer.Enabled = true;
       }

       static void timer_Elapsed(object sender, ElapsedEventArgs e)
       {
           foreach (var item in _schedule.GetTasks())
           {            
               if (item.TaskTrigger.Trigger(e.SignalTime))
               {
                   System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(item.Task.Execute));
                   thread.Start();
               }
           }           
       }

       public static void Close()
       {
           _timer.Enabled = false;
           _timer = null;
       }
    }
}
