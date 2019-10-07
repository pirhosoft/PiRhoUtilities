using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(MinimumAttribute))]
	class MinimumDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUMNDIT) invalid type for MinimumAttribute on field {0}: Minimum can only be applied to int or float fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var minimum = (attribute as MinimumAttribute).Minimum;
			var element = this.CreateNextElement(property);

			if (property.propertyType == SerializedPropertyType.Integer)
			{
				Clamp(property, Mathf.RoundToInt(minimum));
				element.RegisterCallback<FocusOutEvent>(e => Clamp(property, Mathf.RoundToInt(minimum)));
			}
			else if (property.propertyType == SerializedPropertyType.Float)
			{
				Clamp(property, minimum);
				element.RegisterCallback<FocusOutEvent>(e => Clamp(property, minimum));
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
			}

			return element;
		}

		private void Clamp(SerializedProperty property, int minimum)
		{
			property.intValue = Mathf.Max(minimum, property.intValue);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void Clamp(SerializedProperty property, float minimum)
		{
			property.floatValue = Mathf.Max(minimum, property.floatValue);
			property.serializedObject.ApplyModifiedProperties();
		}
	}
}