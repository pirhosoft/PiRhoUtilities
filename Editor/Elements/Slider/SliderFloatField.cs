using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class SliderFloatField : SliderField<float>
	{
		#region Defaults

		// These match unity's internal defaults
		public const float DefaultMinimum = 0;
		public const float DefaultMaximum = 10;

		#endregion

		#region Public Interface

		public SliderFloatField(string label) : base(label, new SliderFloatControl())
		{
		}

		public SliderFloatField(string label, float min, float max) : this(label)
		{
			Minimum = min;
			Maximum = max;
		}

		public SliderFloatField(float min, float max) : this(null, min, max)
		{
		}

		#endregion

		#region Visual Input

		private class SliderFloatControl : SliderControl
		{
			public override float Minimum { get => _slider.lowValue; set => _slider.lowValue = value; }
			public override float Maximum { get => _slider.highValue; set => _slider.highValue = value; }

			private readonly Slider _slider;
			private readonly FloatField _text;

			public SliderFloatControl()
			{
				_slider = new Slider(DefaultMinimum, DefaultMaximum);
				_slider.AddToClassList(SliderUssClassName);
				_text = new FloatField();
				_text.AddToClassList(TextUssClassName);

				Add(_slider);
				Add(_text);
			}

			public override void SetValueWithoutNotify(float value)
			{
				_slider.SetValueWithoutNotify(value);
				_text.SetValueWithoutNotify(value);
			}
		}

		#endregion

		#region UXML Support

		public SliderFloatField() : this(null) { }

		public new class UxmlFactory : UxmlFactory<SliderFloatField, UxmlTraits> { }
		public new class UxmlTraits : UxmlTraits<UxmlFloatAttributeDescription>
		{
			public UxmlTraits()
			{
				_minimum.defaultValue = DefaultMinimum;
				_maximum.defaultValue = DefaultMaximum;
			}
		}

		#endregion
	}
}
