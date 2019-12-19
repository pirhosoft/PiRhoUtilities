using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(ValidateAttribute))]
	class ValidateDrawer : PropertyDrawer
	{
		public const string Stylesheet = "Validate/ValidateStyle.uss";
		public const string UssClassName = "pirho-validate";
		public const string SideUssClassName = UssClassName + "--side";
		public const string MessageUssClassName = UssClassName + "__message";
		public const string AboveUssClassName = MessageUssClassName + "--above";
		public const string BelowUssClassName = MessageUssClassName + "--below";
		public const string LeftUssClassName = MessageUssClassName + "--left";
		public const string RightUssClassName = MessageUssClassName + "--right";

		private const string _invalidMethodWarning = "(PUVDMM) invalid method for ValidateAttribute on field '{0}': a parameterless, bool method named '{1}' could not be found on type '{2}'";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var validateAttribute = attribute as ValidateAttribute;
			var element = this.CreateNextElement(property);

			var method = ReflectionHelper.CreateFunctionCallback<bool>(validateAttribute.Method, fieldInfo.DeclaringType, property);
			if (method != null)
			{
				var container = new VisualElement();
				container.AddToClassList(UssClassName);
				container.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
				container.Add(element);

				var message = new MessageBox(validateAttribute.Type, validateAttribute.Message);
				message.AddToClassList(MessageUssClassName);

				Validate(message, method);
				element.Add(ChangeTrigger.Create(property, (_) => Validate(message, method)));

				message.AddToClassList(MessageUssClassName);

				if (validateAttribute.Location == TraitLocation.Above)
				{
					message.AddToClassList(BelowUssClassName);
					container.Insert(0, message);
				}
				if (validateAttribute.Location == TraitLocation.Below)
				{
					message.AddToClassList(BelowUssClassName);
					container.Add(message);
				}
				else if (validateAttribute.Location == TraitLocation.Left)
				{
					message.AddToClassList(LeftUssClassName);
					container.Insert(0, message);
					container.AddToClassList(SideUssClassName);
				}
				else if (validateAttribute.Location == TraitLocation.Right)
				{
					message.AddToClassList(RightUssClassName);
					container.Add(message);
					container.AddToClassList(SideUssClassName);
				}

				return container;
			}
			else
			{
				Debug.LogWarningFormat(_invalidMethodWarning, property.propertyPath, validateAttribute.Method, fieldInfo.DeclaringType.Name);
			}

			return element;
		}

		private void Validate(VisualElement message, Func<bool> validate)
		{
			var valid = validate();
			message.SetDisplayed(!valid);
		}
	}
}