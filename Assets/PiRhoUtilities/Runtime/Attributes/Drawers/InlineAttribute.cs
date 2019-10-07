namespace PiRhoSoft.Utilities
{
	public class InlineAttribute : PropertyTraitAttribute
	{
		public bool ShowMemberLabels { get; private set; }

		public InlineAttribute(bool showMemberLabels = true) : base(ControlPhase, 0)
		{
			ShowMemberLabels = showMemberLabels;
		}
	}
}