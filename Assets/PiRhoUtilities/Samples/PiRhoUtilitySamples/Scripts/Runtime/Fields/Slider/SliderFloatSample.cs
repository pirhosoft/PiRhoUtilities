using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Slider Float")]
	public class SliderFloatSample : MonoBehaviour
	{
		[MessageBox("The [Slider] attribute can be applied to a float to show the selection as a slider. The minimum and maximum values can be hard coded or retrieved from another field, method, or property.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Slider(0, 10)]
		public float HardCodedOptions = 5;

		public float FieldMinimum = 0;
		public float FieldMaximum = 10;
		[Slider(nameof(FieldMinimum), nameof(FieldMaximum))]
		public float FromField = 5;

		public float PropertyMinimum = 0;
		public float PropertyMaximum = 10;
		[Slider(nameof(GetPropertyMinimum), nameof(GetPropertyMaximum))]
		public float FromProperty = 5;

		public float MethodMinimum = 0;
		public float MethodMaximum = 10;
		[Slider(nameof(GetMethodMinimum), nameof(GetMethodMaximum))]
		public float FromMethod = 5;

		public float GetPropertyMinimum => PropertyMinimum;
		public float GetPropertyMaximum => PropertyMaximum;
		public float GetMethodMinimum() => MethodMinimum;
		public float GetMethodMaximum() => MethodMaximum;
	}
}