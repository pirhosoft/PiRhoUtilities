using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Snap Vector2Int")]
	public class SnapVector2IntSample : MonoBehaviour
	{
		[MessageBox("The [Snap] snaps a numeric value to a desired multiple. The snap value can be hard coded or retrieved from a field, property, or method.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Snap(5, 5)]
		public Vector2Int HardCodedMultiplesOf5;

		public Vector2Int FieldSnapValue = new Vector2Int(3, 3);
		[Snap(nameof(FieldSnapValue))]
		public Vector2Int SnapFromField;

		public Vector2Int PropertySnapValue = new Vector2Int(2, 2);
		[Snap(nameof(PropertySnap))]
		public Vector2Int SnapFromProperty;

		public Vector2Int MethodSnapValue = new Vector2Int(7, 7);
		[Snap(nameof(MethodSnap))]
		public Vector2Int FromMethod;

		public Vector2Int PropertySnap => PropertySnapValue;
		public Vector2Int MethodSnap() => MethodSnapValue;
	}
}