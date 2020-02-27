using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Group")]
	public class GroupSample : MonoBehaviour
	{
		[MessageBox("The [Group] attribute displays fields together in a frame like view.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Group("Group One")] public int Int1;
		[Group("Group One")] public float Float1;
		[Group("Group Two")] [Maximum(100)] public float Float2;
		[Group("Group One")] public bool Bool1;
		[Group("Group Two")] public bool Bool2;
		[Group("Group Two")] public int Int2;
	}
}
