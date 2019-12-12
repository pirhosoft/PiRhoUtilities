using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class SliderFloatField : SliderField<float>
	{
		public SliderFloatField(string label) : base(label, new SliderFloatControl())
		{
		}

		public SliderFloatField(string label, float min, float max) : this(label)
		{
			_control.Minimum = min;
			_control.Maximum = max;
		}

		public SliderFloatField(float min, float max) : this(null, min, max)
		{
		}

		private class SliderFloatControl : SliderControl
		{
			public override float Minimum { get => _slider.lowValue; set => _slider.lowValue = value; }
			public override float Maximum { get => _slider.highValue; set => _slider.highValue = value; }

			private readonly Slider _slider;
			private readonly FloatField _text;

			public SliderFloatControl()
			{
				_slider = new Slider();
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

		#region UXML Support

		public SliderFloatField() : this(null) { }

		public new class UxmlFactory : UxmlFactory<SliderFloatField, UxmlTraits> { }
		public new class UxmlTraits : UxmlTraits<UxmlFloatAttributeDescription> { }

		#endregion
	}
}
