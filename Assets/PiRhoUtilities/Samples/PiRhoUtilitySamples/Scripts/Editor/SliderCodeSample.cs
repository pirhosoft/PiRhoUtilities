using PiRhoSoft.Utilities.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Samples
{
	public class SliderCodeSample : CodeSample
	{
		public override void Create(VisualElement root)
		{
			var ratingSlider = new SliderFloatField("Rating", -10, 10);
			ratingSlider.value = 0;
			ratingSlider.RegisterValueChangedCallback(evt => Debug.Log($"Rating changed to {evt.newValue}"));
			root.Add(ratingSlider);

			var rangeSlider = new MinMaxSliderField("Range", -100, 100);
			rangeSlider.value = new Vector2(ratingSlider.Minimum, ratingSlider.Maximum);
			rangeSlider.RegisterValueChangedCallback(evt => { ratingSlider.Minimum = evt.newValue.x; ratingSlider.Maximum = evt.newValue.y; });
			root.Add(rangeSlider);
		}
	}
}
