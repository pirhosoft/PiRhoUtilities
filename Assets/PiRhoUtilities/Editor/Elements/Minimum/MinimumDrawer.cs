using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(MinimumAttribute))]
	class MinimumDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUMNDIT) invalid type for MinimumAttribute on field {0}: Minimum can only be applied to int or float fields";
		private const string _missingCompareWarning = "(PUMNDMC) invalid method, field, or property for MinimumAttribute on field '{0}': '{1}' could not be found on type '{2}'";
		private const string _invalidMethodReturnWarning = "(PUMNIMR) invalid method for MinimumAttribute on field '{0}': the method '{1}' should return a '{2}'";
		private const string _invalidMethodParametersWarning = "(PUMNIMP) invalid method for MinimumAttribute on field '{0}': the method '{1}' should take no parameters";
		private const string _invalidFieldReturnWarning = "(PUMNIFR) invalid field for MinimumAttribute on field '{0}': the method '{1}' should return an {2}";
		private const string _invalidPropertyReturnWarning = "(PUMNIPR) invalid property for MinimumAttribute on field '{0}': the method '{1}' should return an {2}";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var minimumAttribute = attribute as MinimumAttribute;
			var element = this.CreateNextElement(property);

			if (property.propertyType == SerializedPropertyType.Integer)
			{
				var min = GetMin(property, minimumAttribute, Mathf.RoundToInt(minimumAttribute.Minimum));

				Clamp(property, min());
				element.RegisterCallback<FocusOutEvent>(e => Clamp(property, min()));
			}
			else if (property.propertyType == SerializedPropertyType.Float)
			{
				var min = GetMin(property, minimumAttribute, minimumAttribute.Minimum);

				Clamp(property, min());
				element.RegisterCallback<FocusOutEvent>(e => Clamp(property, min()));
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

		private Func<FieldType> GetMin<FieldType>(SerializedProperty property, MinimumAttribute minimumAttribute, FieldType defaultValue)
		{
			if (!string.IsNullOrEmpty(minimumAttribute.Compare))
			{
				var method = fieldInfo.DeclaringType.GetMethod(minimumAttribute.Compare, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				var field = fieldInfo.DeclaringType.GetField(minimumAttribute.Compare, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				var prop = fieldInfo.DeclaringType.GetProperty(minimumAttribute.Compare, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

				if (method != null)
				{
					if (method.ReturnType != typeof(FieldType))
						Debug.LogWarningFormat(_invalidMethodReturnWarning, property.propertyPath, minimumAttribute.Compare, property.type);
					else if (!method.HasSignature(null))
						Debug.LogWarningFormat(_invalidMethodParametersWarning, property.propertyPath, minimumAttribute.Compare);
					else
						return () => (FieldType)method.Invoke(method.IsStatic ? null : property.GetOwner<object>(), null);
				}
				else if (field != null)
				{
					if (field.FieldType != typeof(FieldType))
						Debug.LogWarningFormat(_invalidFieldReturnWarning, property.propertyPath, minimumAttribute.Compare, property.type);
					else
						return () => (FieldType)field.GetValue(field.IsStatic ? null : property.GetOwner<object>());
				}
				else if (prop != null)
				{
					if (prop.PropertyType != typeof(FieldType) || !prop.CanRead)
						Debug.LogWarningFormat(_invalidPropertyReturnWarning, property.propertyPath, minimumAttribute.Compare, property.type);
					else
						return () => (FieldType)prop.GetValue(prop.GetGetMethod().IsStatic ? null : property.GetOwner<object>());
				}
				else
				{
					Debug.LogWarningFormat(_missingCompareWarning, property.propertyPath, minimumAttribute.Compare, fieldInfo.DeclaringType.Name);
				}
			}

			return () => defaultValue;
		}
	}
}