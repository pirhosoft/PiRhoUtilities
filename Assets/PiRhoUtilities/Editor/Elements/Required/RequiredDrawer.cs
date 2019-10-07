using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(RequiredAttribute))]
	class RequiredDrawer : PropertyDrawer
	{
		public const string Stylesheet = "Required/RequiredDrawer.uss";
		public const string UssClassName = "pirho-required";
		public const string MessageBoxUssClassName = UssClassName + "__message-box";

		private const string _invalidTypeWarning = "(PURDIT) invalid type for RequiredAttribute on field '{0}': Required can only be applied to string or Object fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var element = this.CreateNextElement(property);

			if (property.propertyType == SerializedPropertyType.String || property.propertyType == SerializedPropertyType.ObjectReference)
			{
				var required = new VisualElement();
				required.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
				required.AddToClassList(UssClassName);

				var requiredAttribute = attribute as RequiredAttribute;
				var message = new MessageBox((MessageBoxType)(int)requiredAttribute.Type, requiredAttribute.Message);
				message.AddToClassList(MessageBoxUssClassName);

				required.Add(element);
				required.Add(message);

				if (property.propertyType == SerializedPropertyType.String)
				{
					var change = new ChangeTriggerControl<string>(property, (previous, current) => UpdateString(message, current));
					UpdateString(message, property.stringValue);
					element.Add(change);
				}
				else if (property.propertyType == SerializedPropertyType.ObjectReference)
				{
					var change = new ChangeTriggerControl<Object>(property, (previous, current) => UpdateObject(message, current));
					UpdateObject(message, property.objectReferenceValue);
					element.Add(change);
				}

				return required;
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
				return element;
			}
		}

		private void UpdateString(MessageBox message, string value)
		{
			message.SetDisplayed(string.IsNullOrEmpty(value));
		}

		private void UpdateObject(MessageBox message, Object value)
		{
			message.SetDisplayed(!value);
		}
	}
}