using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Maximum Length")]
	public class MaximumLengthSample : MonoBehaviour
	{
		[MessageBox("The [MaximumLength] attribute sets the maxmimum length of a string field. It can be hard coded or retrieved from another field, method, or property.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[MaximumLength(5)]
		public string MaximumOf5Characters;

		public int PropertyValue = 0;
		[MaximumLength(nameof(PropertyLabel), true)]
		public string MaximumFromProperty;

		public int MethodValue = 0;
		[MaximumLength(nameof(MethodLabel), true)]
		public string MaxmimumFromMethod;

		public int PropertyLabel => PropertyValue;
		public int MethodLabel() => MethodValue;
	}
}