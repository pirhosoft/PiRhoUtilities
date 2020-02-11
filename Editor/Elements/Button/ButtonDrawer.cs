using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(ButtonAttribute))]
	class ButtonDrawer : PropertyDrawer
	{
		public const string Stylesheet = "ButtonStyle.uss";
		public const string UssClassName = "pirho-trait-button";
		public const string SideUssClassName = UssClassName + "--side";
		public const string ButtonUssClassName = UssClassName + "__button";
		public const string AboveUssClassName = ButtonUssClassName + "--above";
		public const string BelowUssClassName = ButtonUssClassName + "--below";
		public const string LeftUssClassName = ButtonUssClassName + "--left";
		public const string RightUssClassName = ButtonUssClassName + "--right";

		private const string _invalidMethodWarning = "(PUBDIM) invalid method for ButtonAttribute on field '{0}': a parameterless method named '{1}' could not be found on type '{2}'";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var buttonAttribute = attribute as ButtonAttribute;
			var element = this.CreateNextElement(property);

			var method = ReflectionHelper.CreateActionCallback(buttonAttribute.Method, fieldInfo.DeclaringType, property);
			if (method != null)
			{
				var container = new VisualElement();
				container.AddStyleSheet(Stylesheet);
				container.AddToClassList(UssClassName);
				container.Add(element);

				var button = CreateButton(buttonAttribute.Label, buttonAttribute.Icon, method, buttonAttribute.Tooltip);

				button.AddToClassList(ButtonUssClassName);

				if (buttonAttribute.Location == TraitLocation.Above)
				{
					button.AddToClassList(BelowUssClassName);
					container.Insert(0, button);
				}
				if (buttonAttribute.Location == TraitLocation.Below)
				{
					button.AddToClassList(BelowUssClassName);
					container.Add(button);
				}
				else if (buttonAttribute.Location == TraitLocation.Left)
				{
					button.AddToClassList(LeftUssClassName);
					container.Insert(0, button);
					container.AddToClassList(SideUssClassName);
				}
				else if (buttonAttribute.Location == TraitLocation.Right)
				{
					button.AddToClassList(RightUssClassName);
					container.Add(button);
					container.AddToClassList(SideUssClassName);
				}

				return container;
			}
			else
			{
				Debug.LogWarningFormat(_invalidMethodWarning, property.propertyPath, buttonAttribute.Method, fieldInfo.DeclaringType.Name);
			}

			return element;
		}

		private VisualElement CreateButton(string label, ButtonIcon icon, Action method, string tooltip)
		{
			if (string.IsNullOrEmpty(label))
			{
				var button = new IconButton(method)
				{
					image = null,
					tooltip = tooltip
				};

				button.SetIcon(icon.ToString());
				button.style.width = button.image.width;
				button.style.height = button.image.height;
				return button;
			}
			else
			{
				return new Button(method)
				{
					text = label,
					tooltip = tooltip
				};
			}
		}
	}
}