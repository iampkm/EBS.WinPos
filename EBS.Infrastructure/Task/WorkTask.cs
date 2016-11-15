using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBS.Infrastructure.Task
{
   public class WorkTask
    {
        public string Name { get; private set; }

        public ITask Task { get; private set; }

        public ITaskTrigger TaskTrigger { get; private set; }
        public WorkTask(string name,ITask task, ITaskTrigger taskTrigger)
        {
            this.Name = name;
            this.TaskTrigger = taskTrigger;
            this.Task = task;
        }
    }
}
