using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(EnumButtonsAttribute))]
	class EnumButtonsDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUEBDIT) invalid type for EnumButtonsAttribute on field '{0}': EnumButtons can only be applied to Enum fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			if (property.propertyType == SerializedPropertyType.Enum)
			{
				var type = this.GetFieldType();
				var tooltip = this.GetTooltip();
				var value = Enum.ToObject(type, property.intValue) as Enum;
				var flags = (attribute as EnumButtonsAttribute).Flags;
				var field = new EnumButtonsField(property.displayName, value, flags);

				return field.ConfigureProperty(property, tooltip);
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
				return new FieldContainer(property.displayName);
			}
		}
	}
}