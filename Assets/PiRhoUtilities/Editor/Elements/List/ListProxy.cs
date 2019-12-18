using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public abstract class ListProxy
	{
		public virtual VisualElement CreateElement(int index)
		{
			var element = new VisualElement
			{
				userData = index
			};

			return element;
		}

		public virtual bool NeedsUpdate(VisualElement item, int index)
		{
			return !(item.userData is int i) || i != index;
		}

		public virtual bool CanAdd() => true;
		public virtual bool CanRemove(int index) => true;

		public abstract int ItemCount { get; }

		public abstract void AddItem();
		public abstract void AddItem(object item);
		public abstract void RemoveItem(int index);
		public abstract void ReorderItem(int from, int to);
	}

	public class IListProxy : ListProxy
	{
		public IList Items { get; private set; }
		public Type ItemType { get; private set; }
		public Action<VisualElement, int> Creator { get; private set; }

		public IListProxy(IList items, Type itemType, Action<VisualElement, int> creator)
		{
			Items = items;
			ItemType = itemType;
			Creator = creator;
		}

		public override VisualElement CreateElement(int index)
		{
			var element = base.CreateElement(index);
			Creator?.Invoke(element, index);
			return element;
		}

		public override int ItemCount => Items.Count;

		public override void AddItem()
		{
			if (ItemType != null && ItemType.IsCreatable())
				Items.Add(Activator.CreateInstance(ItemType));
		}

		public override void AddItem(object item)
		{
			if (ItemType == null || ItemType.IsAssignableFrom(item.GetType()))
				Items.Add(item);
		}

		public override void RemoveItem(int index)
		{
			Items.RemoveAt(index);
		}

		public override void ReorderItem(int from, int to)
		{
			var item = Items[from];
			Items.RemoveAt(from);
			Items.Insert(to, item);
		}
	}

	public class ListProxy<T> : ListProxy
	{
		public List<T> Items { get; private set; }
		public Action<VisualElement> Creator { get; private set; }

		public ListProxy(List<T> items, Action<VisualElement> creator)
		{
			Items = items;
			Creator = creator;
		}

		public override VisualElement CreateElement(int index)
		{
			var element = base.CreateElement(index);
			Creator?.Invoke(element);
			return element;
		}

		public override int ItemCount => Items.Count;

		public override void AddItem()
		{
			Items.Add(default);
		}

		public override void AddItem(object item)
		{
			if (item is T t)
				Items.Add(t);
		}

		public override void RemoveItem(int index)
		{
			Items.RemoveAt(index);
		}

		public override void ReorderItem(int from, int to)
		{
			var item = Items[from];
			Items.RemoveAt(from);
			Items.Insert(to, item);
		}
	}

	public class PropertyListProxy : ListProxy
	{
		public Func<bool> CanAddCallback;
		public Func<int, bool> CanRemoveCallback;

		private readonly SerializedProperty _property;
		private readonly PropertyDrawer _drawer;

		public PropertyListProxy(SerializedProperty property, PropertyDrawer drawer)
		{
			_property = property;
			_drawer = drawer;
		}

		public override VisualElement CreateElement(int index)
		{
			var property = _property.GetArrayElementAtIndex(index);
			var field = _drawer?.CreatePropertyGUI(property) ?? property.CreateField();

			if (field == null) // this happens when a ManagedReference type name changes
				return new VisualElement();

			field.userData = index;
			field.Bind(_property.serializedObject);

			if (!(field is Foldout))
				field.SetFieldLabel(null); // TODO: for references this should be the type name

			return field;
		}

		public override bool CanAdd()
		{
			return CanAddCallback != null
				? CanAddCallback.Invoke()
				: base.CanAdd();
		}

		public override bool CanRemove(int index)
		{
			return CanRemoveCallback != null
				? CanRemoveCallback.Invoke(index)
				: base.CanRemove(index);
		}

		public override int ItemCount
		{
			get => _property.arraySize;
		}

		public override void AddItem()
		{
			_property.ResizeArray(_property.arraySize + 1);
			_property.serializedObject.ApplyModifiedProperties();
		}

		public override void AddItem(object item)
		{
			_property.ResizeArray(_property.arraySize + 1);
			_property.GetArrayElementAtIndex(_property.arraySize - 1).TrySetValue(item);
			_property.serializedObject.ApplyModifiedProperties();
		}

		public override void RemoveItem(int index)
		{
			_property.RemoveFromArray(index);
			_property.serializedObject.ApplyModifiedProperties();
		}

		public override void ReorderItem(int from, int to)
		{
			_property.MoveArrayElement(from, to);
			_property.serializedObject.ApplyModifiedProperties();
		}
	}
}
