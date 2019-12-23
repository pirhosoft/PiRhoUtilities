using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Minimum Float")]
	public class MinimumFloatSample : MonoBehaviour
	{
		[MessageBox("The [Minimum] clamps a value to a desired minimum. The value can be hard coded or retrieved from a field, property, or method.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Minimum(0)]
		public float HardCodedMinimumOf0;

		public float FieldMinimumValue = 3;
		[Minimum(nameof(FieldMinimumValue))]
		public float MinimumFromField;

		public float PropertyMinimumValue = 2;
		[Minimum(nameof(PropertyMinimum))]
		public float MinimumFromProperty;

		public float MethodMinimumValue = 7;
		[Minimum(nameof(MethodMinimum))]
		public float FromMethod;

		public float PropertyMinimum => PropertyMinimumValue;
		public float MethodMinimum() => MethodMinimumValue;
	}
}