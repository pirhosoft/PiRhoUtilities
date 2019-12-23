using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class MinMaxSliderField : SliderField<Vector2>
	{
		#region Defaults

		// These match unity's internal defaults
		public const float DefaultMinimumValue = 0;
		public const float DefaultMaximumValue = 10;
		public const float DefaultMinimum = float.MinValue;
		public const float DefaultMaximum = float.MaxValue;

		#endregion

		public float MinimumLimit
		{
			get => Minimum.x;
			set => Minimum = new Vector2(value, rawValue.x);
		}

		public float MaximumLimit
		{
			get => Maximum.y;
			set => Maximum = new Vector2(rawValue.y, value);
		}

		public MinMaxSliderField() : this(null)
		{
		}

		public MinMaxSliderField(string label) : base(label, new MinMaxSliderControl())
		{
		}

		public MinMaxSliderField(string label, float min, float max) : this(label)
		{
			MinimumLimit = min;
			MaximumLimit = max;
		}

		public MinMaxSliderField(float min, float max) : this(null, min, max)
		{
		}

		private class MinMaxSliderControl : SliderControl
		{
			private readonly MinMaxSlider _slider;
			private readonly FloatField _minText;
			private readonly FloatField _maxText;

			public override Vector2 Minimum { get => new Vector2(_slider.lowLimit, _slider.minValue); set => _slider.lowLimit = value.x; }
			public override Vector2 Maximum { get => new Vector2(_slider.maxValue, _slider.highLimit); set => _slider.highLimit = value.y; }

			public MinMaxSliderControl()
			{
				_slider = new MinMaxSlider(DefaultMinimumValue, DefaultMaximumValue, DefaultMinimum, DefaultMaximum);
				_slider.AddToClassList(SliderUssClassName);

				_minText = new FloatField();
				_minText.RegisterValueChangedCallback(evt => this.SendChangeEvent(evt.previousValue, evt.newValue));
				_minText.AddToClassList(TextUssClassName);

				_maxText = new FloatField();
				_maxText.RegisterValueChangedCallback(evt => this.SendChangeEvent(evt.previousValue, evt.newValue));
				_maxText.AddToClassList(TextUssClassName);

				Add(_minText);
				Add(_slider);
				Add(_maxText);
			}

			public override void SetValueWithoutNotify(Vector2 value)
			{
				_slider.SetValueWithoutNotify(value);
				_minText.SetValueWithoutNotify(value.x);
				_maxText.SetValueWithoutNotify(value.y);
			}
		}

		#region UXML Support

		public new class UxmlFactory : UxmlFactory<MinMaxSliderField, UxmlTraits> { }

		public new class UxmlTraits : BaseField<Vector2>.UxmlTraits
		{
			private readonly UxmlFloatAttributeDescription _maxValue = new UxmlFloatAttributeDescription { name = "minimum-value", defaultValue = DefaultMinimumValue };
			private readonly UxmlFloatAttributeDescription _minValue = new UxmlFloatAttributeDescription { name = "maximum-value", defaultValue = DefaultMaximumValue };
			private readonly UxmlFloatAttributeDescription _minimum = new UxmlFloatAttributeDescription { name = "minimum", defaultValue = DefaultMinimum };
			private readonly UxmlFloatAttributeDescription _maximum = new UxmlFloatAttributeDescription { name = "maximum", defaultValue = DefaultMaximum };

			public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(element, bag, cc);

				var field = element as MinMaxSliderField;
				var maxValue = _minValue.GetValueFromBag(bag, cc);
				var minValue = _maxValue.GetValueFromBag(bag, cc);
				field.MinimumLimit = _minimum.GetValueFromBag(bag, cc);
				field.MaximumLimit = _maximum.GetValueFromBag(bag, cc);

				field.SetValueWithoutNotify(new Vector2(minValue, maxValue));
			}
		}

		#endregion
	}
}
