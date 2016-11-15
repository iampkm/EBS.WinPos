using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Reflection;
namespace EBS.Infrastructure.Task
{
    public class DefaultSchedule : ISchedule
    {
        private string _Path;
        private FileInfo _config;
        IList<WorkTask> _tasks;
        public DefaultSchedule(string configPath)
        {
            this._Path = configPath;
            _config = new FileInfo(configPath);
            _tasks = LoadConfig();

        }
        public IList<WorkTask> GetTasks()
        {
            if (ConfigChanged())
            {
                this._tasks = LoadConfig();
            }
            return this._tasks;
        }
        private bool ConfigChanged()
        {
            bool result = false;
            FileInfo file = new FileInfo(_Path);
            if (this._config.LastWriteTime != file.LastWriteTime)
            {
                this._config = file;
                result = true;
            }
            return result;
        }
        private IList<WorkTask> LoadConfig()
        {
            IList<WorkTask> taskList = new List<WorkTask>();
            using (FileStream fs = new FileStream(this._Path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                XDocument doc = XDocument.Load(fs);
                var tasks = doc.Document.Element("Schedule").Elements("Task");
                foreach (var task in tasks)
                {
                    string running = task.Attribute("running").Value;
                    if (running.ToLower() != "true")
                    {
                        continue;  // 停止的不加入列表
                    }

                    string name = task.Attribute("name").Value;
                    string type = task.Attribute("type").Value;
                    string assmeblyName = task.Attribute("assembly").Value;
                    Type taskType = Assembly.Load(assmeblyName).GetType(type);
                    ITask itask = Activator.CreateInstance(taskType) as ITask;
                    // 获取触发节点
                    string method = task.Attribute("method").Value;
                    if (!string.IsNullOrEmpty(method)) { method = method.ToLower(); }
                    foreach (var triggerItem in task.Elements("Trigger"))
                    {                       
                        string weekday = triggerItem.Attribute("weekday") == null ? "" : triggerItem.Attribute("weekday").Value;                        
                        if (method == "interval")
                        {
                            string seconds = triggerItem.Attribute("seconds").Value;
                            string beginTime = triggerItem.Attribute("beginTime") == null ? "" : triggerItem.Attribute("beginTime").Value;
                            string endTime = triggerItem.Attribute("endTime") == null ? "" : triggerItem.Attribute("endTime").Value;   
                            taskList.Add(new WorkTask(name, itask, new IntervalTask(weekday, seconds,beginTime,endTime)));
                        }
                        else if (method == "schedule")
                        {
                            string time = triggerItem.Attribute("time").Value;
                            taskList.Add(new WorkTask(name, itask, new ScheduleTask(weekday, time)));
                        }
                        else
                        {
                            throw new Exception("触发方式method配置不正确");
                        }
                    }  
                   
                }
            }
            return taskList;
        }
    }
}

/*

<Schedule>
   <Trigger time='00:00:00' weekday='1,2,3,4,5,6,7'>
 * </Trigger>
   <Tasks>
      <Task name='' Type=''/>
 *  <Task name='' Type=''/>
 *   <Task name='' Type=''/>
   </Tasks>
</Schedule>
 * <Schedule>
   <Trigger time='00:00:00' weekday='1,2,3,4,5,6,7'>
 * </Trigger>
   <Tasks>
      <Task name='' Type=''/>
 *  <Task name='' Type=''/>
 *   <Task name='' Type=''/>
   </Tasks>
</Schedule>




*/