using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(ComboBoxAttribute))]
	class ComboBoxDrawer : PropertyDrawer
	{
		private const string _invalidTypeError = "(PUCBDIT) invalid type for ComboBoxAttribute on field '{0}': ComboBox can only be applied to string fields";
		private const string _missingWarning = "(PUCBDMC) invalid options method, field, or property for ComboBoxAttribute on field '{0}': '{1}' could not be found on type '{2}'";
		private const string _invalidReturnWarning = "(PUCBDIMR) invalid options method, field, or property for ComboBoxAttribute on field '{0}': '{1}' should return a List<string>";
		private const string _invalidParametersWarning = "(PUCBDIMP) invalid options method for ComboBoxAttribute on field '{0}': '{1}' should take no parameters";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var comboBox = attribute as ComboBoxAttribute;

			if (property.propertyType == SerializedPropertyType.String)
			{
				var options = GetOptions(property, comboBox.OptionsMethod, comboBox.Options);
				return new ComboBoxField(options).ConfigureProperty(property);
			}
			else
			{
				Debug.LogErrorFormat(_invalidTypeError, property.propertyPath);
			}

			return new FieldContainer(property.displayName);
		}

		private List<string> GetOptions(SerializedProperty property, string methodName, List<string> defaultValues)
		{
			if (!string.IsNullOrEmpty(methodName))
			{
				var method = fieldInfo.DeclaringType.GetMethod(methodName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				var field = fieldInfo.DeclaringType.GetField(methodName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				var prop = fieldInfo.DeclaringType.GetProperty(methodName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

				if (method != null)
				{
					if (method.ReturnType != typeof(List<string>))
						Debug.LogWarningFormat(_invalidReturnWarning, property.propertyPath, methodName);
					else if (!method.HasSignature(null))
						Debug.LogWarningFormat(_invalidParametersWarning, property.propertyPath, methodName);
					else
						return (List<string>)method.Invoke(method.IsStatic ? null : property.GetOwner<object>(), null);
				}
				else if (field != null)
				{
					if (field.FieldType != typeof(List<string>))
						Debug.LogWarningFormat(_invalidReturnWarning, property.propertyPath, methodName);
					else
						return (List<string>)field.GetValue(field.IsStatic ? null : property.GetOwner<object>());
				}
				else if (prop != null)
				{
					if (prop.PropertyType != typeof(List<string>) || !prop.CanRead)
						Debug.LogWarningFormat(_invalidReturnWarning, property.propertyPath, methodName);
					else
						return (List<string>)prop.GetValue(prop.GetGetMethod(true).IsStatic ? null : property.GetOwner<object>());
				}
				else
				{
					Debug.LogWarningFormat(_missingWarning, property.propertyPath, methodName, fieldInfo.DeclaringType.Name);
				}
			}

			return defaultValues;
		}
	}
}