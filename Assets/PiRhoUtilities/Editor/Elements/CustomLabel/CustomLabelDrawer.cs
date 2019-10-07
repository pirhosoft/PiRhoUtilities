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
		private const string _missingMethodWarning = "(PUCLDMM) invalid method for CustomLabelAttribute on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _invalidMethodReturnWarning = "(PUCLDIMR) invalid method for CustomLabelAttribute on field '{0}': the method '{1}' should return a string";
		private const string _invalidMethodParametersWarning = "(PUCLDIMP) invalid method for CustomLabelAttribute on field '{0}': the method '{1}' should take no parameters";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var element = this.CreateNextElement(property);
			var label = element.Query<Label>(className: PropertyField.labelUssClassName).First();

			if (label != null)
				label.text = GetText(property, attribute as CustomLabelAttribute);

			return element;
		}

		private string GetText(SerializedProperty property, CustomLabelAttribute labelAttribute)
		{
			if (!string.IsNullOrEmpty(labelAttribute.Method))
			{
				var method = fieldInfo.DeclaringType.GetMethod(labelAttribute.Method, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

				if (method != null)
				{
					var owner = method.IsStatic
						? null
						: property.GetOwner<object>();

					if (method.ReturnType != typeof(string))
						Debug.LogWarningFormat(_invalidMethodReturnWarning, property.propertyPath, labelAttribute.Method);
					else if (method.HasSignature(null))
						Debug.LogWarningFormat(_invalidMethodParametersWarning, property.propertyPath, labelAttribute.Method);
					else
						return (string)method.Invoke(owner, null);
				}
				else
				{
					Debug.LogWarningFormat(_missingMethodWarning, property.propertyPath, labelAttribute.Method, fieldInfo.DeclaringType.Name);
				}
			}

			return labelAttribute.Label;
		}
	}
}