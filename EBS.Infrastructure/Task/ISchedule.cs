using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBS.Infrastructure.Task
{
   public interface ISchedule
    {
       IList<WorkTask> GetTasks();
    }
}
