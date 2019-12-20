using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Snap Rect")]
	public class SnapRectSample : MonoBehaviour
	{
		[MessageBox("The [Snap] snaps a numeric value to a desired multiple. The snap value can be hard coded or retrieved from a field, property, or method.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Snap(5, 5, 5, 5)]
		public Rect HardCodedMultiplesOf5;

		public Rect FieldSnapValue = new Rect(3, 3, 3, 3);
		[Snap(nameof(FieldSnapValue))]
		public Rect SnapFromField;

		public Rect PropertySnapValue = new Rect(2, 2, 2, 2);
		[Snap(nameof(PropertySnap))]
		public Rect SnapFromProperty;

		public Rect MethodSnapValue = new Rect(7, 7, 7, 7);
		[Snap(nameof(MethodSnap))]
		public Rect FromMethod;

		public Rect PropertySnap => PropertySnapValue;
		public Rect MethodSnap() => MethodSnapValue;
	}
}