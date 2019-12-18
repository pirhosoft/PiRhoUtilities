using System;

namespace PiRhoSoft.Utilities
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class ButtonAttribute : PropertyTraitAttribute
	{
		public string Method { get; private set; }
		public string Label { get; private set; }
		public string Tooltip { get; set; }
		public TraitLocation Location { get; set; }

		public ButtonAttribute(string method, string label) : base(PerContainerPhase, 0)
		{
			Method = method;
			Label = label;
		}
	}
}