using System;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Utilities
{
	// This class is nothing more than a direct wrapper of the built in Array class from the .net framework. Its purpose
	// is to provide a base class for array types to derive from that can then be targeted by an ArrayDisplay.

	[Serializable]
	public class SerializedArray<T> : ICloneable, IList, IStructuralComparable, IStructuralEquatable, ICollection, IEnumerable
	{
		[SerializeField] protected T[] _items; // this is protected so it can be found by the editor

		public T[] Array => _items;
		public int Length => _items.Length;

		public SerializedArray(int count)
		{
			_items = new T[count];
		}

		public T this[int index]
		{
			get { return _items[index]; }
			set { _items[index] = value; }
		}

		#region ICollection Implementation

		int ICollection.Count
		{
			get { return ((ICollection)_items).Count; }
		}

		public bool IsSynchronized
		{
			get { return _items.IsSynchronized; }
		}

		public object SyncRoot
		{
			get { return _items.SyncRoot; }
		}

		public void CopyTo(Array array, int index)
		{
			_items.CopyTo(array, index);
		}

		#endregion

		#region IClonable Implementation

		public object Clone()
		{
			return _items.Clone();
		}

		#endregion

		#region IComparable Implementation

		int IStructuralComparable.CompareTo(object other, IComparer comparer)
		{
			return ((IStructuralComparable)_items).CompareTo(other, comparer);
		}

		#endregion

		#region IStructuralEquatable Implementation

		bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
		{
			return ((IStructuralEquatable)_items).Equals(other, comparer);
		}

		int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			return ((IStructuralEquatable)_items).GetHashCode(comparer);
		}

		#endregion

		#region IEnumerable Implementation

		public IEnumerator GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		#endregion

		#region IList Implementation

		object IList.this[int index]
		{
			get { return _items[index]; }
			set { ((IList)_items)[index] = value; }
		}

		public bool IsFixedSize
		{
			get { return _items.IsFixedSize; }
		}

		public bool IsReadOnly
		{
			get { return _items.IsReadOnly; }
		}

		int IList.Add(object value)
		{
			return ((IList)_items).Add(value);
		}

		void IList.Clear()
		{
			((IList)_items).Clear();
		}

		bool IList.Contains(object value)
		{
			return ((IList)_items).Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return ((IList)_items).IndexOf(value);
		}

		void IList.Insert(int index, object value)
		{
			((IList)_items).Insert(index, value);
		}

		void IList.Remove(object value)
		{
			((IList)_items).Remove(value);
		}

		void IList.RemoveAt(int index)
		{
			((IList)_items).RemoveAt(index);
		}

		#endregion
	}
}
