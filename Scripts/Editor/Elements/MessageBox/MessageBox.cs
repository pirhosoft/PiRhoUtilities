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
		public const string Stylesheet = "MessageBox/MessageBox.uss";
		public const string UssClassName = "pirho-message-box";
		public const string ImageUssClassName = UssClassName + "__image";
		public const string LabelUssClassName = UssClassName + "__label";

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
			set	{ _label.text = value; }
		}

		private Image _image;
		private TextElement _label;
		private MessageBoxType _type;

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

		private Texture GetIcon(MessageBoxType type)
		{
			switch (type)
			{
				case MessageBoxType.Info: return Icon.Info.Texture;
				case MessageBoxType.Warning: return Icon.Warning.Texture;
				case MessageBoxType.Error: return Icon.Error.Texture;
			}

			return null;
		}
	}
}