namespace PiRhoSoft.Utilities
{
	public class RequiredAttribute : PropertyTraitAttribute
	{
		public string Message { get; private set; }
		public MessageBoxType Type { get; private set; }

		public RequiredAttribute(string message, MessageBoxType type = MessageBoxType.Warning) : base(ValidatePhase, 0)
		{
			Message = message;
			Type = type;
		}
	}
}