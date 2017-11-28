using System;
using System.Collections.Generic;
namespace CommandLine.Infrastructure
{
	internal sealed class ReflectionCache
	{
		private static readonly ReflectionCache Singleton;
		private readonly IDictionary<Pair<Type, object>, WeakReference> _cache;
		public static ReflectionCache Instance
		{
			get
			{
				return ReflectionCache.Singleton;
			}
		}
		public object this[Pair<Type, object> key]
		{
			get
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				if (!this._cache.ContainsKey(key))
				{
					return null;
				}
				return this._cache[key].Target;
			}
			set
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				this._cache[key] = new WeakReference(value);
			}
		}
		static ReflectionCache()
		{
			ReflectionCache.Singleton = new ReflectionCache();
		}
		private ReflectionCache()
		{
			this._cache = new Dictionary<Pair<Type, object>, WeakReference>();
		}
	}
}
