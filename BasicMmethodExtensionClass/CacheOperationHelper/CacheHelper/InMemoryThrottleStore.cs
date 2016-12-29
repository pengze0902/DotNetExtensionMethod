using System.Collections.Concurrent;
using Helper.CacheHelper;

namespace BasicMmethodExtensionClass.CacheOperationHelper.CacheHelper
{
    public class InMemoryThrottleStore : IThrottleStore
    {
        /// <summary>
        /// 定义类型字段时，采用线程安全字典
        /// </summary>
        private readonly ConcurrentDictionary<string, ThrottleEntry> _throttleStore = new ConcurrentDictionary<string, ThrottleEntry>();

        public bool TryGetValue(string key, out ThrottleEntry entry)
        {
            return _throttleStore.TryGetValue(key, out entry);
        }

        public void IncrementRequests(string key)
        {
            _throttleStore.AddOrUpdate(key, k => { return new ThrottleEntry() { Requests = 1 }; },
                                       (k, e) => { e.Requests++; return e; });
        }

        public void Rollover(string key)
        {
            ThrottleEntry dummy;
            _throttleStore.TryRemove(key, out dummy);
        }

        public void Clear()
        {
            _throttleStore.Clear();
        }
    }
}