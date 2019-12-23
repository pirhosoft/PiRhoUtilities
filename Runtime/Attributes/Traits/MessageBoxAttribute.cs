namespace PiRhoSoft.Utilities
{
	public enum MessageBoxType
	{
		Info,
		Warning,
		Error
	}

	public class MessageBoxAttribute : PropertyTraitAttribute
	{
		public string Message { get; private set; }
		public MessageBoxType Type { get; private set; }
		public TraitLocation Location { get; set; }

		public MessageBoxAttribute(string message, MessageBoxType type) : base(PerContainerPhase, 0)
		{
			Message = message;
			Type = type;
		}
	}
}