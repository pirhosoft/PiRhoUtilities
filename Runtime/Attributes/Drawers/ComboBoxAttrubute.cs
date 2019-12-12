using System.Collections.Generic;
using System.Linq;

namespace PiRhoSoft.Utilities
{
	public class ComboBoxAttribute : PropertyTraitAttribute
	{
		public string OptionsMethod { get; private set; }
		public List<string> Options { get; private set; }

		public ComboBoxAttribute(string[] options) : base(ControlPhase, 0)
		{
			Options = options.ToList();
		}

		public ComboBoxAttribute(string optionsMethod) : base(ControlPhase, 0)
		{
			OptionsMethod = optionsMethod;
		}
	}
}