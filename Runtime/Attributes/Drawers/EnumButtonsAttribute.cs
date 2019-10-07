namespace PiRhoSoft.Utilities
{
	public class EnumButtonsAttribute : PropertyTraitAttribute
	{
		public bool? Flags { get; private set; }

		public EnumButtonsAttribute() : base(ControlPhase, 0) => Flags = null;
		public EnumButtonsAttribute(bool flags) : base(ControlPhase, 0) => Flags = flags;
	}
}