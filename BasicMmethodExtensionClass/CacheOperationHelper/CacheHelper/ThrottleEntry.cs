using System;

namespace BasicMmethodExtensionClass.CacheOperationHelper.CacheHelper
{
    /// <summary>
    /// 调节实体
    /// </summary>
    public class ThrottleEntry
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime PeriodStart { get; set; }

        /// <summary>
        /// 请求
        /// </summary>
        public long Requests { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ThrottleEntry()
        {
            PeriodStart = DateTime.UtcNow;
            Requests = 0;
        }
    }
}