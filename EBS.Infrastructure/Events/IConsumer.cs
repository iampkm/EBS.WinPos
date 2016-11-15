﻿
namespace EBS.Infrastructure.Events
{
    /// <summary>
    /// 消费者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConsumer<T>
    {
        void HandleEvent(T eventMessage);
    }
}
