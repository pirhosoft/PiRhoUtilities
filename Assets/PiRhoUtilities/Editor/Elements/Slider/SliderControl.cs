using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public abstract class BaseSliderControl<ValueType> : VisualElement
	{
		public const string Stylesheet = "Slider/SliderStyle.uss";
		public static readonly string UssClassName = "pirho-slider";
		public static readonly string SliderUssClassName = UssClassName + "__slider";
		public static readonly string TextUssClassName = UssClassName + "__text";

		public ValueType Value;

		protected BaseField<ValueType> _slider;
		protected BaseField<ValueType> _text;

		protected void Setup(ValueType value)
		{
			_slider?.RegisterValueChangedCallback(evt => this.SendChangeEvent(evt.previousValue, evt.newValue));
			_slider?.AddToClassList(SliderUssClassName);

			_text?.RegisterValueChangedCallback(evt => this.SendChangeEvent(evt.previousValue, evt.newValue));
			_text?.AddToClassList(TextUssClassName);

			if (_slider != null)
				Add(_slider);

			if (_text != null)
				Add(_text);

			SetValueWithoutNotify(value);
			AddToClassList(UssClassName);
			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
		}

		public void SetValueWithoutNotify(ValueType value)
		{
			Value = value;
			Refresh();
		}

		protected virtual void Refresh()
		{
			_slider?.SetValueWithoutNotify(Value);
			_text?.SetValueWithoutNotify(Value);
		}
	}

	public class SliderIntControl : BaseSliderControl<int>
	{
		public SliderIntControl(int value, int min, int max)
		{
			_slider = new SliderInt(min, max);
			_text = new IntegerField();

			Setup(value);
		}
	}

	public class SliderFloatControl : BaseSliderControl<float>
	{
		public SliderFloatControl(float value, float min, float max)
		{
			_slider = new Slider(min, max);
			_text = new FloatField();

			Setup(value);
		}
	}

	public class MinMaxSliderControl : BaseSliderControl<Vector2>
	{
		private FloatField _minText;
		private FloatField _maxText;

		public MinMaxSliderControl(Vector2 value, float min, float max)
		{
			_slider = new MinMaxSlider(value.x, value.y, min, max);

			_minText = new FloatField();
			_minText.RegisterValueChangedCallback(evt => this.SendChangeEvent(evt.previousValue, evt.newValue));
			_minText.AddToClassList(TextUssClassName);

			_maxText = new FloatField();
			_maxText.RegisterValueChangedCallback(evt => this.SendChangeEvent(evt.previousValue, evt.newValue));
			_maxText.AddToClassList(TextUssClassName);

			Add(_minText);
			Setup(value);
			Add(_maxText);
		}

		protected override void Refresh()
		{
			_slider.SetValueWithoutNotify(Value);
			_minText.SetValueWithoutNotify(Value.x);
			_maxText.SetValueWithoutNotify(Value.y);
		}
	}
}