using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BasicMmethodExtensionClass.CacheOperationHelper.CacheHelper.ThreadHelper
{
    /// <summary>
    /// 线程处理类
    /// </summary>
    public static class ThreadUtility
    {
        /// <summary>
        /// 异步线程容器
        /// </summary>
        public static Dictionary<string, Thread> AsynThreadCollection = new Dictionary<string, Thread>();//后台运行线程

        /// <summary>
        /// 注册线程
        /// </summary>
        public static void Register()
        {
            if (AsynThreadCollection.Count != 0) return;
            {
                var senparcMessageQueue = new SenparcMessageQueueThreadUtility();
                var senparcMessageQueueThread = new Thread(senparcMessageQueue.Run) { Name = "SenparcMessageQueue" };
                if(senparcMessageQueueThread.Name==null)
                    throw new ArgumentNullException(nameof("senparcMessageQueueThread.Name"));
                AsynThreadCollection.Add(senparcMessageQueueThread.Name, senparcMessageQueueThread);
            }

            AsynThreadCollection.Values.ToList().ForEach(z =>
            {
                z.IsBackground = true;
                z.Start();
            });//全部运行
        }

        private static string nameof(string p)
        {
            throw new NotImplementedException();
        }
    }
}
