using System;
using System.Threading.Tasks;

namespace BasicMmethodExtensionClass.CacheOperationHelper.CacheHelper
{
    public interface ICacheProvider
    {
        /// <summary>
        /// Gets the item from the cache, if the item is not present 
        /// then we will get that item and store it in the cache.
        /// </summary>
        /// <typeparam name="T">Type to store in the cache</typeparam>
        /// <param name="key">The key</param>
        /// <param name="itemCallback">The item callback. This will be called if the item is not present in the cache. </param>
        /// <param name="cacheTime">The amount of time we want to cache the object</param>
        /// <returns><see cref="T"/></returns>
        T GetOrSet<T>(string key, Func<T> itemCallback, int cacheTime = 20) where T : class;
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> itemCallback, int cacheTime = 20) where T : class;

        /// <summary>
        /// Gets the specified item from the cache.
        /// </summary>
        /// <typeparam name="T">Type to get from the cache</typeparam>
        /// <param name="key">The key.</param>
        /// <returns><see cref="T"/></returns>
        T Get<T>(string key) where T : class;

        /// <summary>
        /// Set/Store the specified object in the cache
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="data">The object we want to store.</param>
        /// <param name="cacheTime">The amount of time we want to cache the object.</param>
        void Set(string key, object data, int cacheTime = 20);

        /// <summary>
        /// Removes the specified object from the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        void Remove(string key);
    }
}
