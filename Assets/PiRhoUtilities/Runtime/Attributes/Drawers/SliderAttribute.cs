namespace PiRhoSoft.Utilities
{
	public class SliderAttribute : PropertyTraitAttribute
	{
		public float Minimum { get; private set; }
		public float Maximum { get; private set; }

		public string MinimumMethod { get; private set; }
		public string MaximumMethod { get; private set; }

		public SliderAttribute(float minimum, float maximum) : base(ControlPhase, 0)
		{
			Minimum = minimum;
			Maximum = maximum;
		}

		public SliderAttribute(string minimumMethod, string maximumMethod) : base(ControlPhase, 0)
		{
			MinimumMethod = minimumMethod;
			MaximumMethod = maximumMethod;
		}
	}
}