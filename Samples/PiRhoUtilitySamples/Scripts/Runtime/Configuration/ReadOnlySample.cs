using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/ReadOnly")]
	public class ReadOnlySample : MonoBehaviour
	{
		[MessageBox("The [ReadOnly] attribute disables editing of the field", MessageBoxType.Info, Location = TraitLocation.Above)]

		[ReadOnly]
		public string ReadOnly = "This string cannot be changed in the editor";
	}
}