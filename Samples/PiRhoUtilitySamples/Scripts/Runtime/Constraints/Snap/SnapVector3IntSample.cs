using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Snap Vector3Int")]
	public class SnapVector3IntSample : MonoBehaviour
	{
		[MessageBox("The [Snap] snaps a numeric value to a desired multiple. The snap value can be hard coded or retrieved from a field, property, or method.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Snap(5, 5, 5)]
		public Vector3Int HardCodedMultiplesOf5;

		public Vector3Int FieldSnapValue = new Vector3Int(3, 3, 3);
		[Snap(nameof(FieldSnapValue))]
		public Vector3Int SnapFromField;

		public Vector3Int PropertySnapValue = new Vector3Int(2, 2, 2);
		[Snap(nameof(PropertySnap))]
		public Vector3Int SnapFromProperty;

		public Vector3Int MethodSnapValue = new Vector3Int(7, 7, 7);
		[Snap(nameof(MethodSnap))]
		public Vector3Int FromMethod;

		public Vector3Int PropertySnap => PropertySnapValue;
		public Vector3Int MethodSnap() => MethodSnapValue;
	}
}