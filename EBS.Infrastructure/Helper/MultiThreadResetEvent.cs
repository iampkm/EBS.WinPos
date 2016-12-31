using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace EBS.Infrastructure.Helper
{
    /// <summary>
    /// 多线程信号等待
    /// </summary>
    public class MultiThreadResetEvent : IDisposable
    {
        private readonly ManualResetEvent done;
        private readonly int total;
        private long current;

        /// <summary>
        /// 监视total个线程执行（线程数固定，可以超过64个）
        /// </summary>
        /// <param name="total">需要等待执行的线程总数</param>
        public MultiThreadResetEvent(int total)
        {
            this.total = total;
            current = total;
            done = new ManualResetEvent(false);
        }

        /// <summary>
        /// 线程数不固定，监视任意线程数时
        /// </summary>
        public MultiThreadResetEvent()
        {          
            done = new ManualResetEvent(false);
        }

        /// <summary>
        /// 加入一个要等待的线程信号
        /// </summary>
        public void addWaitOne()
        {
            Interlocked.Increment(ref current);
        }
 
        /// <summary>
        /// 唤醒一个等待的线程
        /// </summary>
        public void Set()
        {
          
            // Interlocked 原子操作类 ,此处将计数器减1
            if (Interlocked.Decrement(ref current) == 0)
            {
                //当所以等待线程执行完毕时，唤醒等待的线程
                done.Set();
            }
        }
 
        /// <summary>
        /// 等待所以线程执行完毕
        /// </summary>
        public void WaitAll()
        {
            done.WaitOne(); 
        }
 
        /// <summary>
        /// 释放对象占用的空间
        /// </summary>
        public void Dispose()
        {
            ((IDisposable)done).Dispose();
        }
    }
}
