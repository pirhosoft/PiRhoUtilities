namespace PiRhoSoft.Utilities
{
	public class PlaceholderAttribute : PropertyTraitAttribute
	{
		public string Text { get; private set; }

		public PlaceholderAttribute(string text) : base(FieldPhase, 0)
		{
			Text = text;
		}
	}
}