using System.Collections.Generic;
using System.Linq;

namespace PiRhoSoft.Utilities
{
	public class PopupAttribute : PropertyTraitAttribute
	{
		public List<int> IntValues { get; private set; }
		public List<float> FloatValues { get; private set; }
		public List<string> StringValues { get; private set; }
		public List<string> Options { get; private set; }
		public string ValuesSource { get; private set; }
		public string OptionsSource { get; private set; }
		public bool AutoUpdate { get; private set; }

		public PopupAttribute(string[] values, string[] options = null) : base(ControlPhase, 0)
		{
			StringValues = values.ToList();
			Options = options?.ToList();
		}

		public PopupAttribute(int[] values, string[] options = null) : base(ControlPhase, 0)
		{
			IntValues = values.ToList();
			Options = options?.ToList();
		}

		public PopupAttribute(float[] values, string[] options = null) : base(ControlPhase, 0)
		{
			FloatValues = values.ToList();
			Options = options?.ToList();
		}

		public PopupAttribute(string valuesSource, string optionsSource = null, bool autoUpdate = true) : base(ControlPhase, 0)
		{
			ValuesSource = valuesSource;
			OptionsSource = optionsSource;
			AutoUpdate = autoUpdate;
		}
	}
}