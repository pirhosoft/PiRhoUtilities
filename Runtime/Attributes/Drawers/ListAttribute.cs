namespace PiRhoSoft.Utilities
{
	public class ListAttribute : PropertyTraitAttribute
	{
		public const string Always = "";
		public const string Never = null;

		public string AllowAdd = Always;
		public string AllowRemove = Always;
		public string AllowReorder = Always;
		public string EmptyLabel = null;

		public string AddCallback = null;
		public string RemoveCallback = null;
		public string ReorderCallback = null;

		public ListAttribute() : base(ContainerPhase, 0)
		{
		}
	}
}