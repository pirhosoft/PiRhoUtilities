namespace PiRhoSoft.Utilities
{
	public class CustomLabelAttribute : PropertyTraitAttribute
	{
		public string Label { get; private set; }
		public string Resolve { get; private set; }

		public CustomLabelAttribute(string label = null, string resolve = null) : base(PerContainerPhase, 0)
		{
			Label = label;
			Resolve = resolve;
		}
	}
}