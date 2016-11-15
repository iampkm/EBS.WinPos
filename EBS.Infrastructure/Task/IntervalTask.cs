using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EBS.Infrastructure.Extension;
namespace EBS.Infrastructure.Task
{
   public class IntervalTask :ITaskTrigger
    {
        public IList<DayOfWeek> WeekDays { get; private set; }
        public int Seconds { get; private set; }

        public DateTime BeginTime { get; private set; }

        public DateTime EndTime { get; private set; }
        public IntervalTask(string weekday, string seconds,string beginTime,string endTime)
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
            this.Seconds = int.Parse(seconds);
            this.BeginTime = DateTime.Parse("00:00:00");
            this.EndTime = DateTime.Parse("23:59:59");
            if (!string.IsNullOrEmpty(beginTime)) {
                this.BeginTime = DateTime.Parse(beginTime);
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                this.EndTime = DateTime.Parse(endTime);
            }           
        }

        public bool Trigger(DateTime when)
        {           
            int beginSeconds = BeginTime.Hour * 60 * 60 + BeginTime.Minute * 60 + BeginTime.Second;
            int endSeconds = EndTime.Hour * 60 * 60 + EndTime.Minute * 60 + EndTime.Second;
            int totalSeconds = when.Hour * 60 * 60 + when.Minute * 60 + when.Second;
            //不在限制范围内的，直接跳过
            if (totalSeconds < beginSeconds || totalSeconds > endSeconds) 
            {              
                return false;
            }
            if ((WeekDays.Contains(when.DayOfWeek) || WeekDays.Count == 0) && totalSeconds % this.Seconds == 0)
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
