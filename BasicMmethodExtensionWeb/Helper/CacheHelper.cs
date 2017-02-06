using System;
using System.Web;
using System.Web.Caching;

namespace BasicMmethodExtensionWeb.Helper
{
    /// <summary>
    /// 缓存管理-工具类
    /// </summary>
    public static class CacheHelper
    {
        private static readonly object TheLocker = new object();

        /// <summary>
        /// 键销毁指定标识符的缓存项
        /// </summary>
        /// <param name="key">要检索的缓存项的标识符</param>
        public static void Destroy(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(key);
            }
            lock (TheLocker)
            {
                HttpRuntime.Cache.Remove(key);
            }
        }

        /// <summary>
        /// 获取指定标识符的缓存项
        /// </summary>
        /// <param name="key">要检索的缓存项的标识符</param>
        /// <returns>检索到的缓存项，未找到该键时为 null</returns>
        public static object Get(string key)
        {
            var cached = HttpRuntime.Cache.Get(key);
            return cached;
        }

        /// <summary>
        /// 获取指定标识符的缓存项
        /// </summary>
        /// <typeparam name="T">缓存项的类型</typeparam>
        /// <param name="key">要检索的缓存项的标识符</param>
        /// <param name="value">检索到的缓存项，未找到该键时为 null</param>
        /// <returns>是否成功获取缓存项</returns>
        public static bool Get<T>(string key, out T value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(key);
            }
            var cached = Get(key);
            if (cached is T)
            {
                value = (T)cached;
                return true;
            }
            value = default(T);
            return false;
        }

        /// <summary>
        /// 设置指定标识符的缓存项
        /// </summary>
        /// <typeparam name="T">缓存项的类型</typeparam>
        /// <param name="key">要检索的缓存项的标识符</param>
        /// <param name="value">缓存项</param>
        /// <param name="expire">相对过期时间</param>
        /// <param name="onRemoveCallback">在从缓存中移除对象时所调用的委托。 当从缓存中删除应用程序的对象时，可使用它来通知应用程序。</param>
        public static void Set<T>(string key, T value, TimeSpan expire, CacheItemRemovedCallback onRemoveCallback = null)
        {
            lock (TheLocker)
            {
                HttpRuntime.Cache.Remove(key);
                HttpRuntime.Cache.Add(key, value, null, Cache.NoAbsoluteExpiration, expire, CacheItemPriority.Default, onRemoveCallback);
            }
        }

        /// <summary>
        /// 设置指定标识符的缓存项
        /// </summary>
        /// <typeparam name="T">缓存项的类型</typeparam>
        /// <param name="key">要检索的缓存项的标识符</param>
        /// <param name="value">缓存项</param>
        /// <param name="expire">绝对过期时间</param>
        /// <param name="onRemoveCallback">在从缓存中移除对象时所调用的委托。 当从缓存中删除应用程序的对象时，可使用它来通知应用程序。</param>
        public static void Set<T>(string key, T value, DateTime expire, CacheItemRemovedCallback onRemoveCallback = null)
        {
            lock (TheLocker)
            {
                HttpRuntime.Cache.Remove(key);
                HttpRuntime.Cache.Add(key, value, null, expire, Cache.NoSlidingExpiration, CacheItemPriority.Default, onRemoveCallback);
            }
        }

        /// <summary>
        /// 设置指定标识符的缓存项
        /// 自增，从 0 开始，默认加 1
        /// </summary>
        /// <param name="key">要检索的缓存项的标识符</param>
        /// <param name="expire">相对过期时间</param>
        /// <param name="onRemoveCallback">在从缓存中移除对象时所调用的委托。 当从缓存中删除应用程序的对象时，可使用它来通知应用程序。</param>
        public static void SetIncrby(string key, TimeSpan expire, CacheItemRemovedCallback onRemoveCallback = null)
        {
            lock (TheLocker)
            {
                var cached = HttpRuntime.Cache.Get(key);
                long cachedValue = 0;
                if (cached is long)
                {
                    cachedValue = (long)cached;
                }
                cachedValue += 1;
                HttpRuntime.Cache.Remove(key);
                HttpRuntime.Cache.Add(key, cachedValue, null, Cache.NoAbsoluteExpiration, expire, CacheItemPriority.Default, onRemoveCallback);
            }
        }

        /// <summary>
        /// 设置指定标识符的缓存项
        /// 自增，从 0 开始，默认加 1
        /// </summary>
        /// <param name="key">要检索的缓存项的标识符</param>
        /// <param name="expire">绝对过期时间</param>
        /// <param name="onRemoveCallback">在从缓存中移除对象时所调用的委托。 当从缓存中删除应用程序的对象时，可使用它来通知应用程序。</param>
        public static void SetIncrby(string key, DateTime expire, CacheItemRemovedCallback onRemoveCallback = null)
        {
            lock (TheLocker)
            {
                var cached = HttpRuntime.Cache.Get(key);
                long cachedValue = 0;
                if (cached is long)
                {
                    cachedValue = (long)cached;
                }
                cachedValue += 1;
                HttpRuntime.Cache.Remove(key);
                HttpRuntime.Cache.Add(key, cachedValue, null, expire, Cache.NoSlidingExpiration, CacheItemPriority.Default, onRemoveCallback);
            }
        }
    }
}