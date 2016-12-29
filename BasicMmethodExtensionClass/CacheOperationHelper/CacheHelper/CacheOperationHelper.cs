using System;
using System.Collections;
using System.Web;
using System.Web.Caching;

namespace Helper.CacheHelper
{
    /// <summary>
    /// 缓存辅助类
    /// </summary>
    public static class CacheOperationHelper
    {
        // 绝对缓存过期时间
        public static DateTime AbsoluteMinute1
        {
            get { return DateTime.Now.AddMinutes(1); }
        }

        public static DateTime AbsoluteMinute10
        {
            get { return DateTime.Now.AddMinutes(10); }
        }

        public static DateTime AbsoluteMinute30
        {
            get { return DateTime.Now.AddMinutes(30); }
        }

        public static DateTime AbsoluteHour1
        {
            get { return DateTime.Now.AddHours(1); }
        }

        public static DateTime AbsoluteHour2
        {
            get { return DateTime.Now.AddHours(2); }
        }

        public static DateTime AbsoluteHour5
        {
            get { return DateTime.Now.AddHours(5); }
        }

        public static DateTime AbsoluteHour12
        {
            get { return DateTime.Now.AddHours(12); }
        }

        public static DateTime AbsoluteDay1
        {
            get { return DateTime.Now.AddDays(1); }
        }

        public static DateTime AbsoluteDay7
        {
            get { return DateTime.Now.AddDays(7); }
        }

        public static DateTime AbsoluteDay14
        {
            get { return DateTime.Now.AddDays(14); }
        }

        public static DateTime AbsoluteDay15
        {
            get { return DateTime.Now.AddDays(15); }
        }

        public static DateTime AbsoluteMonth1
        {
            get { return DateTime.Now.AddMonths(1); }
        }
        

        // 滑动缓存过期时间
        public static TimeSpan SlidingMinute1
        {
            get { return new TimeSpan(TimeSpan.TicksPerMinute); }
        }

        public static TimeSpan SlidingMinute10
        {
            get { return new TimeSpan(TimeSpan.TicksPerMinute * 10); }
        }

        public static TimeSpan SlidingMinute30
        {
            get { return new TimeSpan(TimeSpan.TicksPerMinute * 30); }
        }

        public static TimeSpan SlidingHour1
        {
            get { return new TimeSpan(TimeSpan.TicksPerHour); }
        }

        public static TimeSpan SlidingHour2
        {
            get { return new TimeSpan(TimeSpan.TicksPerHour * 2); }
        }

        public static TimeSpan SlidingHour5
        {
            get { return new TimeSpan(TimeSpan.TicksPerHour * 5); }
        }

        public static TimeSpan SlidingHour12
        {
            get { return new TimeSpan(TimeSpan.TicksPerHour * 12); }
        }

        public static TimeSpan SlidingDay1
        {
            get { return new TimeSpan(TimeSpan.TicksPerDay); }
        }

        /// <summary>
        /// 缓存
        /// </summary>
        private static readonly Cache Cache = HttpRuntime.Cache;

        /// <summary>
        /// 根据键获取缓存数据
        /// </summary>
        /// <param name="cacheKey">缓存的键</param>
        /// <returns></returns>
        private static object GetCache(string cacheKey)
        {
            return Cache.Get(cacheKey);
        }

