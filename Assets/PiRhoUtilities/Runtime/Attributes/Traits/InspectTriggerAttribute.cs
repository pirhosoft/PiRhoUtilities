namespace PiRhoSoft.Utilities
{
	public class InspectTriggerAttribute : PropertyTraitAttribute
	{
		public string Method { get; private set; }

		public InspectTriggerAttribute(string method) : base(TestPhase, 0)
		{
			Method = method;
		}
	}
}