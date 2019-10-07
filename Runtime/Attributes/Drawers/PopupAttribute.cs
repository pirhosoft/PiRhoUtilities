using System.Collections.Generic;
using System.Linq;

namespace PiRhoSoft.Utilities
{
	public class PopupAttribute : PropertyTraitAttribute
	{
		public List<int> IntValues { get; private set; }
		public List<float> FloatValues { get; private set; }
		public List<string> Options { get; private set; }

		public PopupAttribute(string[] options) : base(ContainerPhase, 0)
		{
			Options = options.ToList();
		}

		public PopupAttribute(int[] values, string[] options = null) : base(ContainerPhase, 0)
		{
			IntValues = values.ToList();
			Options = options?.ToList();
		}

		public PopupAttribute(float[] values, string[] options = null) : base(ContainerPhase, 0)
		{
			FloatValues = values.ToList();
			Options = options?.ToList();
		}
	}
}