using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(MaximumAttribute))]
	class MaximumDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUMXDIT) invalid type for MaximumAttribute on field {0}: Maximum can only be applied to int or float fields";
		private const string _invalidSourceError = "(PUMXDIS) invalid source for MaximumAttribute on field '{0}': a field, method, or property of type '{1}' named '{2}' could not be found";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var maximumAttribute = attribute as MaximumAttribute;
			var element = this.CreateNextElement(property);

			if (property.propertyType == SerializedPropertyType.Integer)
				SetupMaximum(maximumAttribute.MaximumSource, property, element, Mathf.RoundToInt(maximumAttribute.Maximum), ClampInt);
			else if (property.propertyType == SerializedPropertyType.Float)
				SetupMaximum(maximumAttribute.MaximumSource, property, element, maximumAttribute.Maximum, ClampFloat);
			else
				Debug.LogWarningFormat(property.serializedObject.targetObject, _invalidTypeWarning, property.propertyPath);

			return element;
		}

		private void SetupMaximum<T>(string sourceName, SerializedProperty property, VisualElement element, T defaultValue, Action<SerializedProperty, T> clamp)
		{
			var max = ReflectionHelper.CreateValueSourceFunction(sourceName, property, element, fieldInfo.DeclaringType, defaultValue);

			if (max != null)
			{
				clamp(property, max());
				element.RegisterCallback<FocusOutEvent>(e => clamp(property, max()));
			}
			else
			{
				Debug.LogWarningFormat(_invalidSourceError, property.propertyPath, nameof(T), sourceName);
			}
		}

		private void ClampInt(SerializedProperty property, int maximum)
		{
			property.intValue = Mathf.Min(maximum, property.intValue);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void ClampFloat(SerializedProperty property, float maximum)
		{
			property.floatValue = Mathf.Min(maximum, property.floatValue);
			property.serializedObject.ApplyModifiedProperties();
		}
	}
}