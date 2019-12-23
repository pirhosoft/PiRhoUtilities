using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Snap BoundsInt")]
	public class SnapBoundsIntSample : MonoBehaviour
	{
		[MessageBox("The [Snap] snaps a numeric value to a desired multiple. The snap value can be hard coded or retrieved from a field, property, or method.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Snap(5, 5, 5, 5, 5, 5)]
		public BoundsInt HardCodedMultiplesOf5;

		public BoundsInt FieldSnapValue = new BoundsInt(2, 2, 2, 2, 2, 2);
		[Snap(nameof(FieldSnapValue))]
		public BoundsInt SnapFromField;

		public BoundsInt PropertySnapValue = new BoundsInt(3, 3, 3, 3, 3, 3);
		[Snap(nameof(PropertySnap))]
		public BoundsInt SnapFromProperty;

		public BoundsInt MethodSnapValue = new BoundsInt(7, 7, 7, 7, 7, 7);
		[Snap(nameof(MethodSnap))]
		public BoundsInt FromMethod;

		public BoundsInt PropertySnap => PropertySnapValue;
		public BoundsInt MethodSnap() => MethodSnapValue;
	}
}
