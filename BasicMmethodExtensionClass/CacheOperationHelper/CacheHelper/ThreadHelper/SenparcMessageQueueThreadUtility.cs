using System;
using System.Diagnostics;
using System.Threading;

namespace BasicMmethodExtensionClass.CacheOperationHelper.CacheHelper.ThreadHelper
{
    /// <summary>
    /// SenparcMessageQueue线程自动处理
    /// </summary>
    public class SenparcMessageQueueThreadUtility
    {
        private readonly int _sleepMilliSeconds;


        public SenparcMessageQueueThreadUtility(int sleepMilliSeconds = 2000)
        {
            _sleepMilliSeconds = sleepMilliSeconds;
        }

        /// <summary>
        /// 析构函数，将未处理的列队处理掉
        /// </summary>
        public SenparcMessageQueueThreadUtility()
        {
            try
            {
                var mq = new SenparcMessageQueue();
                Trace.WriteLine("SenparcMessageQueueThreadUtility执行析构函数");
                Trace.WriteLine(string.Format("当前列队数量：{0}", mq.GetCount()));

                SenparcMessageQueue.OperateQueue();//处理列队
            }
            catch (Exception ex)
            {
                //此处可以添加日志
                Trace.WriteLine(string.Format("SenparcMessageQueueThreadUtility执行析构函数错误：{0}", ex.Message));
            }
        }

        /// <summary>
        /// 启动线程轮询
        /// </summary>
        public void Run()
        {
            do
            {
                SenparcMessageQueue.OperateQueue();
                Thread.Sleep(_sleepMilliSeconds);
            } while (true);
        }
    }
}
