namespace PiRhoSoft.Utilities
{
	public class ReferenceAttribute : PropertyTraitAttribute
	{
		public bool IsCollapsable = true;

		public ReferenceAttribute() : base(ControlPhase, 0)
		{
		}
	}
}
