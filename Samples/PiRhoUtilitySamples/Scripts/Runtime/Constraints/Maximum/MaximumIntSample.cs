using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Maximum Int")]
	public class MaximumIntSample : MonoBehaviour
	{
		[MessageBox("The [Maximum] clamps a value to a desired maximum. The value can be hard coded or retrieved from a field, property, or method.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Maximum(0)]
		public int HardCodedMinimumOf0;

		public int FieldMinimumValue = 3;
		[Maximum(nameof(FieldMinimumValue))]
		public int MinimumFromField;

		public int PropertyMinimumValue = 2;
		[Maximum(nameof(PropertyMinimum))]
		public int MinimumFromProperty;

		public int MethodMinimumValue = 7;
		[Maximum(nameof(MethodMinimum))]
		public int FromMethod;

		public int PropertyMinimum => PropertyMinimumValue;
		public int MethodMinimum() => MethodMinimumValue;
	}
}
