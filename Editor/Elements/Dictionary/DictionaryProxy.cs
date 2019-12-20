using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public interface IDictionaryProxy
	{
		VisualElement CreateElement(int index, string key);

		int Count { get; }
		string GetKey(int index);

		bool CanAdd(string key);
		bool CanAdd(Type type);
		bool AddItem(string key, Type type);

		bool CanRemove(int index, string key);
		void RemoveItem(int index, string key);

		bool IsReorderable { get; }
		void ReorderItem(int from, int to);
	}

	public class DictionaryProxy : IDictionaryProxy
	{
		public IDictionary Items { get; private set; }
		public Func<IDictionary, string, VisualElement> Creator { get; private set; }

		public DictionaryProxy(IDictionary items, Func<IDictionary, string, VisualElement> creator)
		{
			Items = items;
			Creator = creator;
		}

		public VisualElement CreateElement(int index, string key) => Creator.Invoke(Items, key);

		public int Count => Items.Count;
		public string GetKey(int index) => Items.Keys.Cast<string>().ElementAt(index);

		public bool CanAdd(string key) => !Items.Contains(key);
		public bool CanAdd(Type type) => type != null;

		public bool AddItem(string key, Type type)
		{
			try
			{
				var item = Activator.CreateInstance(type);
				Items.Add(key, item);
			}
			catch
			{
				return false;
			}

			return true;
		}

		public bool CanRemove(int index, string key) => true;
		public void RemoveItem(int index, string key) => Items.Remove(key);

		public bool IsReorderable => false;
		public void ReorderItem(int from, int to) { }
	}

	public class DictionaryProxy<T> : IDictionaryProxy
	{
		public IDictionary<string, T> Items { get; private set; }
		public Func<IDictionary<string, T>, string, VisualElement> Creator { get; private set; }

		public DictionaryProxy(Dictionary<string, T> items, Func<IDictionary<string, T>, string, VisualElement> creator)
		{
			Items = items;
			Creator = creator;
		}

		public VisualElement CreateElement(int index, string key) => Creator.Invoke(Items, key);

		public int Count => Items.Count;
		public string GetKey(int index) => Items.Keys.ElementAt(index);

		public bool CanAdd(string key) => !Items.ContainsKey(key);
		public bool CanAdd(Type type) => type == null || (type == typeof(T) && type.IsValueType) || type.IsCreatableAs<T>();

		public bool AddItem(string key, Type type)
		{
			var item = type == null || type.IsValueType
				? default
				: (T)Activator.CreateInstance(type);

			Items.Add(key, item);
			return true;
		}

		public bool CanRemove(int index, string key) => true;
		public void RemoveItem(int index, string key) => Items.Remove(key);

		public bool IsReorderable => false;
		public void ReorderItem(int from, int to) { }
	}

	public class PropertyDictionaryProxy : IDictionaryProxy
	{
		public Func<string, bool> CanAddKeyCallback;
		public Func<Type, bool> CanAddTypeCallback;
		public Func<string, bool> CanRemoveCallback;

		private readonly SerializedProperty _property;
		private readonly SerializedProperty _keysProperty;
		private readonly SerializedProperty _valuesProperty;
		private readonly PropertyDrawer _drawer;

		public PropertyDictionaryProxy(SerializedProperty property, SerializedProperty keys, SerializedProperty values, PropertyDrawer drawer)
		{
			_property = property;
			_keysProperty = keys;
			_valuesProperty = values;
			_drawer = drawer;
		}

		public VisualElement CreateElement(int index, string key)
		{
			var value = _valuesProperty.GetArrayElementAtIndex(index);
			var field = _drawer?.CreatePropertyGUI(value) ?? value.CreateField();

			field.Bind(_property.serializedObject);

			if (string.IsNullOrEmpty(key))
				key = " "; // An empty label will cause the label to be removed

			field.SetFieldLabel(key); // TODO: for references this should include the type name

			return field;
		}

		public int Count => _keysProperty.arraySize;
		public string GetKey(int index) => _keysProperty.GetArrayElementAtIndex(index).stringValue;

		public bool CanAdd(string key)
		{
			for (var i = 0; i < _keysProperty.arraySize; i++)
			{
				var property = _keysProperty.GetArrayElementAtIndex(i);
				if (property.stringValue == key)
					return false;
			}

			return CanAddKeyCallback != null
				? CanAddKeyCallback.Invoke(key)
				: true;
		}

		public bool CanAdd(Type type)
		{
			return type != null && CanAddTypeCallback != null
				? CanAddTypeCallback.Invoke(type)
				: true;
		}

		public bool AddItem(string key, Type type)
		{
			try
			{
				var newSize = _keysProperty.arraySize + 1;
				_valuesProperty.ResizeArray(newSize);

				if (type != null)
				{
					var newValue = Activator.CreateInstance(type);
					var valueProperty = _valuesProperty.GetArrayElementAtIndex(newSize - 1);

					if (!valueProperty.TrySetValue(newValue))
					{
						_valuesProperty.arraySize = newSize - 1;
						return false;
					}
				}

				_keysProperty.arraySize = newSize;
				var newItem = _keysProperty.GetArrayElementAtIndex(newSize - 1);
				newItem.stringValue = key;

				_property.serializedObject.ApplyModifiedProperties(); // TODO: not applying new reference values for some reason
				return true;
			}
			catch
			{
				// Technically a user could do something really wierd like set the item type on the DictionaryField
				// to Float when the property is actually a string

				// TODO: this also happens if the type is not Serializable (_valuesProperty will be null)
				return false;
			}
		}

		public bool CanRemove(int index, string key)
		{
			return CanRemoveCallback != null
				? CanRemoveCallback.Invoke(key)
				: true;
		}

		public void RemoveItem(int index, string key)
		{
			_keysProperty.RemoveFromArray(index);
			_valuesProperty.RemoveFromArray(index);
			_property.serializedObject.ApplyModifiedProperties();
		}

		public bool IsReorderable
		{
			get => true;
		}

		public void ReorderItem(int from, int to)
		{
			_keysProperty.MoveArrayElement(from, to);
			_valuesProperty.MoveArrayElement(from, to);
			_property.serializedObject.ApplyModifiedProperties();
		}
	}
}
