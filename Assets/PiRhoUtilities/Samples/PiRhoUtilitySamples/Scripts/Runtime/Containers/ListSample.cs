using System;
using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/List")]
	public class ListSample : MonoBehaviour
	{
		[Serializable] public class StringList : SerializedList<string> { }
		[Serializable] public class IntArray : SerializedArray<int> { public IntArray() : base(5) { } }

		[MessageBox("The [List] attribute is applied to fields subclassed from SeriializedList<T> or SerializedArray<T> to display it as a customizable list view. Adding, removing, reordering and other callbacks can be specified.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[List(EmptyLabel = "There are no strings in the list", AddCallback = nameof(StringAdded), RemoveCallback = nameof(StringRemoved), ReorderCallback = nameof(StringsReordered), ChangeCallback = nameof(StringsChanged))]
		public StringList Strings = new StringList();

		[List(AllowAdd = nameof(MaximumArray5), AllowRemove = nameof(MinimumArray2), AllowReorder = false)]
		public IntArray Ints = new IntArray();

		private void StringAdded()
		{
			Debug.Log("String added");
		}

		private void StringRemoved()
		{
			Debug.Log("String removed");
		}

		private void StringsReordered()
		{
			Debug.Log("Strings reordered");
		}

		private void StringsChanged()
		{
			Debug.Log("Strings changed");
		}

		private bool MaximumArray5()
		{
			return Ints.Length < 5;
		}

		private bool MinimumArray2(int index)
		{
			return Ints.Array.Length > 2;
		}
	}
}