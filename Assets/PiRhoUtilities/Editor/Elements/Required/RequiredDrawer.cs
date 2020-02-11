using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(RequiredAttribute))]
	class RequiredDrawer : PropertyDrawer
	{
		public const string Stylesheet = "RequiredDrawer.uss";
		public const string UssClassName = "pirho-required";
		public const string MessageBoxUssClassName = UssClassName + "__message-box";

		private const string _invalidTypeWarning = "(PURDIT) invalid type for RequiredAttribute on field '{0}': Required can only be applied to string or Object fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var element = this.CreateNextElement(property);

			if (property.propertyType == SerializedPropertyType.String || property.propertyType == SerializedPropertyType.ObjectReference || property.propertyType == SerializedPropertyType.ManagedReference)
			{
				var requiredAttribute = attribute as RequiredAttribute;
				var required = new VisualElement();
				required.AddStyleSheet(Stylesheet);
				required.AddToClassList(UssClassName);

				var message = new MessageBox((MessageBoxType)(int)requiredAttribute.Type, requiredAttribute.Message);
				message.AddToClassList(MessageBoxUssClassName);

				if (property.propertyType == SerializedPropertyType.String)
					CreateControl(property, element, message, UpdateString, property.stringValue);
				else if (property.propertyType == SerializedPropertyType.ObjectReference)
					CreateControl(property, element, message, UpdateObject, property.objectReferenceValue);
				else if (property.propertyType == SerializedPropertyType.ManagedReference)
					CreateControl(property, element, message, UpdateReference, property.GetManagedReferenceValue());

				required.Add(element);
				required.Add(message);

				return required;
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
				return element;
			}
		}

		private void CreateControl<T>(SerializedProperty property, VisualElement container, MessageBox message, Action<MessageBox, T> updateAction, T defaultValue)
		{
			var change = new ChangeTrigger<T>(property, (_, previous, current) => updateAction(message, current));
			updateAction(message, defaultValue);
			container.Add(change);
		}

		private void UpdateString(MessageBox message, string value)
		{
			message.SetDisplayed(string.IsNullOrEmpty(value));
		}

		private void UpdateObject(MessageBox message, Object value)
		{
			message.SetDisplayed(!value);
		}

		private void UpdateReference(MessageBox message, object value)
		{
			message.SetDisplayed(value == null);
		}
	}
}