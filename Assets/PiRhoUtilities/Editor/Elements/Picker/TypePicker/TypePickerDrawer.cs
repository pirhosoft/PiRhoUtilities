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
				var picker = new TypePickerField(property.displayName, property.stringValue, typeAttribute.BaseType, typeAttribute.ShowAbstract);
				var tooltip = this.GetTooltip();

				return picker.ConfigureProperty(property, tooltip);
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
				return new FieldContainer(property.displayName);
			}
		}
	}
}
