using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class PropertyListProxy : IListProxy
	{
		public Func<bool> CanAddCallback;
		public Func<int, bool> CanRemoveCallback;
		public Func<int, int, bool> CanReorderCallback;

		public Action AddCallback;
		public Action<int> RemoveCallback;
		public Action<int, int> ReorderCallback;

		private SerializedProperty _property;
		private readonly PropertyDrawer _drawer;

		public int ItemCount => _property.arraySize;

		public string Label { get; set; }
		public string Tooltip { get; set; }
		public string EmptyLabel { get; set; } = ListProxy.DefaultEmptyLabel;
		public string EmptyTooltip { get; set; } = ListProxy.DefaultEmptyTooltip;
		public string AddTooltip { get; set; } = ListProxy.DefaultAddTooltip;
		public string RemoveTooltip { get; set; } = ListProxy.DefaultRemoveTooltip;
		public string ReorderTooltip { get; set; } = ListProxy.DefaultReorderTooltip;

		public bool AllowAdd { get; set; } = true;
		public bool AllowRemove { get; set; } = true;
		public bool AllowReorder { get; set; } = true;

		public bool CanAdd() => CanAddCallback?.Invoke() ?? AllowAdd;
		public bool CanRemove(int index) => CanRemoveCallback?.Invoke(index) ?? AllowRemove;
		public bool CanReorder(int from, int to) => CanReorderCallback?.Invoke(from, to) ?? AllowReorder;

		public PropertyListProxy(SerializedProperty property, PropertyDrawer drawer)
		{
			_property = property;
			_drawer = drawer;
		}

		public VisualElement CreateElement(int index)
		{
			var property = _property.GetArrayElementAtIndex(index);
			var field = _drawer?.CreatePropertyGUI(property) ?? property.CreateField();

			field.userData = index;
			field.Bind(_property.serializedObject);

			if (field is PropertyField propertyField)
				propertyField.SetLabel(null);
			else if (field is FieldContainer fieldContainer)
				fieldContainer.SetLabel(null);
			else if (field.GetType().InheritsGeneric(typeof(BaseField<>)))
				BaseFieldExtensions.SetLabel(field, null);

			return field;
		}

		public bool NeedsUpdate(VisualElement item, int index)
		{
			return !(item.userData is int i) || i != index;
		}

		public void AddItem()
		{
			_property.ResizeArray(_property.arraySize + 1);
			_property.serializedObject.ApplyModifiedProperties();

			AddCallback?.Invoke();
		}

		public void RemoveItem(int index)
		{
			RemoveCallback?.Invoke(index);

			_property.RemoveFromArray(index);
			_property.serializedObject.ApplyModifiedProperties();
		}

		public void ReorderItem(int from, int to)
		{
			_property.MoveArrayElement(from, to);
			_property.serializedObject.ApplyModifiedProperties();

			ReorderCallback?.Invoke(from, to);
		}
	}
}