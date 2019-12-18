using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(ValidateAttribute))]
	class ValidateDrawer : PropertyDrawer
	{
		public const string Stylesheet = "Validate/ValidateDrawer.uss";
		public const string UssClassName = "pirho-validate";
		public const string MessageBoxUssClassName = UssClassName + "__message-box";

		private const string _invalidMethodWarning = "(PUVDMM) invalid method for ValidateAttribute on field '{0}': a parameterless bool returing method named '{1}' could not be found on type '{2}'";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var element = this.CreateNextElement(property);

			var validate = new VisualElement();
			validate.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
			validate.AddToClassList(UssClassName);

			var validateAttribute = attribute as ValidateAttribute;
			var message = new MessageBox((MessageBoxType)(int)validateAttribute.Type, validateAttribute.Message);
			message.AddToClassList(MessageBoxUssClassName);

			validate.Add(element);
			validate.Add(message);
			
			var method = ReflectionHelper.CreateFunctionCallback<bool>(property, fieldInfo.DeclaringType, validateAttribute.Method, nameof(ValidateAttribute), nameof(ValidateAttribute.Method));

			if (method != null)
			{
				element.Add(ChangeTrigger.Create(property, (_) => Validate(message, method)));
				Validate(message, method);
			}
			else
			{
				Debug.LogWarningFormat(_invalidMethodWarning, property.propertyPath, validateAttribute.Method, fieldInfo.DeclaringType.Name);
			}

			return validate;
		}

		private void Validate(VisualElement message, Func<bool> validate)
		{
			var valid = validate();
			message.SetDisplayed(!valid);
		}
	}
}