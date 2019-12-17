namespace PiRhoSoft.Utilities
{
	public class SliderAttribute : PropertyTraitAttribute
	{
		public float Minimum { get; private set; }
		public float Maximum { get; private set; }

		public string MinimumSource { get; private set; }
		public string MaximumSource { get; private set; }
		public bool AutoUpdate { get; private set; }

		public SliderAttribute(float minimum, float maximum) : base(ControlPhase, 0)
		{
			Minimum = minimum;
			Maximum = maximum;
		}

		public SliderAttribute(string minimumSource, string maximumSource, bool autoUpdate = true) : base(ControlPhase, 0)
		{
			MinimumSource = minimumSource;
			MaximumSource = maximumSource;
			AutoUpdate = autoUpdate;
		}
	}
}