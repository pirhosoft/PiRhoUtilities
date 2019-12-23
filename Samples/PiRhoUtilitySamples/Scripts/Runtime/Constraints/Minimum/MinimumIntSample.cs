using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Minimum Int")]
	public class MinimumIntSample : MonoBehaviour
	{
		[MessageBox("The [Minimum] clamps a value to a desired minimum. The value can be hard coded or retrieved from a field, property, or method.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Minimum(0)]
		public int HardCodedMinimumOf0;

		public int FieldMinimumValue = 3;
		[Minimum(nameof(FieldMinimumValue))]
		public int MinimumFromField;

		public int PropertyMinimumValue = 2;
		[Minimum(nameof(PropertyMinimum))]
		public int MinimumFromProperty;

		public int MethodMinimumValue = 7;
		[Minimum(nameof(MethodMinimum))]
		public int FromMethod;

		public int PropertyMinimum => PropertyMinimumValue;
		public int MethodMinimum() => MethodMinimumValue;
	}
}
