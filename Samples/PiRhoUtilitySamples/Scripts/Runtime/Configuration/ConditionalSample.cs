using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Conditional")]
	public class ConditionalSample : MonoBehaviour
	{
		public enum ConditionalEnumTest
		{
			Set,
			NotSet
		}

		[MessageBox("The [Conditional] attribute shows and hides a field based on the value of a different field, property, or method. Conditions can be tested against bools, ints, floats, strings, Objects, and enums.", MessageBoxType.Info, Location = TraitLocation.Above)]

		public bool ShowIfTrue = true;
		[Conditional(nameof(ShowIfTrue), BoolTest.ShowIfTrue)]
		[NoLabel]
		[ReadOnly]
		public string VisibleIfTrue = "Visible";

		public bool ShowIfFalse = true;
		[Conditional(nameof(ShowIfFalse), BoolTest.ShowIfFalse)]
		[NoLabel]
		[ReadOnly]
		public string VisibleIfFalse = "Visible";

		public int ShowIfEqual0 = 0;
		[Conditional(nameof(ShowIfEqual0), 0, NumberTest.ShowIfEqual)]
		[NoLabel]
		[ReadOnly]
		public string VisibleIfEqual0 = "Visible";

		public int ShowIfInequal0 = 0;
		[Conditional(nameof(ShowIfInequal0), 0, NumberTest.ShowIfInequal)]
		[NoLabel]
		[ReadOnly]
		public string VisibleIfInequal0 = "Visible";

		public int ShowIfLess0 = 0;
		[Conditional(nameof(ShowIfLess0), 0, NumberTest.ShowIfLessThan)]
		[NoLabel]
		[ReadOnly]
		public string VisibleIfLess0 = "Visible";

		public int ShowIfLessOrEqual0 = 0;
		[Conditional(nameof(ShowIfLessOrEqual0), 0, NumberTest.ShowIfLessThanOrEqual)]
		[NoLabel]
		[ReadOnly]
		public string VisibleIfLessOrEqual0 = "Visible";

		public int ShowIfGreater0 = 0;
		[Conditional(nameof(ShowIfGreater0), 0, NumberTest.ShowIfGreaterThan)]
		[NoLabel]
		[ReadOnly]
		public string VisibleIfGreater0 = "Visible";
			
		public int ShowIfGreaterOrEqual0 = 0;
		[Conditional(nameof(ShowIfGreaterOrEqual0), 0, NumberTest.ShowIfGreaterThanOrEqual)]
		[NoLabel]
		[ReadOnly]
		public string VisibleIfGreaterOrEqual0 = "Visible";

		public float ShowIfLess0f = 0.0f;
		[Conditional(nameof(ShowIfLess0f), 0.0f, NumberTest.ShowIfLessThan)]
		[NoLabel]
		[ReadOnly]
		public string VisibleIfLess0f = "Visible";

		public float ShowIfGreater0f = 0.0f;
		[Conditional(nameof(ShowIfGreater0f), 0.0f, NumberTest.ShowIfGreaterThan)]
		[NoLabel]
		[ReadOnly]
		public string VisibleIfGreater0f = "Visible";

		public ConditionalEnumTest ShowIfSet = ConditionalEnumTest.Set;
		[Conditional(nameof(ShowIfSet), (int)ConditionalEnumTest.Set, EnumTest.ShowIfEqual)]
		[NoLabel]
		[ReadOnly]
		public string VisibleIfSet = "Visible";

		public ConditionalEnumTest ShowIfNotSet = ConditionalEnumTest.Set;
		[Conditional(nameof(ShowIfNotSet), (int)ConditionalEnumTest.Set, EnumTest.ShowIfInequal)]
		[NoLabel]
		[ReadOnly] 
		public string VisibleIfNotSet = "Visible";

		public string ShowIfTest = "Test";
		[Conditional(nameof(ShowIfTest), "Test", StringTest.ShowIfSame)]
		[NoLabel]
		[ReadOnly]
		public string VisibleIfSame = "Visible";
			
		public string ShowIfNotTest = "Test";
		[Conditional(nameof(ShowIfNotTest), "Test", StringTest.ShowIfDifferent)]
		[NoLabel]
		[ReadOnly]
		public string VisibleIfDifferent = "Visible";

		public Object ShowIfNotNull;
		[Conditional(nameof(ShowIfNotNull), ObjectTest.ShowIfSet)]
		[NoLabel]
		[ReadOnly]
		public string VisibleNotNull = "Visible";

		public Object ShowIfNull;
		[Conditional(nameof(ShowIfNull), ObjectTest.ShowIfNotSet)]
		[NoLabel]
		[ReadOnly]
		public string VisibleIfNull = "Visible";

		public bool ControledByField = true;
		[Conditional(nameof(ControledByField), BoolTest.ShowIfTrue)]
		[NoLabel]
		[ReadOnly]
		public string ShownByField = "Visible";

		public bool ControledByProperty = true;
		[Conditional(nameof(ShownProperty), BoolTest.ShowIfTrue)]
		[NoLabel]
		[ReadOnly]
		public string ShownByProperty = "Visible";

		public bool ControledByMethod = true;
		[Conditional(nameof(ShownMethod), BoolTest.ShowIfTrue)]
		[NoLabel]
		[ReadOnly]
		public string ShownByMethod = "Visible";

		public bool ShownProperty => ControledByProperty;
		public bool ShownMethod() => ControledByMethod;
	}
}