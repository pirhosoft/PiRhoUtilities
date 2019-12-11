using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public abstract class BasePickerControl<ValueType> : VisualElement
	{
		public const string Stylesheet = "Picker/BasePicker/BasePickerStyle.uss";
		public const string UssClassName = "pirho-base-picker";
		public const string ButtonUssClassName = UssClassName + "__button";
		public const string IconUssClassName = ButtonUssClassName + "__icon";
		public const string LabelUssClassName = ButtonUssClassName + "__label";

		public ValueType Value { get; private set; }

		private Image _icon;
		private TextElement _label;
		private VisualElement _arrow;

		protected void Setup<PickerType>(PickerProvider<PickerType> provider, ValueType value) where PickerType : class
		{
			var button = new Button();
			button.AddToClassList(ButtonUssClassName);
			button.AddToClassList(BasePopupField<ValueType, ValueType>.inputUssClassName);
			button.clickable.clicked += () => SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(new Vector2(button.worldBound.center.x, button.worldBound.yMax + button.worldBound.height - 4)), button.worldBound.width), provider);
			// this position to open isn't perfect because they positioned the search window origin from the middle of it's search box instead of the top like a sane person.

			_icon = new Image();
			_icon.AddToClassList(IconUssClassName);

			_label = new TextElement();
			_label.AddToClassList(LabelUssClassName);

			_arrow = new VisualElement();
			_arrow.AddToClassList(BasePopupField<ValueType, ValueType>.arrowUssClassName);

			button.Add(_icon);
			button.Add(_label);
			button.Add(_arrow);

			Add(button);
			SetValueWithoutNotify(value);

			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
			AddToClassList(UssClassName);
		}

		public void SetValueWithoutNotify(ValueType value)
		{
			Value = value;
			Refresh();
		}

		protected abstract void Refresh();

		protected void SetLabel(Texture icon, string text)
		{
			_icon.image = icon;
			_label.text = text;
			_icon.SetDisplayed(icon);
		}
	}
}
