using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class PropertyReferenceDrawer : IReferenceDrawer
	{
		private SerializedProperty _property;
		private PropertyDrawer _drawer;

		public PropertyReferenceDrawer(SerializedProperty property, PropertyDrawer drawer)
		{
			_property = property;
			_drawer = drawer;
		}

		public VisualElement CreateElement(object value)
		{
			if (_drawer != null)
			{
				return _drawer.CreatePropertyGUI(_property);
			}
			else
			{
				var container = new VisualElement();

				foreach (var child in _property.Children())
				{
					var field = new PropertyField(child);
					field.Bind(_property.serializedObject); // this is only called automatically when the inspector is first created
					container.Add(field);
				}

				return container;
			}
		}
	}
}
