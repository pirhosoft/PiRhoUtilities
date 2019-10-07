namespace PiRhoSoft.Utilities
{
	public enum GroupStyle
	{
		Frame,
		Rollout
	}

	public class GroupAttribute : PropertyTraitAttribute
	{
		public string Name { get; private set; }
		public GroupStyle Style { get; set; }

		public GroupAttribute(string name) : base(ContainerPhase, 0)
		{
			Name = name;
		}
	}
}