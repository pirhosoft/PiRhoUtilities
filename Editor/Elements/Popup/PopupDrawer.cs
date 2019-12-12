using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(PopupAttribute))]
	class PopupDrawer : PropertyDrawer
	{
		private const string _invalidTypeError = "(PUPDIT) invalid type for PopupAttribute on field '{0}': Popup can only be applied to int, float, or string fields";
		private const string _invalidValuesError = "(PUPDIIV) invalid values for PopupAttribute on field '{0}': Popup on fields must specify an a {1}[] or a method/property/field that returns a List<{1}>";
		private const string _invalidOptionsWarning = "(PUPDIOW) invalid options for PopupAttribute on field '{0}': the number of Options does not match the number of Values";
		private const string _missingWarning = "(PUPDDMC) invalid {0} method, field, or property for PopupAttribute on field '{1}': '{2}' could not be found on type '{3}'";
		private const string _invalidReturnWarning = "(PUPDIMR) invalid {0} method, field, or property for PopupAttribute on field '{0}': '{1}' should return a '{2}'";
		private const string _invalidParametersWarning = "(PUPDIMP) invalid {0} method for PopupAttribute on field '{1}': '{2}' should take no parameters";
		private const string _missingValueWarning = "(PUPDMV) the value of field '{0}' did not exsist in the list of values: Changing to the first valid value";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var popup = attribute as PopupAttribute;

			if (property.propertyType == SerializedPropertyType.Integer)
			{
				var value = property.intValue;
				var field = CreatePopup(property, popup, popup.IntValues, ref value);
				if (field != null)
				{
					property.intValue = value;
					property.serializedObject.ApplyModifiedProperties();
					return field.ConfigureProperty(property);
				}
			}
			else if (property.propertyType == SerializedPropertyType.Float)
			{
				var value = property.floatValue;
				var field = CreatePopup(property, popup, popup.FloatValues, ref value);
				if (field != null)
				{
					property.floatValue = value;
					property.serializedObject.ApplyModifiedProperties();
					return field.ConfigureProperty(property);
				}
			}
			else if (property.propertyType == SerializedPropertyType.String)
			{
				var value = property.stringValue;
				var field = CreatePopup(property, popup, popup.StringValues, ref value);
				if (field != null)
				{
					property.stringValue = value;
					property.serializedObject.ApplyModifiedProperties();
					return field.ConfigureProperty(property);
				}
			}
			else
			{
				Debug.LogErrorFormat(_invalidTypeError, property.propertyPath);
			}

			return new FieldContainer(property.displayName);
		}

		private BaseField<T> CreatePopup<T>(SerializedProperty property, PopupAttribute popup, List<T> defaultValues, ref T defaultValue)
		{
			var values = GetList(property, popup.ValuesMethod, defaultValues, "values");
			var options = GetList(property, popup.OptionsMethod, popup.Options, "options");

			if (values != null && values.Count > 0)
			{
				if (options != null && options.Count != values.Count)
					Debug.LogWarningFormat(_invalidOptionsWarning, property.propertyPath);

				if (!values.Contains(defaultValue))
				{
					defaultValue = values[0];
					Debug.LogWarningFormat(_missingValueWarning, property.propertyPath);
				}

				return new PopupField<T>(property.displayName, values, defaultIndex: 0, value => Format(value, values, options), value => Format(value, values, options));
			}
			else
			{
				Debug.LogErrorFormat(_invalidValuesError, property.propertyPath, nameof(T));
				return null;
			}
		}

		private string Format<T>(T value, List<T> values, List<string> options)
		{
			var index = values.IndexOf(value);

			if (options == null || index < 0 || index >= options.Count)
				return value.ToString();

			return options[index];
		}

		private List<FieldType> GetList<FieldType>(SerializedProperty property, string methodName, List<FieldType> defaultValues, string warningType)
		{
			if (!string.IsNullOrEmpty(methodName))
			{
				var method = fieldInfo.DeclaringType.GetMethod(methodName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				var field = fieldInfo.DeclaringType.GetField(methodName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				var prop = fieldInfo.DeclaringType.GetProperty(methodName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

				if (method != null)
				{
					if (method.ReturnType != typeof(List<FieldType>))
						Debug.LogWarningFormat(_invalidReturnWarning, warningType, property.propertyPath, methodName, nameof(List<FieldType>));
					else if (!method.HasSignature(null))
						Debug.LogWarningFormat(_invalidParametersWarning, warningType, property.propertyPath, methodName);
					else
						return (List<FieldType>)method.Invoke(method.IsStatic ? null : property.GetOwner<object>(), null);
				}
				else if (field != null)
				{
					if (field.FieldType != typeof(List<FieldType>))
						Debug.LogWarningFormat(_invalidReturnWarning, warningType, property.propertyPath, methodName, nameof(List<FieldType>));
					else
						return (List<FieldType>)field.GetValue(field.IsStatic ? null : property.GetOwner<object>());
				}
				else if (prop != null)
				{
					if (prop.PropertyType != typeof(List<FieldType>) || !prop.CanRead)
						Debug.LogWarningFormat(_invalidReturnWarning, warningType, property.propertyPath, methodName, nameof(List<FieldType>));
					else
						return (List<FieldType>)prop.GetValue(prop.GetGetMethod(true).IsStatic ? null : property.GetOwner<object>());
				}
				else
				{
					Debug.LogWarningFormat(_missingWarning, warningType, property.propertyPath, methodName, fieldInfo.DeclaringType.Name);
				}
			}

			return defaultValues;
		}
	}
}