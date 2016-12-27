using System.Collections.Generic;

namespace BasicMmethodExtensionClass.JsonOperationHelper
{
	public class SafeDictionary<TKey, TValue>
	{
		private readonly object _padlock = new object();

		private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
		
		public bool ContainsKey(TKey key)
		{
			return _dictionary.ContainsKey(key);
		}

		public TValue this[TKey key]
		{
			get
			{
				return _dictionary[key];
			}
		}
		
		public void Add(TKey key, TValue value)
		{
			lock(_padlock)
			{
				_dictionary.Add(key,value);
			}
		}
	}
}
