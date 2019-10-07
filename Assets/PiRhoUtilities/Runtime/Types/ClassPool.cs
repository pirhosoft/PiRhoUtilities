using System.Collections.Generic;

namespace PiRhoSoft.Utilities
{
	public interface IPoolable
	{
		void Reset();
	}

	public interface IPoolInfo
	{
		int Size { get; }
		int Growth { get; }
	}

	public interface IClassPool<T> where T : IPoolable
	{
		void Grow();
		T Reserve();
		void Release(T value);
	}

	public class ClassPool<T, I> : IClassPool<T> where T : IPoolable, new() where I : IPoolInfo, new()
	{
		private static IPoolInfo _info = new I();
		private Stack<T> _freeList;

		public ClassPool()
		{
			_freeList = new Stack<T>(_info.Size);

			for (var i = 0; i < _info.Size; i++)
				Release(new T());
		}

		public void Grow()
		{
			for (var i = 0; i < _info.Growth; i++)
				Release(new T());
		}

		public T Reserve()
		{
			if (_freeList.Count == 0)
				Grow();

			return _freeList.Pop();
		}

		public void Release(T value)
		{
			value.Reset();
			_freeList.Push(value);
		}
	}
}
