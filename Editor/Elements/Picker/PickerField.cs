using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public abstract class PickerField<ValueType> : BaseField<ValueType> where ValueType : class
	{
		public const string Stylesheet = "Picker/PickerStyle.uss";
		public const string UssClassName = "pirho-picker-field";
		public const string InputUssClassName = UssClassName + "__input";
		public const string LabelUssClassName = UssClassName + "__label";
		public const string ButtonUssClassName = InputUssClassName + "__button";
		public const string IconUssClassName = ButtonUssClassName + "__icon";
		public const string InputLabelUssClassName = ButtonUssClassName + "__label";

		protected PickerControl _control;

		protected PickerField(string label, PickerControl control) : base(label, control)
		{
			_control = control;
			_control.AddToClassList(InputUssClassName);
			_control.RegisterCallback<ChangeEvent<ValueType>>(evt =>
			{
				if (evt.currentTarget == _control)
				{
					base.value = evt.newValue;
					evt.StopImmediatePropagation();
				}
			});

			labelElement.AddToClassList(LabelUssClassName);

			AddToClassList(UssClassName);
			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
		}

		public override void SetValueWithoutNotify(ValueType newValue)
		{
			base.SetValueWithoutNotify(newValue);
			_control.SetValueWithoutNotify(newValue);
		}

		protected abstract class PickerControl : VisualElement
		{
			private readonly Button _button;
			private readonly Image _icon;
			private readonly TextElement _label;
			private readonly VisualElement _arrow;
			
			protected PickerProvider<ValueType> _provider;

			public abstract void SetValueWithoutNotify(ValueType newValue);

			public PickerControl()
			{
				_button = new Button();
				_button.AddToClassList(ButtonUssClassName);
				_button.AddToClassList(BasePopupField<ValueType, ValueType>.inputUssClassName);
				_button.clicked += () =>
				{
					if (_provider)
						SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(new Vector2(worldBound.center.x, worldBound.yMax + worldBound.height - 4)), worldBound.width), _provider);
				};

				_icon = new Image();
				_icon.AddToClassList(IconUssClassName);

				_label = new TextElement();
				_label.AddToClassList(InputLabelUssClassName);

				_arrow = new VisualElement();
				_arrow.AddToClassList(BasePopupField<ValueType, ValueType>.arrowUssClassName);

				_button.Add(_icon);
				_button.Add(_label);
				_button.Add(_arrow);

				Add(_button);
			}

			protected void SetLabel(Texture icon, string text)
			{
				((INotifyValueChanged<string>)_label).SetValueWithoutNotify(text);
				_icon.image = icon;
				_icon.SetDisplayed(icon);
			}
		}
	}
}
