using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public interface IListProxy
	{
		VisualElement CreateElement(int index);

		int Count { get; }

		bool CanAdd();
		bool CanAdd(Type type);
		bool AddItem(Type type);

		bool CanRemove(int index);
		void RemoveItem(int index);
		
		void ReorderItem(int from, int to);
	}

	public class ListProxy : IListProxy
	{
		public IList Items { get; private set; }
		public Func<IList, int, VisualElement> Creator { get; private set; }

		public ListProxy(IList items, Func<IList, int, VisualElement> creator)
		{
			Items = items;
			Creator = creator;
		}

		public VisualElement CreateElement(int index) => Creator.Invoke(Items, index);

		public int Count => Items.Count;

		public bool CanAdd() => true;
		public bool CanAdd(Type type) => type != null;

		public bool AddItem(Type type)
		{
			try
			{
				var item = Activator.CreateInstance(type);
				Items.Add(item);
			}
			catch
			{
				return false;
			}

			return true;
		}

		public bool CanRemove(int index) => true;
		public void RemoveItem(int index) => Items.RemoveAt(index);

		public void ReorderItem(int from, int to)
		{
			var item = Items[from];
			Items.RemoveAt(from);
			Items.Insert(to, item);
		}
	}

	public class ListProxy<T> : IListProxy
	{
		public IList<T> Items { get; private set; }
		public Func<IList<T>, int, VisualElement> Creator { get; private set; }

		public ListProxy(IList<T> items, Func<IList<T>, int, VisualElement> creator)
		{
			Items = items;
			Creator = creator;
		}

		public VisualElement CreateElement(int index) => Creator.Invoke(Items, index);

		public int Count => Items.Count;

		public bool CanAdd() => true;
		public bool CanAdd(Type type) => type == null || (type == typeof(T) && type.IsValueType) || type.IsCreatableAs<T>();

		public bool AddItem(Type type)
		{
			var item = type == null || type.IsValueType
				? default
				: (T)Activator.CreateInstance(type);

			Items.Add(item);
			return true;
		}

		public bool CanRemove(int index) => true;
		public void RemoveItem(int index) => Items.RemoveAt(index);

		public void ReorderItem(int from, int to)
		{
			var item = Items[from];
			Items.RemoveAt(from);
			Items.Insert(to, item);
		}
	}

	public class PropertyListProxy : IListProxy
	{
		public Func<bool> CanAddCallback;
		public Func<Type, bool> CanAddTypeCallback;
		public Func<int, bool> CanRemoveCallback;

		private readonly SerializedProperty _property;
		private readonly PropertyDrawer _drawer;

		public PropertyListProxy(SerializedProperty property, PropertyDrawer drawer)
		{
			_property = property;
			_drawer = drawer;
		}

		public VisualElement CreateElement(int index)
		{
			var property = _property.GetArrayElementAtIndex(index);
			var field = _drawer?.CreatePropertyGUI(property) ?? property.CreateField();
			field.Bind(_property.serializedObject);

			if (!(field is Foldout))
				field.SetFieldLabel(null); // TODO: for references this should be the type name

			return field;
		}

		public int Count => _property.arraySize;

		public bool CanAdd()
		{
			return CanAddCallback != null
				? CanAddCallback.Invoke()
				: true;
		}

		public bool CanAdd(Type type)
		{
			return type != null && CanAddTypeCallback != null
				? CanAddTypeCallback.Invoke(type)
				: true;
		}

		public bool AddItem(Type type)
		{
			try
			{
				var newSize = _property.arraySize + 1;
				_property.ResizeArray(newSize);
				if (type != null)
				{
					var newValue = Activator.CreateInstance(type);
					var valueProperty = _property.GetArrayElementAtIndex(newSize - 1);

					if (!valueProperty.TrySetValue(newValue))
					{
						_property.arraySize = newSize - 1;
						return false;
					}
				}
				_property.serializedObject.ApplyModifiedProperties(); // TODO: not applying new reference values for some reason
				return true;
			}
			catch
			{
				// Technically a user could do something really wierd like set the item type on the DictionaryField
				// to Float when the property is actually a string

				// TODO: this also happens if the type is not Serializable (_property will be null)
				return false;
			}
		}

		public bool CanRemove(int index)
		{
			return CanRemoveCallback != null
				? CanRemoveCallback.Invoke(index)
				: true;
		}
		
		public void RemoveItem(int index)
		{
			_property.RemoveFromArray(index);
			_property.serializedObject.ApplyModifiedProperties();
		}

		public void ReorderItem(int from, int to)
		{
			_property.MoveArrayElement(from, to);
			_property.serializedObject.ApplyModifiedProperties();
		}
	}
}
