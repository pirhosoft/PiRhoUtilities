using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Snap Vector3")]
	public class SnapVector3Sample : MonoBehaviour
	{
		[MessageBox("The [Snap] snaps a numeric value to a desired multiple. The snap value can be hard coded or retrieved from a field, property, or method.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Snap(5, 5, 5)]
		public Vector3 HardCodedMultiplesOf5;

		public Vector3 FieldSnapValue = new Vector3(3, 3, 3);
		[Snap(nameof(FieldSnapValue))]
		public Vector3 SnapFromField;

		public Vector3 PropertySnapValue = new Vector3(2, 2, 2);
		[Snap(nameof(PropertySnap))]
		public Vector3 SnapFromProperty;

		public Vector3 MethodSnapValue = new Vector3(7, 7, 7);
		[Snap(nameof(MethodSnap))]
		public Vector3 FromMethod;

		public Vector3 PropertySnap => PropertySnapValue;
		public Vector3 MethodSnap() => MethodSnapValue;
	}
}
