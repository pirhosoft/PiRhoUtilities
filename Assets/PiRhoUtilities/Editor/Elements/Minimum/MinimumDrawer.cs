using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(MinimumAttribute))]
	class MinimumDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUMNDIT) invalid type for MinimumAttribute on field {0}: Minimum can only be applied to int or float fields";
		private const string _invalidSourceError = "(PUMXDIS) invalid source for MinimumAttribute on field '{0}': a field, method, or property of type '{1}' named '{2}' could not be found";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var minimumAttribute = attribute as MinimumAttribute;
			var element = this.CreateNextElement(property);

			if (property.propertyType == SerializedPropertyType.Integer)
				SetupMinimum(minimumAttribute.MinimumSource, property, element, Mathf.RoundToInt(minimumAttribute.Minimum), ClampInt);
			else if (property.propertyType == SerializedPropertyType.Float)
				SetupMinimum(minimumAttribute.MinimumSource, property, element, minimumAttribute.Minimum, ClampFloat);
			else
				Debug.LogWarningFormat(property.serializedObject.targetObject, _invalidTypeWarning, property.propertyPath);

			return element;
		}

		private void SetupMinimum<T>(string sourceName, SerializedProperty property, VisualElement element, T defaultValue, Action<SerializedProperty, T> clamp)
		{
			var min = ReflectionHelper.CreateValueSourceFunction(sourceName, property, element, fieldInfo.DeclaringType, defaultValue);

			if (min != null)
			{
				clamp(property, min());
				element.RegisterCallback<FocusOutEvent>(e => clamp(property, min()));
			}
			else
			{
				Debug.LogWarningFormat(_invalidSourceError, property.propertyPath, nameof(T), sourceName);
			}
		}

		private void ClampInt(SerializedProperty property, int minimum)
		{
			property.intValue = Mathf.Max(minimum, property.intValue);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void ClampFloat(SerializedProperty property, float minimum)
		{
			property.floatValue = Mathf.Max(minimum, property.floatValue);
			property.serializedObject.ApplyModifiedProperties();
		}
	}
}