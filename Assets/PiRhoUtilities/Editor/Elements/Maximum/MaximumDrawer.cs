using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(MaximumAttribute))]
	class MaximumDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUMXDIT) invalid type for MaximumAttribute on field {0}: Maximum can only be applied to int or float fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var maximumAttribute = attribute as MaximumAttribute;
			var element = this.CreateNextElement(property);

			if (property.propertyType == SerializedPropertyType.Integer)
			{
				var max = ReflectionHelper.CreateValueSourceFunction(property, element, fieldInfo.DeclaringType, maximumAttribute.MaximumSource, Mathf.RoundToInt(maximumAttribute.Maximum), nameof(MaximumAttribute), nameof(MaximumAttribute.MaximumSource));

				Clamp(property, max());
				element.RegisterCallback<FocusOutEvent>(e => Clamp(property, max()));
			}
			else if (property.propertyType == SerializedPropertyType.Float)
			{
				var max = ReflectionHelper.CreateValueSourceFunction(property, element, fieldInfo.DeclaringType, maximumAttribute.MaximumSource, maximumAttribute.Maximum, nameof(MaximumAttribute), nameof(MaximumAttribute.MaximumSource));

				Clamp(property, max());
				element.RegisterCallback<FocusOutEvent>(e => Clamp(property, max()));
			}
			else
			{
				Debug.LogWarningFormat(property.serializedObject.targetObject, _invalidTypeWarning, property.propertyPath);
			}

			return element;
		}

		private void Clamp(SerializedProperty property, int maximum)
		{
			property.intValue = Mathf.Min(maximum, property.intValue);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void Clamp(SerializedProperty property, float maximum)
		{
			property.floatValue = Mathf.Min(maximum, property.floatValue);
			property.serializedObject.ApplyModifiedProperties();
		}
	}
}