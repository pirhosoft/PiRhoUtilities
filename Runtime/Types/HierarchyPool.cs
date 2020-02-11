using System;
using System.Collections.Generic;
using System.Linq;

namespace PiRhoSoft.Utilities
{
	public class HierarchyPool<BaseType>
		where BaseType : class
	{
		private Dictionary<Type, ISubclassPool> _pools = new Dictionary<Type, ISubclassPool>();

		public void Register<KeyType, Type>(Func<Type> creator, int capacity = ClassPool.DefaultCapacity, int growth = ClassPool.DefaultGrowth)
			where Type : class, BaseType
		{
			_pools.Add(typeof(KeyType), new SubclassPool<Type>(creator, capacity, growth));
		}

		public List<ClassPoolInfo> GetPoolInfo()
		{
			return _pools
				.Select(pool => new ClassPoolInfo
				{
					Type = pool.Key,
					IsRegistered = pool.Value == null ? false : pool.Value.IsRegistered,
					ReservedCount = pool.Value == null ? 0 : pool.Value.ReservedCount,
					FreeCount = pool.Value == null ? -1 : pool.Value.FreeCount
				})
				.ToList();
		}

		public BaseType Reserve(Type type)
		{
			var pool = GetPool(type);
			return pool?.Reserve();
		}

		public Type Reserve<Type>()
			where Type : class, BaseType
		{
			var pool = GetPool(typeof(Type));
			return pool?.Reserve() as Type;
		}

		public void Release(BaseType item)
		{
			var pool = GetPool(item.GetType());
			pool?.Release(item);
		}

		private interface ISubclassPool
		{
			bool IsRegistered { get; }
			int ReservedCount { get; }
			int FreeCount { get; }
			BaseType Reserve();
			void Release(BaseType item);
		}

		private class SubclassPool<Type> : ClassPool<Type>, ISubclassPool
			where Type : class, BaseType
		{
			public bool IsRegistered => true;
			BaseType ISubclassPool.Reserve() => Reserve();
			void ISubclassPool.Release(BaseType item) => Release((Type)item);

			public SubclassPool(Func<Type> creator, int capacity, int growth) : base(creator, capacity, growth) { }
		}

		private class GenericSubclassPool : ClassPool<object>, ISubclassPool
		{
			public bool IsRegistered => false;
			BaseType ISubclassPool.Reserve() => Reserve() as BaseType;
			void ISubclassPool.Release(BaseType item) => Release(item);

			public GenericSubclassPool(Type type) : base(() => Activator.CreateInstance(type)) {}
		}

		private ISubclassPool GetPool(Type type)
		{
			if (!_pools.TryGetValue(type, out var pool))
			{
				// This will not work in builds where unusued types get stripped. It is intended to be used as a
				// fallback during development without having to register all the types.

				// TODO: Figure out exactly what happens and handle when type has not been included in the build.

				if (typeof(BaseType).IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null)
					pool = new GenericSubclassPool(type);
				else
					pool = null;

				// If the pool can't be created it is still registered so the creation won't be attempted the next time
				// and so it can be seen in GetPoolInfo.
				_pools.Add(type, pool);
			}

			return pool;
		}
	}
}
