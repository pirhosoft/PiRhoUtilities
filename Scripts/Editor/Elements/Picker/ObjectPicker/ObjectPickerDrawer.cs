using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(ObjectPickerAttribute))]
	public class ObjectPickerDrawer : PropertyDrawer
	{
		private const string _invalidPropertyTypeWarning = "(PUOPDIPT) invalid type for ObjectPickerAttribute on field {0}: ObjectPicker can only be applied to Object or derived fields";
		private const string _invalidBaseTypeWarning = "(PUOPDIBT) invalid base type for ObjectPickerAttribute on field {0}: the field type must creatable as the base type";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			if (property.propertyType == SerializedPropertyType.ObjectReference)
			{
				var fieldType = this.GetFieldType();
				var baseType = (attribute as ObjectPickerAttribute)?.BaseType ?? fieldType;

				if (fieldType.IsCreatableAs(baseType))
				{
					var tooltip = this.GetTooltip();
					var field = new ObjectPickerField(property.displayName, property.objectReferenceValue, property.serializedObject.targetObject, baseType);
					return field.ConfigureProperty(property, tooltip);
				}
				else
				{
					Debug.LogWarningFormat(_invalidPropertyTypeWarning, property.propertyPath);
				}
			}
			else
			{
				Debug.LogWarningFormat(_invalidPropertyTypeWarning, property.propertyPath);
			}

			return new FieldContainer(property.displayName);
		}
	}
}
