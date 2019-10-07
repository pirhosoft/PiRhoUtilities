namespace PiRhoSoft.Utilities
{
	public class RequiredAttribute : PropertyTraitAttribute
	{
		public string Message { get; private set; }
		public TraitMessageType Type { get; private set; }

		public RequiredAttribute(string message, TraitMessageType type = TraitMessageType.Warning) : base(ValidatePhase, 0)
		{
			Message = message;
			Type = type;
		}
	}
}