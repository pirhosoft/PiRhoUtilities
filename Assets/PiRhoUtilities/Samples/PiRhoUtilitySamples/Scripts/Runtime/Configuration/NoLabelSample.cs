using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/No Label")]
	public class NoLabelSample : MonoBehaviour
	{
		[MessageBox("The [NoLabel] attribute removes the label from a field", MessageBoxType.Info, Location = TraitLocation.Above)]

		[NoLabel]
		public string NoLabel = "<- notice how there is no label over there";
	}
}