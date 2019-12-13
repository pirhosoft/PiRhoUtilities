using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public abstract class DictionaryProxy
	{
		public virtual VisualElement CreateElement(int index)
		{
			var element = new VisualElement();
			element.userData = index;
			return element;
		}

		public virtual bool NeedsUpdate(VisualElement item, int index)
		{
			return !(item.userData is int i) || i != index;
		}

		public virtual bool CanAdd(string key) => true;
		public virtual bool CanRemove(int index) => true;

		public abstract int ItemCount { get; }
		public abstract string GetKey(int index);

		public abstract void AddItem(string key);
		public abstract void AddItem(string key, object item);
		public abstract void RemoveItem(int index);
		public abstract void ReorderItem(int from, int to);
	}

	public class IDictionaryProxy : DictionaryProxy
	{
		private IDictionary _items;
		private List<string> _indexMap = new List<string>();

		public IDictionary Items { get => _items; set => SetItems(value); }
		public Type ItemType { get; private set; }
		public Action<string, VisualElement> Creator { get; private set; }

		public IDictionaryProxy(IDictionary items, Action<string, VisualElement> creator)
		{
			Items = items;
			Creator = creator;
		}

		public override VisualElement CreateElement(int index)
		{
			var key = GetKey(index);
			var element = base.CreateElement(index);
			Creator?.Invoke(key, element);
			return element;
		}

		public override int ItemCount => Items.Count;

		public override string GetKey(int index)
		{
			return index >= 0 && index < _indexMap.Count
				? _indexMap[index]
				: string.Empty;
		}

		public override void AddItem(string key)
		{
			if (ItemType != null && ItemType.IsCreatableAs(ItemType))
			{
				_indexMap.Add(key);
				Items.Add(key, Activator.CreateInstance(ItemType));
			}
		}

		public override void AddItem(string key, object item)
		{
			if (ItemType == null || ItemType.IsAssignableFrom(item.GetType()))
			{
				_indexMap.Add(key);
				Items.Add(key, item);
			}
		}

		public override void RemoveItem(int index)
		{
			var key = GetKey(index);
			Items.Remove(key);
			_indexMap.RemoveAt(index);
		}

		private void SetItems(IDictionary items)
		{
			_items = items;
			_indexMap.Clear();

			foreach (var item in Items.Keys)
				_indexMap.Add(item.ToString());
		}

		public override void ReorderItem(int from, int to)
		{
		}
	}

	public class DictionaryProxy<T> : DictionaryProxy
	{
		private Dictionary<string, T> _items = new Dictionary<string, T>();
		private List<string> _indexMap = new List<string>();

		public Dictionary<string, T> Items { get => _items; set => SetItems(value); }
		public Action<string, VisualElement> Creator { get; private set; }

		public DictionaryProxy(Dictionary<string, T> items, Action<string, VisualElement> creator)
		{
			Items = items;
			Creator = creator;
		}

		public override VisualElement CreateElement(int index)
		{
			var key = GetKey(index);
			var element = base.CreateElement(index);
			Creator?.Invoke(key, element);
			return element;
		}

		public override int ItemCount => Items.Count;

		public override string GetKey(int index)
		{
			return index >= 0 && index < _indexMap.Count
				? _indexMap[index]
				: string.Empty;
		}

		public override void AddItem(string key)
		{
			_indexMap.Add(key);
			Items.Add(key, default);
		}

		public override void AddItem(string key, object item)
		{
			if (item is T t)
			{
				_indexMap.Add(key);
				Items.Add(key, t);
			}
		}

		public override void RemoveItem(int index)
		{
			var key = GetKey(index);
			Items.Remove(key);
			_indexMap.RemoveAt(index);
		}

		private void SetItems(Dictionary<string, T> items)
		{
			_items = items;
			_indexMap.Clear();

			foreach (var item in Items)
				_indexMap.Add(item.Key);
		}

		public override void ReorderItem(int from, int to)
		{
		}
	}

	public class PropertyDictionaryProxy : DictionaryProxy
	{
		public Func<string, bool> CanAddCallback;
		public Func<string, bool> CanRemoveCallback;

		private SerializedProperty _property;
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

		public override VisualElement CreateElement(int index)
		{
			var key = _keysProperty.GetArrayElementAtIndex(index);
			var value = _valuesProperty.GetArrayElementAtIndex(index);
			var field = _drawer?.CreatePropertyGUI(value) ?? value.CreateField();

			if (field == null) // this happens when a ManagedReference type name changes
				return new VisualElement();

			field.userData = index;
			field.Bind(_property.serializedObject);
			field.SetFieldLabel(key.stringValue); // TODO: for references this should include the type name

			return field;
		}

		public override bool CanAdd(string key)
		{
			for (var i = 0; i < _keysProperty.arraySize; i++)
			{
				var property = _keysProperty.GetArrayElementAtIndex(i);
				if (property.stringValue == key)
					return false;
			}

			return CanAddCallback != null
				? CanAddCallback.Invoke(key)
				: base.CanAdd(key);
		}

		public override bool CanRemove(int index)
		{
			var property = _keysProperty.GetArrayElementAtIndex(index);

			return CanRemoveCallback != null
				? CanRemoveCallback.Invoke(property.stringValue)
				: base.CanRemove(index);
		}

		public override int ItemCount
		{
			get => _keysProperty.arraySize;
		}

		public override string GetKey(int index)
		{
			return index >= 0 && index < _keysProperty.arraySize
				? _keysProperty.GetArrayElementAtIndex(index).stringValue
				: string.Empty;
		}

		public override void AddItem(string key)
		{
			PushItem(key);
			_property.serializedObject.ApplyModifiedProperties();
		}

		public override void AddItem(string key, object item)
		{
			var index = PushItem(key);
			_valuesProperty.GetArrayElementAtIndex(index).TrySetValue(item);
			_property.serializedObject.ApplyModifiedProperties();
		}

		private int PushItem(string key)
		{
			var newSize = _keysProperty.arraySize + 1;

			_keysProperty.arraySize = newSize;
			_valuesProperty.ResizeArray(newSize);

			var newItem = _keysProperty.GetArrayElementAtIndex(newSize - 1);
			newItem.stringValue = key;

			return newSize - 1;
		}

		public override void RemoveItem(int index)
		{
			_keysProperty.RemoveFromArray(index);
			_valuesProperty.RemoveFromArray(index);
			_property.serializedObject.ApplyModifiedProperties();
		}

		public override void ReorderItem(int from, int to)
		{
			_keysProperty.MoveArrayElement(from, to);
			_valuesProperty.MoveArrayElement(from, to);
			_property.serializedObject.ApplyModifiedProperties();
		}
	}
}
