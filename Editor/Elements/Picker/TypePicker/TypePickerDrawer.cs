using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(TypePickerAttribute))]
	public class TypePickerDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUTPDIT) Invalid type for TypePickerAttribute on field {0}: TypePicker can only be applied to string fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			if (property.propertyType == SerializedPropertyType.String)
			{
				var typeAttribute = attribute as TypePickerAttribute;
				return new TypePickerField(property, typeAttribute.BaseType, typeAttribute.ShowAbstract);
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
				return new FieldContainer(property.displayName);
			}
		}
	}
}
