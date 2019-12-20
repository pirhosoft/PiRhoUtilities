using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Snap Int")]
	public class SnapIntSample : MonoBehaviour
	{
		[MessageBox("The [Snap] snaps a numeric value to a desired multiple. The snap value can be hard coded or retrieved from a field, property, or method.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Snap(5)]
		public int HardCodedMultiplesOf5;

		public int FieldSnapValue = 3;
		[Snap(nameof(FieldSnapValue))]
		public int SnapFromField;

		public int PropertySnapValue = 2;
		[Snap(nameof(PropertySnap))]
		public int SnapFromProperty;

		public int MethodSnapValue = 7;
		[Snap(nameof(MethodSnap))]
		public int FromMethod;

		public int PropertySnap => PropertySnapValue;
		public int MethodSnap() => MethodSnapValue;
	}
}