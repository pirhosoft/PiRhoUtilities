using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Maxmimum Float")]
	public class MaximumFloatSample : MonoBehaviour
	{
		[MessageBox("The [Maximum] clamps a value to a desired maximum. The value can be hard coded or retrieved from a field, property, or method.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Maximum(0)]
		public float HardCodedMinimumOf0;

		public float FieldMinimumValue = 3;
		[Maximum(nameof(FieldMinimumValue))]
		public float MinimumFromField;

		public float PropertyMinimumValue = 2;
		[Maximum(nameof(PropertyMinimum))]
		public float MinimumFromProperty;

		public float MethodMinimumValue = 7;
		[Maximum(nameof(MethodMinimum))]
		public float FromMethod;

		public float PropertyMinimum => PropertyMinimumValue;
		public float MethodMinimum() => MethodMinimumValue;
	}
}