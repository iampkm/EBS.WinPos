using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EBS.Infrastructure.Extension;
namespace EBS.Infrastructure.Task
{
    public class ScheduleTask : ITaskTrigger
    {
        public IList<DayOfWeek> WeekDays { get; private set; }
        public DateTime Time { get; private set; }  
        public ScheduleTask(string weekday, string time)
        {
           
            WeekDays = new List<DayOfWeek>();
            if (!string.IsNullOrWhiteSpace(weekday))
            {
                foreach (var day in weekday.Split(','))
                {
                    int intDay = int.Parse(day);
                    DayOfWeek dayofweek = intDay.ToEnumByValue<DayOfWeek>();
                    WeekDays.Add(dayofweek);
                }
            }
            this.Time = DateTime.Parse(time);
        }

        public bool Trigger(DateTime when)
        { 

            if ((WeekDays.Contains(when.DayOfWeek) || WeekDays.Count == 0) && when.Hour == Time.Hour && when.Minute == Time.Minute && when.Second == Time.Second)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
