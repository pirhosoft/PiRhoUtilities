namespace PiRhoSoft.Utilities
{
	public class TabsAttribute : PropertyTraitAttribute
	{
		public string Group { get; private set; }
		public string Tab { get; private set; }

		public TabsAttribute(string group, string tab) : base(ContainerPhase, 0)
		{
			Group = group;
			Tab = tab;
		}
	}
}