using System.Collections.Generic;
using System.Linq;

namespace PiRhoSoft.Utilities
{
	public class ComboBoxAttribute : PropertyTraitAttribute
	{
		public List<string> Options { get; private set; }
		public string OptionsSource { get; private set; }
		public bool AutoUpdate { get; private set; }

		public ComboBoxAttribute(string[] options) : base(ControlPhase, 0)
		{
			Options = options.ToList();
		}

		public ComboBoxAttribute(string optionsSource, bool autoUpdate = true) : base(ControlPhase, 0)
		{
			OptionsSource = optionsSource;
			AutoUpdate = autoUpdate;
		}
	}
}