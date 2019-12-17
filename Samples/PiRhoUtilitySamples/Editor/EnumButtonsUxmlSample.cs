using System;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Samples
{
	public class EnumButtonsUxmlSample : UxmlSample
	{
		// These enums can be referenced in uxml by their namespace qualified name (Type.FullName in c#) as can any
		// type in the default created runtime and editor assemblies (Assembly-CSharp and Assembly-CSharp-Editor). If
		// the enum is declared in any other assembly the uxml must use the assembly qualified name
		// (Type.AssemblyQualifiedName in c#). Note that nested types like these use a '+' as shown in the uxml for
		// this sample.

		public enum Games
		{
			AdventRising,
			TheOuterWilds,
			BeyondGoodAndEvil,
			EternalDarkness,
			OcarinaOfTime,
			HorizonZeroDawn
		}

		[Flags]
		public enum Books
		{
			None = 0,
			ThePillarsOfTheEarth = 1 << 0,
			HardBoiledWonderland = 1 << 1,
			RedRising = 1 << 2,
			All = ~0
		}

		public override void Setup(VisualElement uxml)
		{
		}
	}
}
