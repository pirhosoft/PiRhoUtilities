namespace PiRhoSoft.Utilities
{
	public class PlaceholderAttribute : PropertyTraitAttribute
	{
		public string Text { get; private set; }
		public string TextSource { get; private set; }
		public bool AutoUpdate { get; private set; }

		public PlaceholderAttribute(string text) : base(FieldPhase, 0)
		{
			Text = text;
		}

		public PlaceholderAttribute(string textSource, bool autoUpdate) : base(FieldPhase, 0)
		{
			TextSource = textSource;
			AutoUpdate = autoUpdate;
		}
	}
}