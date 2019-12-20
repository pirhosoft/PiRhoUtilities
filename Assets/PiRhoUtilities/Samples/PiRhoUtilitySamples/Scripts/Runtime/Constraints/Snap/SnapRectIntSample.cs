using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Snap RectInt")]
	public class SnapRectIntSample : MonoBehaviour
	{
		[MessageBox("The [Snap] snaps a numeric value to a desired multiple. The snap value can be hard coded or retrieved from a field, property, or method.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Snap(5, 5, 5, 5)]
		public RectInt HardCodedMultiplesOf5;

		public RectInt FieldSnapValue = new RectInt(3, 3, 3, 3);
		[Snap(nameof(FieldSnapValue))]
		public RectInt SnapFromField;

		public RectInt PropertySnapValue = new RectInt(2, 2, 2, 2);
		[Snap(nameof(PropertySnap))]
		public RectInt SnapFromProperty;

		public RectInt MethodSnapValue = new RectInt(7, 7, 7, 7);
		[Snap(nameof(MethodSnap))]
		public RectInt FromMethod;

		public RectInt PropertySnap => PropertySnapValue;
		public RectInt MethodSnap() => MethodSnapValue;
	}
}