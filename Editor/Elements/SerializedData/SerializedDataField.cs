using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public abstract class SerializedDataField<T> : PropertyWatcher<string>
	{
		private const string _contentPropertyName = "_content";
		private const string _referencesPropertyName = "_references";

		private T _value;
		private readonly SerializedData _data;
		private readonly SerializedProperty _contentProperty;
		private readonly SerializedProperty _referencesProperty;

		public SerializedDataField(SerializedProperty property, T value)
		{
			_value = value;
			_data = new SerializedData();
			_contentProperty = property.FindPropertyRelative(_contentPropertyName);
			_referencesProperty = property.FindPropertyRelative(_referencesPropertyName);

			style.display = DisplayStyle.Flex;

			Watch(_contentProperty);
		}

		protected override sealed void OnChanged(SerializedProperty property, string previousValue, string newValue)
		{
			Extract();
		}

		protected abstract void Load(SerializedDataReader reader, ref T value);
		protected abstract void Save(SerializedDataWriter writer, T value);
		protected abstract void Update(T value);

		public void Extract()
		{
			_data.EditorContent = _contentProperty.stringValue;
			_data.EditorReferences.Clear();

			if (!string.IsNullOrEmpty(_data.EditorContent))
			{
				for (var i = 0; i < _referencesProperty.arraySize; i++)
				{
					var reference = _referencesProperty.GetArrayElementAtIndex(i).objectReferenceValue;
					_data.EditorReferences.Add(reference);
				}

				using (var reader = new SerializedDataReader(_data))
					Load(reader, ref _value);
			}

			Update(_value);
		}

		public void Inject(T value)
		{
			_value = value;
			_data.EditorContent = string.Empty;
			_data.EditorReferences.Clear();

			using (var writer = new SerializedDataWriter(_data))
				Save(writer, _value);

			_contentProperty.stringValue = _data.EditorContent;
			_referencesProperty.arraySize = _data.EditorReferences.Count;

			var i = 0;
			foreach (var reference in _data.EditorReferences)
				_referencesProperty.GetArrayElementAtIndex(i++).objectReferenceValue = reference;

			_contentProperty.serializedObject.ApplyModifiedProperties();

			Update(_value);
		}
	}
}
