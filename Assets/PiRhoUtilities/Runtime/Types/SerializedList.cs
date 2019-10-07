using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Utilities
{
	// This class is nothing more than a direct wrapper of the built in List class from the .net framework. Its purpose
	// is to provide a base class for list types to derive from that can then be targeted by a ListDisplay.

	[Serializable]
	public class SerializedList<T> : ICollection<T>, IEnumerable<T>, IEnumerable, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IList
	{
		[SerializeField] protected List<T> _items = new List<T>(); // this is protected so it can be found by the editor

		public List<T> List => _items;

		#region ICollection<T> Implementation

		public int Count
		{
			get { return _items.Count; }
		}

		bool ICollection<T>.IsReadOnly
		{
			get { return false; }
		}

		public void Add(T item)
		{
			_items.Add(item);
		}

		public bool Remove(T item)
		{
			return _items.Remove(item);
		}

		public void Clear()
		{
			_items.Clear();
		}

		public bool Contains(T item)
		{
			return _items.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_items.CopyTo(array, arrayIndex);
		}

		#endregion

		#region ICollection Implementation

		bool ICollection.IsSynchronized
		{
			get { return false; }
		}

		object ICollection.SyncRoot
		{
			get { return this; }
		}

		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)_items).CopyTo(array, index);
		}

		#endregion

		#region IEnumerable<T> Implementation

		public IEnumerator<T> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		#endregion

		#region IEnumerable Implementation

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		#endregion

		#region IList<T> Implementation

		public T this[int index]
		{
			get { return _items[index]; }
			set { _items[index] = value; }
		}

		public int IndexOf(T item)
		{
			return _items.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			_items.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_items.RemoveAt(index);
		}

		#endregion

		#region IList Implementation

		object IList.this[int index]
		{
			get { return _items[index]; }
			set { ((IList)_items)[index] = value; }
		}

		bool IList.IsFixedSize
		{
			get { return false; }
		}

		bool IList.IsReadOnly
		{
			get { return false; }
		}

		int IList.Add(object value)
		{
			return ((IList)_items).Add(value);
		}

		void IList.Insert(int index, object value)
		{
			((IList)_items).Insert(index, value);
		}

		void IList.Remove(object value)
		{
			((IList)_items).Remove(value);
		}

		bool IList.Contains(object value)
		{
			return ((IList)_items).Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return ((IList)_items).IndexOf(value);
		}

		#endregion
	}
}
