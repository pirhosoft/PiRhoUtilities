namespace PiRhoSoft.Utilities
{
	public class MaximumAttribute : PropertyTraitAttribute
	{
		public const int Order = 1;

		public float Maximum { get; private set; }
		public string Compare { get; private set; }

		public MaximumAttribute(float maximum) : base(ValidatePhase, Order)
		{
			Maximum = maximum;
		}

		public MaximumAttribute(int maximum) : base(ValidatePhase, Order)
		{
			Maximum = maximum;
		}

		public MaximumAttribute(string maximum) : base(ValidatePhase, Order)
		{
			Compare = maximum;
		}
	}
}