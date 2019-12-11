using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(MaximumAttribute))]
	class MaximumDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUMXDIT) invalid type for MaximumAttribute on field {0}: Maximum can only be applied to int or float fields";
		private const string _missingCompareWarning = "(PUMXDMC) invalid method, field, or property for MaximumAttribute on field '{0}': '{1}' could not be found on type '{2}'";
		private const string _invalidMethodReturnWarning = "(PUMXIMR) invalid method for MaximumAttribute on field '{0}': the method '{1}' should return a '{2}'";
		private const string _invalidMethodParametersWarning = "(PUMXIMP) invalid method for MaximumAttribute on field '{0}': the method '{1}' should take no parameters";
		private const string _invalidFieldReturnWarning = "(PUMXIFR) invalid field for MaximumAttribute on field '{0}': the field '{1}' should be a {2}";
		private const string _invalidPropertyReturnWarning = "(PUMXIPR) invalid property for MaximumAttribute on field '{0}': the property '{1}' should be a {2}";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var maximumAttribute = attribute as MaximumAttribute;
			var element = this.CreateNextElement(property);

			if (property.propertyType == SerializedPropertyType.Integer)
			{
				var max = GetMax(property, maximumAttribute, Mathf.RoundToInt(maximumAttribute.Maximum));

				Clamp(property, max());
				element.RegisterCallback<FocusOutEvent>(e => Clamp(property, max()));
			}
			else if (property.propertyType == SerializedPropertyType.Float)
			{
				var max = GetMax(property, maximumAttribute, maximumAttribute.Maximum);

				Clamp(property, max());
				element.RegisterCallback<FocusOutEvent>(e => Clamp(property, max()));
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
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

		private Func<FieldType> GetMax<FieldType>(SerializedProperty property, MaximumAttribute maximumAttribute, FieldType defaultValue)
		{
			if (!string.IsNullOrEmpty(maximumAttribute.Compare))
			{
				var method = fieldInfo.DeclaringType.GetMethod(maximumAttribute.Compare, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				var field = fieldInfo.DeclaringType.GetField(maximumAttribute.Compare, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				var prop = fieldInfo.DeclaringType.GetProperty(maximumAttribute.Compare, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

				if (method != null)
				{
					if (method.ReturnType != typeof(FieldType))
						Debug.LogWarningFormat(_invalidMethodReturnWarning, property.propertyPath, maximumAttribute.Compare, property.type);
					else if (!method.HasSignature(null))
						Debug.LogWarningFormat(_invalidMethodParametersWarning, property.propertyPath, maximumAttribute.Compare);
					else
						return () => (FieldType)method.Invoke(method.IsStatic ? null : property.GetOwner<object>(), null);
				}
				else if (field != null)
				{
					if (field.FieldType != typeof(FieldType))
						Debug.LogWarningFormat(_invalidFieldReturnWarning, property.propertyPath, maximumAttribute.Compare, property.type);
					else
						return () => (FieldType)field.GetValue(field.IsStatic ? null : property.GetOwner<object>());
				}
				else if (prop != null)
				{
					if (prop.PropertyType != typeof(FieldType) || !prop.CanRead)
						Debug.LogWarningFormat(_invalidPropertyReturnWarning, property.propertyPath, maximumAttribute.Compare, property.type);
					else
						return () => (FieldType)prop.GetValue(prop.GetGetMethod(true).IsStatic ? null : property.GetOwner<object>());
				}
				else
				{
					Debug.LogWarningFormat(_missingCompareWarning, property.propertyPath, maximumAttribute.Compare, fieldInfo.DeclaringType.Name);
				}
			}

			return () => defaultValue;
		}
	}
}