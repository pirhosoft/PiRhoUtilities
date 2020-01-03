using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Multiline")]
	public class MultilineSample : MonoBehaviour
	{
		[MessageBox("The [Multiline] attribute tells TextFields to be multiline editable", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Multiline]
		public string MultilineText = "This text has a \nnewline in it";
	}
}