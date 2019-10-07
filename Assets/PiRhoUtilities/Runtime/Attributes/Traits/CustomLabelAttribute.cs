namespace PiRhoSoft.Utilities
{
	public class CustomLabelAttribute : PropertyTraitAttribute
	{
		public string Label { get; private set; }
		public string Method { get; private set; }

		public CustomLabelAttribute(string label = null, string method = null) : base(PerContainerPhase, 0)
		{
			Label = label;
			Method = method;
		}
	}
}