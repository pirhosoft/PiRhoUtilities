using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Snap Bounds")]
	public class SnapBoundsSample : MonoBehaviour
	{
		[MessageBox("The [Snap] snaps a numeric value to a desired multiple. The snap value can be hard coded or retrieved from a field, property, or method.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Snap(5, 5, 5, 5, 5, 5)]
		public Bounds HardCodedMultiplesOf5;

		public Bounds FieldSnapValue = new Bounds(new Vector3(2, 2, 2), new Vector3(2, 2, 2));
		[Snap(nameof(FieldSnapValue))]
		public Bounds SnapFromField;

		public Bounds PropertySnapValue = new Bounds(new Vector3(3, 3, 3), new Vector3(3, 3, 3));
		[Snap(nameof(PropertySnap))]
		public Bounds SnapFromProperty;

		public Bounds MethodSnapValue = new Bounds(new Vector3(7, 7, 7), new Vector3(7, 7, 7));
		[Snap(nameof(MethodSnap))]
		public Bounds FromMethod;

		public Bounds PropertySnap => PropertySnapValue;
		public Bounds MethodSnap() => MethodSnapValue;
	}
}