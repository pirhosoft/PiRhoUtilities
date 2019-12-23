using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Snap Float")]
	public class SnapFloatSample : MonoBehaviour
	{
		[MessageBox("The [Snap] snaps a numeric value to a desired multiple. The snap value can be hard coded or retrieved from a field, property, or method.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Snap(0.5f)]
		public float HardCodedMultiplesOfPoint5;

		public float FieldSnapValue = 3.5f;
		[Snap(nameof(FieldSnapValue))]
		public float SnapFromField;

		public float PropertySnapValue = 2.5f;
		[Snap(nameof(PropertySnap))]
		public float SnapFromProperty;

		public float MethodSnapValue = 1.5f;
		[Snap(nameof(MethodSnap))]
		public float FromMethod;

		public float PropertySnap => PropertySnapValue;
		public float MethodSnap() => MethodSnapValue;
	}
}
