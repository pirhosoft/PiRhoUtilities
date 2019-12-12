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
		public string ValuesMethod { get; private set; }
		public string OptionsMethod { get; private set; }

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

		public PopupAttribute(string valuesMethod, string optionsMethod = null) : base(ControlPhase, 0)
		{
			ValuesMethod = valuesMethod;
			OptionsMethod = optionsMethod;
		}
	}
}