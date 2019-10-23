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
				return new EnumButtonsField(property, (attribute as EnumButtonsAttribute).Flags);
			else
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);

			return new FieldContainer(property.displayName);
		}
	}
}