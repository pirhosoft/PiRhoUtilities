using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(CustomLabelAttribute))]
	class CustomLabelDrawer : PropertyDrawer
	{
		private const string _missingMethodWarning = "(PUCLDMM) invalid method, field, or property for CustomLabelAttribute on field '{0}': '{1}' could not be found on type '{2}'";
		private const string _invalidMethodReturnWarning = "(PUCLDIMR) invalid method for CustomLabelAttribute on field '{0}': the method '{1}' should return a string";
		private const string _invalidMethodParametersWarning = "(PUCLDIMP) invalid method for CustomLabelAttribute on field '{0}': the method '{1}' should take no parameters";
		private const string _invalidFieldReturnWarning = "(PUCLDIFR) invalid field for CustomLabelAttribute on field '{0}': the field '{1}' should return a string";
		private const string _invalidPropertyReturnWarning = "(PUCLDIPR) invalid property for CustomLabelAttribute on field '{0}': the field'{1}' should return a string";

		private static readonly string[] _labelClasses = new string[] { PropertyField.labelUssClassName, BaseFieldExtensions.LabelUssClassName, Frame.LabelUssClassName };

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var element = this.CreateNextElement(property);

			foreach (var className in _labelClasses)
			{
				var label = element.Query<Label>(className: className).First();

				if (label != null)
				{
					label.text = GetText(property, attribute as CustomLabelAttribute);
					break;
				}
			}

			return element;
		}

		private string GetText(SerializedProperty property, CustomLabelAttribute labelAttribute)
		{
			if (!string.IsNullOrEmpty(labelAttribute.Resolve))
			{
				var method = fieldInfo.DeclaringType.GetMethod(labelAttribute.Resolve, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				var field = fieldInfo.DeclaringType.GetField(labelAttribute.Resolve, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				var prop = fieldInfo.DeclaringType.GetProperty(labelAttribute.Resolve, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

				if (method != null)
				{
					var owner = method.IsStatic ? null : property.GetOwner<object>();

					if (method.ReturnType != typeof(string))
						Debug.LogWarningFormat(_invalidMethodReturnWarning, property.propertyPath, labelAttribute.Resolve);
					else if (method.HasSignature(null))
						Debug.LogWarningFormat(_invalidMethodParametersWarning, property.propertyPath, labelAttribute.Resolve);
					else
						return (string)method.Invoke(owner, null);
				}
				else if (field != null)
				{
					if (field.FieldType != typeof(string))
						Debug.LogWarningFormat(_invalidFieldReturnWarning, property.propertyPath, labelAttribute.Resolve, property.type);
					else
						return (string)field.GetValue(field.IsStatic ? null : property.GetOwner<object>());
				}
				else if (prop != null)
				{
					if (prop.PropertyType != typeof(string) || !prop.CanRead)
						Debug.LogWarningFormat(_invalidPropertyReturnWarning, property.propertyPath, labelAttribute.Resolve, property.type);
					else
						return (string)prop.GetValue(prop.GetGetMethod(true).IsStatic ? null : property.GetOwner<object>());
				}
				else
				{
					Debug.LogWarningFormat(_missingMethodWarning, property.propertyPath, labelAttribute.Resolve, fieldInfo.DeclaringType.Name);
				}
			}

			return labelAttribute.Label;
		}
	}
}