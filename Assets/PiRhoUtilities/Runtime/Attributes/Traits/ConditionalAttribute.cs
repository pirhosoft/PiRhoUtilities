namespace PiRhoSoft.Utilities
{
	public enum NumberTest
	{
		ShowIfEqual,
		ShowIfInequal,
		ShowIfLessThan,
		ShowIfGreaterThan,
		ShowIfLessThanOrEqual,
		ShowIfGreaterThanOrEqual
	}

	public enum BoolTest
	{
		ShowIfTrue,
		ShowIfFalse
	}

	public enum ObjectTest
	{
		ShowIfSet,
		ShowIfNotSet
	}

	public enum StringTest
	{
		ShowIfEmpty,
		ShowIfNotEmpty,
		ShowIfEqual,
		ShowIfInequal
	}

	public enum EnumTest
	{
		ShowIfEqual,
		ShowIfInequal
	}

	public class ConditionalAttribute : PropertyTraitAttribute
	{
		public enum TestType
		{
			Bool,
			Int,
			Float,
			String,
			Enum,
			Object
		}

		public string ValueSource { get; private set; }
		public TestType Type { get; private set; }

		public int IntValue { get; private set; }
		public float FloatValue { get; private set; }
		public NumberTest NumberTest { get; private set; }

		public string StringValue { get; private set; }
		public StringTest StringTest { get; private set; }

		public BoolTest BoolTest { get; private set; }
		public EnumTest EnumTest { get; private set; }
		public ObjectTest ObjectTest { get; private set; }

		public ConditionalAttribute(string valueSource, int intValue, NumberTest test = NumberTest.ShowIfEqual) : base(TestPhase, 0)
		{
			ValueSource = valueSource;
			IntValue = intValue;
			NumberTest = test;
			Type = TestType.Int;
		}

		public ConditionalAttribute(string valueSource, float floatValue, NumberTest test = NumberTest.ShowIfEqual) : base(TestPhase, 0)
		{
			ValueSource = valueSource;
			FloatValue = floatValue;
			NumberTest = test;
			Type = TestType.Float;
		}

		public ConditionalAttribute(string valueSource, string stringValue, StringTest test = StringTest.ShowIfEqual) : base(TestPhase, 0)
		{
			ValueSource = valueSource;
			StringValue = stringValue;
			StringTest = test;
			Type = TestType.String;
		}

		public ConditionalAttribute(string valueSource, BoolTest test = BoolTest.ShowIfTrue) : base(TestPhase, 0)
		{
			ValueSource = valueSource;
			BoolTest = test;
			Type = TestType.Bool;
		}


		public ConditionalAttribute(string valueSource, int valueAsInt, EnumTest test = EnumTest.ShowIfEqual) : base(TestPhase, 0)
		{
			ValueSource = valueSource;
			IntValue = valueAsInt;
			EnumTest = test;
			Type = TestType.Enum;
		}


		public ConditionalAttribute(string valueSource, ObjectTest test = ObjectTest.ShowIfSet) : base(TestPhase, 0)
		{
			ValueSource = valueSource;
			ObjectTest = test;
			Type = TestType.Object;
		}
	}
}