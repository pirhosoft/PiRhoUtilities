using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Snap Vector4")]
	public class SnapVector4Sample : MonoBehaviour
	{
		[MessageBox("The [Snap] snaps a numeric value to a desired multiple. The snap value can be hard coded or retrieved from a field, property, or method.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Snap(5, 5, 5, 5)]
		public Vector4 HardCodedMultiplesOf5;

		public Vector4 FieldSnapValue = new Vector4(3, 3, 3, 3);
		[Snap(nameof(FieldSnapValue))]
		public Vector4 SnapFromField;

		public Vector4 PropertySnapValue = new Vector4(2, 2, 2, 2);
		[Snap(nameof(PropertySnap))]
		public Vector4 SnapFromProperty;

		public Vector4 MethodSnapValue = new Vector4(7, 7, 7, 7);
		[Snap(nameof(MethodSnap))]
		public Vector4 FromMethod;

		public Vector4 PropertySnap => PropertySnapValue;
		public Vector4 MethodSnap() => MethodSnapValue;
	}
}
