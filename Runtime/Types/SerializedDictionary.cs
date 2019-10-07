using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[Serializable]
	public class SerializedDictionary<KeyType, ValueType> : Dictionary<KeyType, ValueType>, ISerializationCallbackReceiver
	{
		// These are protected so they can be found by the editor.
		[SerializeField] protected List<KeyType> _keys = new List<KeyType>();
		[SerializeField] protected List<ValueType> _values = new List<ValueType>();

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			ConvertToLists();
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			ConvertFromLists();

			_keys.Clear();
			_values.Clear();
		}

		private void ConvertToLists()
		{
			_keys.Clear();
			_values.Clear();

			foreach (var entry in this)
			{
				_keys.Add(entry.Key);
				_values.Add(entry.Value);
			}
		}

		private void ConvertFromLists()
		{
			Clear();

			var count = Math.Min(_keys.Count, _values.Count);

			for (var i = 0; i < count; i++)
				Add(_keys[i], _values[i]);
		}
	}
}
