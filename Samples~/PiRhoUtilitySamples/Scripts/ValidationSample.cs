using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Validation")]
	public class ValidationSample : MonoBehaviour
	{
		[Required("A string must be entered")]
		public string RequiredString;

		[Required("An object must be selected")]
		public GameObject RequiredObject;

		[Validate(nameof(IsOdd), "An odd number must be entered")]
		public int OddInt;
		private bool IsOdd() => OddInt % 2 != 0;

		[Maximum(10)]
		public int MaximumInt;

		[Minimum(5)]
		public int MinimumInt;

		[Minimum(5)] [Maximum(10)]
		public int ClampedInt;

		[Maximum(10.0f)]
		public int MaximumFloat;

		[Minimum(5.0f)]
		public float MinimumFloat;

		[Minimum(5.0f)] [Maximum(10.0f)]
		public int ClampedFloat;

		[Snap(5)]
		public int SnappedInt;

		[Snap(0.5f)]
		public float SnappedFloat;

		[MaximumLength(5)]
		public string MaximumString;

		[Required("A string less than 5 characters must be entered")] [MaximumLength(5)]
		public string RequiredShortString;
	}
}