        /// <summary>
        ///  设置缓存
        /// </summary>
        /// <param name="cacheKey">缓存的键</param>
        /// <param name="objValue">缓存的值</param>
        private static void SetCache(string cacheKey, object objValue)
        {
            Cache.Insert(cacheKey, objValue);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="cacheKey">缓存的键</param>
        /// <param name="objValue">缓存的值</param>
        /// <param name="slidingExpiration">滑动过期时间</param>
        private static void SetCache(string cacheKey, object objValue, TimeSpan slidingExpiration)
        {
            Cache.Insert(cacheKey, objValue, null, Cache.NoAbsoluteExpiration, slidingExpiration);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="cacheKey">缓存的键</param>
        /// <param name="objValue">缓存的值</param>
        /// <param name="absoluteExpiration">绝对过期时间</param>
        private static void SetCache(string cacheKey, object objValue, DateTime absoluteExpiration)
        {
            Cache.Insert(cacheKey, objValue, null, absoluteExpiration, Cache.NoSlidingExpiration);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="cacheKey">缓存的键</param>
        /// <param name="objValue">缓存的值</param>
        /// <param name="dependency">文件依赖</param>
        private static void SetCache(string cacheKey, object objValue, CacheDependency dependency)
        {
            Cache.Insert(cacheKey, objValue, dependency, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration);
        }

        /// <summary>
        /// 移除指定的缓存
        /// </summary>
        /// <param name="cacheKey">缓存的键</param>
        public static void Remove(string cacheKey)
        {
            Cache.Remove(cacheKey);
        }

        /// <summary>
        /// 移除全部缓存
        /// </summary>
        public static void Remove()
        {
            var cacheEnum = Cache.GetEnumerator();
            while (cacheEnum.MoveNext())
            {
                Remove(cacheEnum.Key.ToString());
            }
        }

        /// <summary>
        /// 删除以cacheKeyPrefix为前缀的缓存Key的缓存
        /// </summary>
        /// <param name="cacheKeyPrefix">缓存键前缀</param>
        public static void RemoveByKeyStartsWith(string cacheKeyPrefix)
        {
            var cacheEnum = Cache.GetEnumerator();
            while (cacheEnum.MoveNext())
            {
                var key = cacheEnum.Key.ToString();
                if (key.StartsWith(cacheKeyPrefix))
                {
                    Remove(key);
                }
            }
        }

        /// <summary>
        /// 从缓存中获取数据。缓存中不存在的时候，从回调方法getDate中获取，并设置进缓存。
        /// </summary>
        /// <typeparam name="T">缓存的数据类型</typeparam>
        /// <param name="cacheKey">缓存的键</param>
        /// <param name="getData">回调方法</param>
        /// <returns>缓存中的数据</returns>
        public static T Get<T>(string cacheKey, Func<T> getData)
        {
            var data = GetCache(cacheKey);
            if (data != null) return (T) data;
            data = getData();
            SetCache(cacheKey, data);
            return (T)data;
        }

        /// <summary>
        /// 从缓存中获取数据。缓存中不存在的时候，从回调方法getDate中获取，并设置进缓存。
        /// </summary>
        /// <typeparam name="T">缓存的数据类型</typeparam>
        /// <param name="cacheKey">缓存的键</param>
        /// <param name="slidingExpiration">滑动过期时间</param>
        /// <param name="getData">回调方法</param>
        /// <returns>缓存中的数据</returns>
        public static T Get<T>(string cacheKey, TimeSpan slidingExpiration, Func<T> getData)
        {
            var data = GetCache(cacheKey);
            if (data != null) return (T) data;
            data = getData();
            SetCache(cacheKey, data, slidingExpiration);
            return (T)data;
        }

        /// <summary>
        /// 从缓存中获取数据。缓存中不存在的时候，从回调方法getDate中获取，并设置进缓存。
        /// </summary>
        /// <typeparam name="T">缓存的数据类型</typeparam>
        /// <param name="cacheKey">缓存的键</param>
        /// <param name="absoluteExpiration">绝对过期时间</param>
        /// <param name="getData">回调方法</param>
        /// <returns>缓存中的数据</returns>
        public static T Get<T>(string cacheKey, DateTime absoluteExpiration, Func<T> getData)
        {
            var data = GetCache(cacheKey);
            if (data != null) return (T) data;
            data = getData();
            SetCache(cacheKey, data, absoluteExpiration);
            return (T)data;
        }

        /// <summary>
        /// 从缓存中获取数据。缓存中不存在的时候，从回调方法getDate中获取，并设置进缓存。
        /// </summary>
        /// <typeparam name="T">缓存的数据类型</typeparam>
        /// <param name="cacheKey">缓存的键</param>
        /// <param name="dependency">文件依赖</param>
        /// <param name="getData">回调方法</param>
        /// <returns>缓存中的数据</returns>
        public static T Get<T>(string cacheKey, CacheDependency dependency, Func<T> getData)
        {
            var data = GetCache(cacheKey);
            if (data != null) return (T) data;
            data = getData();
            SetCache(cacheKey, data, dependency);
            return (T)data;
        }

        /// <summary>
        /// 从缓存中获取数据。缓存中不存在的时候，从回调方法getDate中获取，并设置进缓存。
        /// </summary>
        /// <typeparam name="T">缓存的数据类型</typeparam>
        /// <param name="cacheKey">缓存的键</param>
        /// <param name="filename">依赖的文件路径</param>
        /// <param name="getData">回调方法</param>
        /// <returns>缓存中的数据</returns>
        public static T Get<T>(string cacheKey, string filename, Func<T> getData)
        {
            return Get<T>(cacheKey, new CacheDependency(filename), getData);
        }

        /// <summary>
        /// 从缓存中获取数据。缓存中不存在的时候，从回调方法getDate中获取，并设置进缓存。
        /// </summary>
        /// <typeparam name="T">缓存的数据类型</typeparam>
        /// <param name="cacheKey">缓存的键</param>
        /// <param name="filenames">依赖的文件路径</param>
        /// <param name="getData">回调方法</param>
        /// <returns>缓存中的数据</returns>
        public static T Get<T>(string cacheKey, string[] filenames, Func<T> getData)
        {
            return Get<T>(cacheKey, new CacheDependency(filenames), getData);
        }
    }
}