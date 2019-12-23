namespace PiRhoSoft.Utilities
{
	public class MaximumLengthAttribute : PropertyTraitAttribute
	{
		public int MaximumLength { get; private set; }
		public string MaximumLengthSource { get; private set; }
		public bool AutoUpdate { get; private set; }

		public MaximumLengthAttribute(int maximumLength) : base(ValidatePhase, 1)
		{
			MaximumLength = maximumLength;
		}

		public MaximumLengthAttribute(string maximumLengthSource, bool autoUpdate = true) : base(ValidatePhase, 1)
		{
			MaximumLengthSource = maximumLengthSource;
			AutoUpdate = autoUpdate;
		}
	}
}