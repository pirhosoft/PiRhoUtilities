using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Stretch")]
	public class StretchSample : MonoBehaviour
	{
		[MessageBox("The [Stretch] attribute places the input control below the label instead of next to it", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Stretch]
		[Multiline]
		public string Stretch = "Stretch is useful for things like \nmultiline text fields that benefit\nfrom a large editable space.";
	}
}