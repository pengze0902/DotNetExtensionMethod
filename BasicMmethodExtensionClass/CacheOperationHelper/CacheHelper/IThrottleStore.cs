using Helper.CacheHelper;

namespace BasicMmethodExtensionClass.CacheOperationHelper.CacheHelper
{
    public interface IThrottleStore
    {
        /// <summary>
        /// 试图获取值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        bool TryGetValue(string key, out ThrottleEntry entry);

        /// <summary>
        /// 增量请求
        /// </summary>
        /// <param name="key"></param>
        void IncrementRequests(string key);

        /// <summary>
        /// 反转
        /// </summary>
        /// <param name="key"></param>
        void Rollover(string key);

        /// <summary>
        /// 清除
        /// </summary>
        void Clear();
    }
}
