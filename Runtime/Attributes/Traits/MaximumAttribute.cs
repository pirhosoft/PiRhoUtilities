namespace PiRhoSoft.Utilities
{
	public class MaximumAttribute : PropertyTraitAttribute
	{
		public const int Order = 1;

		public float Maximum { get; private set; }
		public string MaximumSource { get; private set; }
		public bool AutoUpdate { get; private set; }

		public MaximumAttribute(float maximum) : base(ValidatePhase, Order)
		{
			Maximum = maximum;
		}

		public MaximumAttribute(int maximum) : base(ValidatePhase, Order)
		{
			Maximum = maximum;
		}

		public MaximumAttribute(string maximumSource, bool autoUpdate = true) : base(ValidatePhase, Order)
		{
			MaximumSource = maximumSource;
			AutoUpdate = autoUpdate;
		}
	}
}