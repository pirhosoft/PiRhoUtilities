using System;
using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/EnumButtons")]
	public class EnumButtonsSample : MonoBehaviour
	{
		public enum ButtonsEnum
		{
			One,
			Two,
			Buckle,
			My,
			Shoe
		}

		[Flags]
		public enum EnumFlags
		{
			None = 0,
			Bit1 = 1,
			Bit2 = 1 << 1,
			Bit3 = 1 << 2,
			Bit4 = 1 << 3,
			All = ~0
		}

		[MessageBox("The [EnumButtons] attribute can be applied to an Enum to make them appear as buttons instead of a dropdown", MessageBoxType.Info, Location = TraitLocation.Above)]

		[EnumButtons]
		public ButtonsEnum NormalButtons;

		[EnumButtons]
		public EnumFlags FlagsButton;
	}
}