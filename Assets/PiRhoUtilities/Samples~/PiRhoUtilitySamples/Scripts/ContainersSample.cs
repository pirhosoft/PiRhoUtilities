using System;
using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Containers")]
	public class ContainersSample : MonoBehaviour
	{
		[Serializable] public class TestList : SerializedList<int> { }
		[Serializable] public class TestArray : SerializedArray<float> { public TestArray(int count) : base(count) { } }
		[Serializable] public class TestDictionary : SerializedDictionary<string, string> { }

		[Tooltip("A test list with a max of 5 items")]
		[List(AllowAdd = nameof(ListCanAdd))]
		[Slider(0, 10)]
		public TestList List;

		[Tooltip("A 4 item test array")]
		[List(AllowAdd = ListAttribute.Never, AllowRemove = ListAttribute.Never, AllowReorder = ListAttribute.Never)]
		[Minimum(0.0f)]
		public TestArray Array = new TestArray(4);

		[Tooltip("A test dictionary with callbacks")]
		[Dictionary(AddCallback = nameof(DictionaryItemAdded), RemoveCallback = nameof(DictionaryItemRemoved))]
		[Stretch]
		public TestDictionary Dictionary;

		private bool ListCanAdd()
		{
			return List.Count < 5;
		}

		private void DictionaryItemAdded(string key)
		{
			Debug.Log($"'{key}' added", this);
		}

		private void DictionaryItemRemoved(string key)
		{
			Debug.Log($"'{key}' removed", this);
		}
	}
}