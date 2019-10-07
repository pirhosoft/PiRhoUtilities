namespace PiRhoSoft.Utilities
{
	public class ScenePickerAttribute : PropertyTraitAttribute
	{
		public string CreateMethod { get; private set; }

		public ScenePickerAttribute() : this(string.Empty)
		{
		}

		public ScenePickerAttribute(string createMethod) : base(ContainerPhase, 0)
		{
			CreateMethod = createMethod;
		}
	}
}
