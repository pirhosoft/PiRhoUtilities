using System;
using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Dictionary")]
	public class DictionarySample : MonoBehaviour
	{
		[Serializable] public class IntDictionary : SerializedDictionary<string, int> { }

		[MessageBox("The [Dictionary] attribute is applied to fields subclassed from SeriializedDictionary<string, Value> to display it as a customizable dictioray view. Adding, removing, reordering and other callbacks can be specified.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Dictionary(AddCallback = nameof(IntAdded), RemoveCallback = nameof(IntRemoved), ReorderCallback = nameof(IntsReordered), ChangeCallback = nameof(IntsChanged), AllowAdd = nameof(MaximumSize5), AllowRemove = nameof(Removable))]
		public IntDictionary Ints = new IntDictionary { { "Can't Remove This", 0 } };

		private void IntAdded(string key)
		{
			Debug.LogFormat("{0} added", key);
		}

		private void IntRemoved(string key)
		{
			Debug.LogFormat("{0} removed", key);
		}

		private void IntsReordered()
		{
			Debug.Log("Ints reordered");
		}

		private void IntsChanged()
		{
			Debug.Log("Ints changed");
		}

		private bool MaximumSize5()
		{
			return Ints.Count < 5;
		}

		private bool Removable(string key)
		{
			return key != "Can't Remove This";
		}
	}
}
