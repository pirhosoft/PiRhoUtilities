using System;
using System.Collections.Generic;

namespace PiRhoSoft.Utilities
{
	public class ClassPoolInfo
	{
		public Type Type;
		public bool IsRegistered;
		public int ReservedCount;
		public int FreeCount;
	}

	public static class ClassPool
	{
		public const int DefaultCapacity = 10;
		public const int DefaultGrowth = 5;
	}

	public class ClassPool<Type>
	{
		private int _growth;
		private Stack<Type> _freeList;
		private Func<Type> _creator;
		private int _reservedCount;

		public int ReservedCount => _reservedCount;
		public int FreeCount => _freeList.Count;

		public ClassPool(Func<Type> creator, int capacity = ClassPool.DefaultCapacity, int growth = ClassPool.DefaultGrowth)
		{
			_growth = growth;
			_freeList = new Stack<Type>(capacity);
			_creator = creator;
			_reservedCount = 0;

			for (var i = 0; i < capacity; i++)
				Release(_creator());
		}

		public Type Reserve()
		{
			if (_freeList.Count == 0)
			{
				if (_growth > 0)
				{
					for (var i = 0; i < _growth; i++)
						Release(_creator());
				}
				else
				{
					return default;
				}
			}

			_reservedCount++;
			return _freeList.Pop();
		}

		public void Release(Type item)
		{
			_reservedCount--;
			_freeList.Push(item);
		}

		public ClassPoolInfo GetPoolInfo()
		{
			return new ClassPoolInfo
			{
				Type = typeof(Type),
				IsRegistered = true,
				ReservedCount = ReservedCount,
				FreeCount = FreeCount
			};
		}
	}
}
