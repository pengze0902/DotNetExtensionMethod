using System;
using System.Collections.Concurrent;

namespace BasicMmethodExtensionClass.CacheOperationHelper.CacheHelper
{
    /// <summary>
    /// 同步助手：与键相关联的静态锁集合。
    /// NamedLock管理可以通过应用程序中的键（名称）访问的关键段的生命周期。
    ///它也有一些帮助方法允许最大等待时间（超时）获得锁和安全释放它。
    ///注意：这个nuget包包含c＃源代码，并且取决于.Net 4.0中引入的System.Collections.Concurrent。
    /// </summary>
    public class NamedLock : IDisposable
    {
        private static readonly ConcurrentDictionary <string, CountedLock> MWaitLock = 
            new ConcurrentDictionary<string, CountedLock> (StringComparer.Ordinal);
        
        private static object GetOrAdd (string key)
        {
            CountedLock padlock = MWaitLock.GetOrAdd (key, LockFactory);
            padlock.Increment ();
            return padlock;
        }

        private static void ReleaseOrRemove (string key)
        {
            CountedLock padlock;
            if (MWaitLock.TryGetValue (key, out padlock))
            {
                if (padlock.Decrement () <= 0)
                    MWaitLock.TryRemove (key, out padlock);
            }
        }

        private static CountedLock LockFactory (string key)
        {
            return new CountedLock ();
        }

        class CountedLock
        {
            private int _mCounter;

            public int Increment ()
            {
                return System.Threading.Interlocked.Increment (ref _mCounter);
            }

            public int Decrement ()
            {
                return System.Threading.Interlocked.Decrement (ref _mCounter);
            }
        }

        private bool _mLocked;
        
        /// <summary>
        /// Check if a lock was aquired.
        /// </summary>
        public bool IsLocked
        {
            get { return _mLocked; }
        }

        /// <summary>
        /// Gets the lock key name.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets the internal lock object.
        /// </summary>
        public object Lock { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="NamedLock" /> class.
        /// </summary>
        /// <param name="key">The named lock key.</param>
        public NamedLock (string key)
        {
            Key = key;
            Lock = GetOrAdd (Key);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing,
        /// or resetting unmanaged resources.
        /// Releases aquired lock and related resources.
        /// </summary>
        public void Dispose ()
        {
            Exit ();
            ReleaseOrRemove (Key);
        }

        /// <summary>
        /// Tries to aquire a lock.
        /// </summary>
        public bool Enter ()
        {
            if (!_mLocked)
            {
                System.Threading.Monitor.Enter (Lock, ref _mLocked);
            }
            return _mLocked;
        }

        /// <summary>
        /// Tries to aquire a lock respecting the specified timeout.
        /// </summary>
        /// <param name="waitTimeoutMilliseconds">The wait timeout milliseconds.</param>
        /// <returns>If the lock was aquired in the specified timeout</returns>
        public bool Enter (int waitTimeoutMilliseconds)
        {
            if (!_mLocked)
            {
                System.Threading.Monitor.TryEnter (Lock, waitTimeoutMilliseconds, ref _mLocked);
            }
            return _mLocked;
        }

        /// <summary>
        /// Tries to aquire a lock respecting the specified timeout.
        /// </summary>
        /// <param name="waitTimeout">The wait timeout.</param>
        /// <returns>If the lock was aquired in the specified timeout</returns>
        public bool Enter (TimeSpan waitTimeout)
        {
            return Enter ((int)waitTimeout.TotalMilliseconds);
        }

        /// <summary>
        /// Releases the lock if it was already aquired.
        /// Called also at "Dispose".
        /// </summary>
        public bool Exit ()
        {
            if (_mLocked)
            {
                _mLocked = false;
                System.Threading.Monitor.Exit (Lock);                
            }
            return false;
        }

        /// <summary>
        /// Creates a new instance and tries to aquire a lock.
        /// </summary>
        /// <param name="key">The named lock key.</param>
        public static NamedLock CreateAndEnter (string key)
        {
            NamedLock item;
            item = new NamedLock (key);
            item.Enter ();
            return item;
        }

        /// <summary>
        /// Creates a new instance and tries to aquire a lock.
        /// </summary>
        /// <param name="key">The named lock key.</param>
        /// <param name="waitTimeoutMilliseconds">The wait timeout milliseconds.</param>
        public static NamedLock CreateAndEnter (string key, int waitTimeoutMilliseconds)
        {
            NamedLock item;
            item = new NamedLock (key);
            item.Enter (waitTimeoutMilliseconds);
            return item;
        }

        /// <summary>
        /// Creates a new instance and tries to aquire a lock.
        /// </summary>
        /// <param name="key">The named lock key.</param>
        /// <param name="waitTimeout">The wait timeout.</param>
        public static NamedLock CreateAndEnter (string key, TimeSpan waitTimeout)
        {
            return CreateAndEnter (key, (int)waitTimeout.TotalMilliseconds);
        }

    }
}