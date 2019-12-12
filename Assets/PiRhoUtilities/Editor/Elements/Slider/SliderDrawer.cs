using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(SliderAttribute))]
	class SliderDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUSLDIT) invalid type for SliderAttribute on field {0}: Slider can only be applied to int, float, or Vector2 fields";
		private const string _missingWarning = "(PUSDDMC) invalid {0} method, field, or property for SliderAttribute on field '{1}': '{2}' could not be found on type '{3}'";
		private const string _invalidReturnWarning = "(PUSDIMR) invalid {0} method, field, or property for SliderAttribute on field '{0}': '{1}' should return a '{2}'";
		private const string _invalidParametersWarning = "(PUSDIMP) invalid {0} method for SliderAttribute on field '{1}': '{2}' should take no parameters";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var sliderAttribute = attribute as SliderAttribute;

			if (property.propertyType == SerializedPropertyType.Integer)
			{
				var minimum = GetValue(property, sliderAttribute.MinimumMethod, Mathf.RoundToInt(sliderAttribute.Minimum), "minimum");
				var maximum = GetValue(property, sliderAttribute.MaximumMethod, Mathf.RoundToInt(sliderAttribute.Maximum), "minimum");

				return new SliderIntField(minimum, maximum).ConfigureProperty(property);
			}
			else if (property.propertyType == SerializedPropertyType.Float)
			{
				var minimum = GetValue(property, sliderAttribute.MinimumMethod, sliderAttribute.Minimum, "minimum");
				var maximum = GetValue(property, sliderAttribute.MaximumMethod, sliderAttribute.Maximum, "minimum");

				return new SliderFloatField(minimum, maximum).ConfigureProperty(property);
			}
			else if (property.propertyType == SerializedPropertyType.Vector2)
			{
				var minimum = GetValue(property, sliderAttribute.MinimumMethod, sliderAttribute.Minimum, "minimum");
				var maximum = GetValue(property, sliderAttribute.MaximumMethod, sliderAttribute.Maximum, "minimum");

				return new MinMaxSliderField(minimum, maximum).ConfigureProperty(property);
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
				return new FieldContainer(property.displayName);
			}
		}

		private FieldType GetValue<FieldType>(SerializedProperty property, string methodName, FieldType defaultValue, string warningType)
		{
			if (!string.IsNullOrEmpty(methodName))
			{
				var method = fieldInfo.DeclaringType.GetMethod(methodName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				var field = fieldInfo.DeclaringType.GetField(methodName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				var prop = fieldInfo.DeclaringType.GetProperty(methodName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

				if (method != null)
				{
					if (method.ReturnType != typeof(FieldType))
						Debug.LogWarningFormat(_invalidReturnWarning, warningType, property.propertyPath, methodName, nameof(FieldType));
					else if (!method.HasSignature(null))
						Debug.LogWarningFormat(_invalidParametersWarning, warningType, property.propertyPath, methodName);
					else
						return (FieldType)method.Invoke(method.IsStatic ? null : property.GetOwner<object>(), null);
				}
				else if (field != null)
				{
					if (field.FieldType != typeof(FieldType))
						Debug.LogWarningFormat(_invalidReturnWarning, warningType, property.propertyPath, methodName, nameof(FieldType));
					else
						return (FieldType)field.GetValue(field.IsStatic ? null : property.GetOwner<object>());
				}
				else if (prop != null)
				{
					if (prop.PropertyType != typeof(FieldType) || !prop.CanRead)
						Debug.LogWarningFormat(_invalidReturnWarning, warningType, property.propertyPath, methodName, nameof(FieldType));
					else
						return (FieldType)prop.GetValue(prop.GetGetMethod(true).IsStatic ? null : property.GetOwner<object>());
				}
				else
				{
					Debug.LogWarningFormat(_missingWarning, warningType, property.propertyPath, methodName, fieldInfo.DeclaringType.Name);
				}
			}

			return defaultValue;
		}
	}
}