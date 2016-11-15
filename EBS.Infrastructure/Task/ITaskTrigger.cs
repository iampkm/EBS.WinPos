using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBS.Infrastructure.Task
{
    public interface ITaskTrigger
    {
        /// <summary>
        /// 触发
        /// </summary>
        /// <param name="when"></param>
        /// <returns></returns>
           bool Trigger(DateTime when);
    }
}
