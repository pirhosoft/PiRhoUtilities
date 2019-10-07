namespace PiRhoSoft.Utilities
{
	public class MaximumLengthAttribute : PropertyTraitAttribute
	{
		public int Length { get; private set; }

		public MaximumLengthAttribute(int length) : base(ValidatePhase, 1)
		{
			Length = length;
		}
	}
}