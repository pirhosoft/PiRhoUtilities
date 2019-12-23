using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(MessageBoxAttribute))]
	class MessageBoxDrawer : PropertyDrawer
	{
		public const string Stylesheet = "MessageBox/MessageBoxStyle.uss";
		public const string UssClassName = "pirho-trait-message";
		public const string SideUssClassName = UssClassName + "--side";
		public const string MessageUssClassName = UssClassName + "__message-box";
		public const string AboveUssClassName = MessageUssClassName + "--above";
		public const string BelowUssClassName = MessageUssClassName + "--below";
		public const string LeftUssClassName = MessageUssClassName + "--left";
		public const string RightUssClassName = MessageUssClassName + "--right";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var messageAttribute = attribute as MessageBoxAttribute;
			var element = this.CreateNextElement(property);
			var container = new VisualElement();

			container.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
			container.AddToClassList(UssClassName);
			container.Add(element);

			var message = new MessageBox(messageAttribute.Type, messageAttribute.Message);
			message.AddToClassList(MessageUssClassName);

			if (messageAttribute.Location == TraitLocation.Above)
			{
				message.AddToClassList(BelowUssClassName);
				container.Insert(0, message);
			}
			if (messageAttribute.Location == TraitLocation.Below)
			{
				message.AddToClassList(BelowUssClassName);
				container.Add(message);
			}
			else if (messageAttribute.Location == TraitLocation.Left)
			{
				message.AddToClassList(LeftUssClassName);
				container.Insert(0, message);
				container.AddToClassList(SideUssClassName);
			}
			else if (messageAttribute.Location == TraitLocation.Right)
			{
				message.AddToClassList(RightUssClassName);
				container.Add(message);
				container.AddToClassList(SideUssClassName);
			}

			return container;
		}
	}
}