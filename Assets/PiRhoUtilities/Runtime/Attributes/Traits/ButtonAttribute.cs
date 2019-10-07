using System;

namespace PiRhoSoft.Utilities
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class ButtonAttribute : PropertyTraitAttribute
	{
		public const int Phase = PerContainerPhase;
		public const int Order = 0;

		public string Method { get; private set; }

		public string Label { get; set; }
		public string Tooltip { get; set; }
		public TraitLocation Location { get; set; }

		public ButtonAttribute(string method) : base(PerContainerPhase, 0)
		{
			Method = method;
		}
	}
}