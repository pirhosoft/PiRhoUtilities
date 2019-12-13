using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public enum MessageBoxType
	{
		Info,
		Warning,
		Error
	}

	public class MessageBox : VisualElement
	{
		#region Class Names

		public const string Stylesheet = "MessageBox/MessageBox.uss";
		public const string UssClassName = "pirho-message-box";
		public const string ImageUssClassName = UssClassName + "__image";
		public const string LabelUssClassName = UssClassName + "__label";

		#endregion

		#region Defaults

		public const MessageBoxType DefaultType = MessageBoxType.Info;

		#endregion

		#region Members

		private readonly Image _image;
		private readonly TextElement _label;

		private MessageBoxType _type = DefaultType;

		#endregion

		#region Public Interface

		public MessageBoxType Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
				_image.image = GetIcon(_type);
			}
		}

		public string Message
		{
			get	{ return _label.text; }
			set	{ ((INotifyValueChanged<string>)_label).SetValueWithoutNotify(value); }
		}

		public MessageBox(MessageBoxType type, string message)
		{
			_image = new Image();
			_image.AddToClassList(ImageUssClassName);

			_label = new TextElement();
			_label.AddToClassList(LabelUssClassName);

			Type = type;
			Message = message;
			
			Add(_image);
			Add(_label);

			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
			AddToClassList(UssClassName);
		}

		#endregion

		#region Icon Management

		private Texture GetIcon(MessageBoxType type)
		{
			switch (type)
			{
				case MessageBoxType.Info: return Icon.Info.Texture;
				case MessageBoxType.Warning: return Icon.Warning.Texture;
				case MessageBoxType.Error: return Icon.Error.Texture;
				default: return null;
			}
		}

		#endregion

		#region UXML Support

		public MessageBox() : this(DefaultType, string.Empty) { }

		public new class UxmlFactory : UxmlFactory<MessageBox, UxmlTraits> { }
		public new class UxmlTraits : VisualElement.UxmlTraits
		{
			private readonly UxmlStringAttributeDescription _messageType = new UxmlStringAttributeDescription { name = "message-type" };
			private readonly UxmlStringAttributeDescription _message = new UxmlStringAttributeDescription { name = "message" };

			public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(element, bag, cc);

				var field = element as MessageBox;

				field.Message = _message.GetValueFromBag(bag, cc);

				var messageType = _messageType.GetValueFromBag(bag, cc);

				if (!string.IsNullOrEmpty(messageType))
					field.Type = ParseValue(messageType);
			}

			private MessageBoxType ParseValue(string valueName)
			{
				try
				{
					return (MessageBoxType)Enum.Parse(typeof(MessageBoxType), valueName);
				}
				catch (Exception exception) when (exception is ArgumentException || exception is OverflowException)
				{
					return DefaultType;
				}
			}
		}

		#endregion
	}
}