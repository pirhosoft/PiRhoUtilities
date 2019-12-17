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
			var minimumAttribute = attribute as MinimumAttribute;
			var element = this.CreateNextElement(property);

			if (property.propertyType == SerializedPropertyType.Integer)
			{
				var min = ReflectionHelper.CreateValueSourceFunction(property, element, fieldInfo.DeclaringType, minimumAttribute.MinimumSource, ReflectionSource.All, Mathf.RoundToInt(minimumAttribute.Minimum), nameof(MinimumAttribute), nameof(MinimumAttribute.MinimumSource));

				Clamp(property, min());
				element.RegisterCallback<FocusOutEvent>(e => Clamp(property, min()));
			}
			else if (property.propertyType == SerializedPropertyType.Float)
			{
				var min = ReflectionHelper.CreateValueSourceFunction(property, element, fieldInfo.DeclaringType, minimumAttribute.MinimumSource, ReflectionSource.All, minimumAttribute.Minimum, nameof(MinimumAttribute), nameof(MinimumAttribute.MinimumSource));

				Clamp(property, min());
				element.RegisterCallback<FocusOutEvent>(e => Clamp(property, min()));
			}
			else
			{
				Debug.LogWarningFormat(property.serializedObject.targetObject, _invalidTypeWarning, property.propertyPath);
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