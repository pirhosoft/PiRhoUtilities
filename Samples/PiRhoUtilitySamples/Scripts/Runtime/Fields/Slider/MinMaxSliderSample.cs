using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/MinMaxSlider")]
	public class MinMaxSliderSample : MonoBehaviour
	{
		[MessageBox("The [Slider] attribute can be applied to a Vector2 to show the selection as a min/max slider. The minimum and maximum values can be hard coded or retrieved from another field, method, or property.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Slider(0, 10)]
		public Vector2 HardCodedOptions = Vector2.one;

		public float FieldMinimum = 0;
		public float FieldMaximum = 10;
		[Slider(nameof(FieldMinimum), nameof(FieldMaximum))]
		public Vector2 FromField = Vector2.one;

		public float PropertyMinimum = 0;
		public float PropertyMaximum = 10;
		[Slider(nameof(GetPropertyMinimum), nameof(GetPropertyMaximum))]
		public Vector2 FromProperty = Vector2.one;

		public float MethodMinimum = 0;
		public float MethodMaximum = 10;
		[Slider(nameof(GetMethodMinimum), nameof(GetMethodMaximum))]
		public Vector2 FromMethod = Vector2.one;

		public float GetPropertyMinimum => PropertyMinimum;
		public float GetPropertyMaximum => PropertyMaximum;
		public float GetMethodMinimum() => MethodMinimum;
		public float GetMethodMaximum() => MethodMaximum;
	}
}