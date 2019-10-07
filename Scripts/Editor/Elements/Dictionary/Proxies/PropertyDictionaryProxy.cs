using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class PropertyDictionaryProxy : IDictionaryProxy
	{
		public Func<string, bool> CanAddCallback;
		public Func<string, bool> CanRemoveCallback;
		public Func<string, bool> CanReorderCallback;

		public Action<string> AddCallback;
		public Action<string> RemoveCallback;
		public Action<string> ReorderCallback;

		private SerializedProperty _property;
		private SerializedProperty _keysProperty;
		private SerializedProperty _valuesProperty;
		private PropertyDrawer _drawer;

		public int KeyCount => _keysProperty.arraySize;

		public string Label => _property.displayName;
		public string Tooltip { get; set; }
		public string EmptyLabel { get; set; } = DictionaryProxy.DefaultEmptyLabel;
		public string EmptyTooltip { get; set; } = DictionaryProxy.DefaultEmptyTooltip;
		public string AddPlaceholder { get; set; } = DictionaryProxy.DefaultAddPlaceholder;
		public string AddTooltip { get; set; } = DictionaryProxy.DefaultAddTooltip;
		public string RemoveTooltip { get; set; } = DictionaryProxy.DefaultRemoveTooltip;
		public string ReorderTooltip { get; set; } = DictionaryProxy.DefaultReorderTooltip;

		public bool AllowAdd { get; set; } = true;
		public bool AllowRemove { get; set; } = true;
		public bool AllowReorder { get; set; } = false;

		public PropertyDictionaryProxy(SerializedProperty property, SerializedProperty keys, SerializedProperty values, PropertyDrawer drawer)
		{
			_property = property;
			_keysProperty = keys;
			_valuesProperty = values;
			_drawer = drawer;
		}

		public VisualElement CreateField(int index)
		{
			var key = _keysProperty.GetArrayElementAtIndex(index);
			var value = _valuesProperty.GetArrayElementAtIndex(index);

			var field = _drawer != null
				? _drawer.CreatePropertyGUI(value)
				: value.CreateField();

			field.userData = index;
			field.Bind(_property.serializedObject);
			
			if (field is PropertyField propertyField)
				propertyField.SetLabel(key.stringValue);
			else if (field is FieldContainer fieldContainer)
				fieldContainer.SetLabel(key.stringValue);
			if (field.GetType().InheritsGeneric(typeof(BaseField<>)))
				BaseFieldExtensions.SetLabel(field, key.stringValue);

			return field;
		}

		public bool NeedsUpdate(VisualElement item, int index)
		{
			return !(item.userData is int i) || i != index;
		}

		public bool CanAdd(string key)
		{
			return (CanAddCallback?.Invoke(key) ?? AllowAdd) && !ContainsKey(key);
		}

		public bool CanRemove(int index)
		{
			var property = _keysProperty.GetArrayElementAtIndex(index);
			return CanRemoveCallback?.Invoke(property.stringValue) ?? AllowRemove;
		}

		public bool CanReorder(int from, int to)
		{
			var property = _keysProperty.GetArrayElementAtIndex(from);
			return CanReorderCallback?.Invoke(property.stringValue) ?? AllowReorder;
		}

		public void AddItem(string key)
		{
			_keysProperty.arraySize++;
			_valuesProperty.ResizeArray(KeyCount);

			var newItem = _keysProperty.GetArrayElementAtIndex(KeyCount - 1);
			newItem.stringValue = key;

			_property.serializedObject.ApplyModifiedProperties();

			AddCallback?.Invoke(key);
		}

		public void RemoveItem(int index)
		{
			var property = _keysProperty.GetArrayElementAtIndex(index);
			RemoveCallback?.Invoke(property.stringValue);

			_keysProperty.RemoveFromArray(index);
			_valuesProperty.RemoveFromArray(index);
			_property.serializedObject.ApplyModifiedProperties();
		}

		public void ReorderItem(int from, int to)
		{
			_keysProperty.MoveArrayElement(from, to);
			_valuesProperty.MoveArrayElement(from, to);
			_property.serializedObject.ApplyModifiedProperties();

			var property = _keysProperty.GetArrayElementAtIndex(to);
			ReorderCallback?.Invoke(property.stringValue);
		}

		private bool ContainsKey(string key)
		{
			for (var i = 0; i < KeyCount; i++)
			{
				var property = _keysProperty.GetArrayElementAtIndex(i);
				if (property.stringValue == key)
					return true;
			}

			return false;
		}
	}
}
