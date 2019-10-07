using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(ButtonAttribute))]
	class ButtonDrawer : PropertyDrawer
	{
		public const string Stylesheet = "Button/ButtonDrawer.uss";
		public const string UssClassName = "pirho-trait-button";
		public const string RightUssClassName = UssClassName + "--right";

		private const string _missingMethodWarning = "(PUBDMM) invalid method for ButtonAttribute on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _invalidMethodWarning = "(PUBDIM) invalid method for ButtonAttribute on field '{0}': the method '{1}' on type '{2}' should not take parameters";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var buttonAttribute = attribute as ButtonAttribute;
			var method = fieldInfo.DeclaringType.GetMethod(buttonAttribute.Method, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			var element = this.CreateNextElement(property);
			var container = new VisualElement();

			container.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
			container.AddToClassList(UssClassName);
			container.Add(element);

			if (method == null)
			{
				Debug.LogWarningFormat(_missingMethodWarning, property.propertyPath, buttonAttribute.Method, fieldInfo.DeclaringType.Name);
			}
			else if (!method.HasSignature(null))
			{
				Debug.LogWarningFormat(_invalidMethodWarning, property.propertyPath, buttonAttribute.Method, fieldInfo.DeclaringType.Name);
			}
			else
			{
				var owner = method.IsStatic ? null : property.GetOwner<object>();
				var text = string.IsNullOrEmpty(buttonAttribute.Label) ? buttonAttribute.Method : buttonAttribute.Label;
				var button = new Button(() => method.Invoke(owner, null))
				{
					text = text,
					tooltip = buttonAttribute.Tooltip
				};

				container.Add(button);

				if (buttonAttribute.Location == TraitLocation.Before)
					button.SendToBack();
				else if (buttonAttribute.Location == TraitLocation.Right)
					container.AddToClassList(RightUssClassName);
			}

			return container;
		}
	}
}