using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Message")]
	public class MessageSample : MonoBehaviour
	{
		[MessageBox("The [Message] attribute is exactly what this is. It shows a message next to a field.", MessageBoxType.Info, Location = TraitLocation.Above)]
		[NoLabel]
		[ReadOnly]
		public string Message = "^ That is a message ^";
	}
}