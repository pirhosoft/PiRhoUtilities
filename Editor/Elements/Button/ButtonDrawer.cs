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
		public const string SideUssClassName = UssClassName + "--side";

		private const string _invalidMethodWarning = "(PUBDIM) invalid method for ButtonAttribute on field '{0}': a parameterless method named '{1}' colud not be found on type '{2}'";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var buttonAttribute = attribute as ButtonAttribute;
			var element = this.CreateNextElement(property);
			var container = new VisualElement();

			container.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
			container.AddToClassList(UssClassName);
			container.Add(element);

			var method = ReflectionHelper.CreateActionCallback(property, fieldInfo.DeclaringType, buttonAttribute.Method, nameof(ButtonAttribute), nameof(ButtonAttribute.Method));

			if (method == null)
			{
				Debug.LogWarningFormat(_invalidMethodWarning, property.propertyPath, buttonAttribute.Method, fieldInfo.DeclaringType.Name);
			}
			else
			{
				var text = string.IsNullOrEmpty(buttonAttribute.Label) ? buttonAttribute.Method : buttonAttribute.Label;
				var button = new Button(method)
				{
					text = text,
					tooltip = buttonAttribute.Tooltip
				};

				if (buttonAttribute.Location == TraitLocation.Before)
				{
					container.Insert(0, button);
				}
				if (buttonAttribute.Location == TraitLocation.After)
				{
					container.Add(button);
				}
				else if (buttonAttribute.Location == TraitLocation.Left)
				{
					container.Insert(0, button);
					container.AddToClassList(SideUssClassName);
				}
				else if (buttonAttribute.Location == TraitLocation.Right)
				{
					container.Add(button);
					container.AddToClassList(SideUssClassName);
				}
			}

			return container;
		}
	}
}