using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Snap Vector2")]
	public class SnapVector2Sample : MonoBehaviour
	{
		[MessageBox("The [Snap] snaps a numeric value to a desired multiple. The snap value can be hard coded or retrieved from a field, property, or method.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Snap(5, 5)]
		public Vector2 HardCodedMultiplesOf5;

		public Vector2 FieldSnapValue = new Vector2(3, 3);
		[Snap(nameof(FieldSnapValue))]
		public Vector2 SnapFromField;

		public Vector2 PropertySnapValue = new Vector2(2, 2);
		[Snap(nameof(PropertySnap))]
		public Vector2 SnapFromProperty;

		public Vector2 MethodSnapValue = new Vector2(7, 7);
		[Snap(nameof(MethodSnap))]
		public Vector2 FromMethod;

		public Vector2 PropertySnap => PropertySnapValue;
		public Vector2 MethodSnap() => MethodSnapValue;
	}
}