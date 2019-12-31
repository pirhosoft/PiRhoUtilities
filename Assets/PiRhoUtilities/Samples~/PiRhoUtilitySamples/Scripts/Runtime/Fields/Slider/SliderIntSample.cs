using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Slider Int")]
	public class SliderIntSample : MonoBehaviour
	{
		[MessageBox("The [Slider] attribute can be applied to an int to show the selection as a slider. The minimum and maximum values can be hard coded or retrieved from another field, method, or property.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Slider(0, 10)]
		public int HardCodedOptions = 5;

		public int FieldMinimum = 0;
		public int FieldMaximum = 10;
		[Slider(nameof(FieldMinimum), nameof(FieldMaximum))]
		public int FromField = 5;

		public int PropertyMinimum = 0;
		public int PropertyMaximum = 10;
		[Slider(nameof(GetPropertyMinimum), nameof(GetPropertyMaximum))]
		public int FromProperty = 5;

		public int MethodMinimum = 0;
		public int MethodMaximum = 10;
		[Slider(nameof(GetMethodMinimum), nameof(GetMethodMaximum))]
		public int FromMethod = 5;

		public int GetPropertyMinimum => PropertyMinimum;
		public int GetPropertyMaximum => PropertyMaximum;
		public int GetMethodMinimum() => MethodMinimum;
		public int GetMethodMaximum() => MethodMaximum;
	}
}