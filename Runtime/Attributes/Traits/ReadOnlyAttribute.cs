namespace PiRhoSoft.Utilities
{
	public class ReadOnlyAttribute : PropertyTraitAttribute
	{
		public ReadOnlyAttribute() : base(PerContainerPhase, 10)
		{
		}
	}
}