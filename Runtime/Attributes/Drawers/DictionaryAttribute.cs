namespace PiRhoSoft.Utilities
{
	public class DictionaryAttribute : PropertyTraitAttribute
	{
		public const string Always = "";
		public const string Never = null;

		public string AllowAdd = Always;
		public string AllowRemove = Always;
		public string AllowReorder = Never;
		public string EmptyLabel = null;

		public string AddCallback = null;
		public string RemoveCallback = null;
		public string ReorderCallback = null;

		public DictionaryAttribute() : base(ContainerPhase, 0)
		{
		}
	}
}