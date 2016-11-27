using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EBS.Infrastructure.Events;
namespace EBS.WinPos.Domain.Event
{
   public class EventHander:IConsumer<FinishOrderEvent>
    {
        public void HandleEvent(FinishOrderEvent eventMessage)
        {
           // 发布消息到服务器

            // 如果失败，写入失败记录表
        }
    }
}
