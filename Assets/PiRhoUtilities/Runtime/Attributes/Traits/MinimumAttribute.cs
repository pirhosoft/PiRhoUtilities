namespace PiRhoSoft.Utilities
{
	public class MinimumAttribute : PropertyTraitAttribute
	{
		public const int Order = 1;

		public float Minimum { get; private set; }
		public string MinimumSource { get; private set; }
		public bool AutoUpdate { get; private set; }

		public MinimumAttribute(float minimum) : base(ValidatePhase, Order)
		{
			Minimum = minimum;
		}

		public MinimumAttribute(int minimum) : base(ValidatePhase, Order)
		{
			Minimum = minimum;
		}

		public MinimumAttribute(string maximumSource, bool autoUpdate = true) : base(ValidatePhase, Order)
		{
			MinimumSource = maximumSource;
			AutoUpdate = autoUpdate;
		}
	}
}