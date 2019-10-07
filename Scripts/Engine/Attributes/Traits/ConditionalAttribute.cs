namespace PiRhoSoft.Utilities
{
	public enum ConditionalSource
	{
		Sibling,
		Field,
		Property,
		Method
	}

	public enum ConditionalTest
	{
		Equal,
		Inequal,
		LessThan,
		GreaterThan,
		LessThanOrEqual,
		GreaterThanOrEqual
	}

	public class ConditionalAttribute : PropertyTraitAttribute
	{
		public const int Order = 0;

		public string SourceName { get; private set; }
		public bool BoolValue { get; private set; }
		public int IntValue { get; private set; }
		public float FloatValue { get; private set; }
		public string StringValue { get; private set; }

		public ConditionalSource Source { get; set; }
		public ConditionalTest Test { get; set; }

		public ConditionalAttribute(string source, bool boolValue) : base(TestPhase, Order)
		{
			SourceName = source;
			BoolValue = boolValue;
		}

		public ConditionalAttribute(string source, int intValue) : base(TestPhase, Order)
		{
			SourceName = source;
			IntValue = intValue;
		}

		public ConditionalAttribute(string source, float floatValue) : base(TestPhase, Order)
		{
			SourceName = source;
			FloatValue = floatValue;
		}

		public ConditionalAttribute(string source, string stringValue) : base(TestPhase, Order)
		{
			SourceName = source;
			StringValue = stringValue;
		}
	}
}