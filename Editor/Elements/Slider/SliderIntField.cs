using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class SliderIntField : SliderField<int>
	{
		#region Defaults

		// These match unity's internal defaults
		public const int DefaultMinimum = 0;
		public const int DefaultMaximum = 10;

		#endregion

		#region Public Interface

		public SliderIntField(string label) : base(label, new SliderIntControl())
		{
		}

		public SliderIntField(string label, int min, int max) : this(label)
		{
			Minimum = min;
			Maximum = max;
		}

		public SliderIntField(int min, int max) : this(null, min, max)
		{
		}

		#endregion

		#region Visual Input

		private class SliderIntControl : SliderControl
		{
			public override int Minimum { get => _slider.lowValue; set => _slider.lowValue = value; }
			public override int Maximum { get => _slider.highValue; set => _slider.highValue = value; }

			private readonly SliderInt _slider;
			private readonly IntegerField _text;

			public SliderIntControl()
			{
				_slider = new SliderInt(DefaultMinimum, DefaultMaximum);
				_slider.AddToClassList(SliderUssClassName);
				_text = new IntegerField();
				_text.AddToClassList(TextUssClassName);

				Add(_slider);
				Add(_text);
			}

			public override void SetValueWithoutNotify(int value)
			{
				_slider.SetValueWithoutNotify(value);
				_text.SetValueWithoutNotify(value);
			}
		}

		#endregion

		#region UXML Support

		public SliderIntField() : this(null) { }

		public new class UxmlFactory : UxmlFactory<SliderIntField, UxmlTraits> { }
		public new class UxmlTraits : UxmlTraits<UxmlIntAttributeDescription>
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
