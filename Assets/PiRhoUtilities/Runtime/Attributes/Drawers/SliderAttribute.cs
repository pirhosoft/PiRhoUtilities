namespace PiRhoSoft.Utilities
{
	public class SliderAttribute : PropertyTraitAttribute
	{
		public const int Order = 1;

		public float Minimum { get; private set; }
		public float Maximum { get; private set; }

		public SliderAttribute(float minimum, float maximum) : base(ControlPhase, Order)
		{
			Minimum = minimum;
			Maximum = maximum;
		}
	}
}