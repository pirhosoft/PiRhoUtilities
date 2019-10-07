using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Grouping")]
	public class GroupingSample : MonoBehaviour
	{
		[Group("One", Style = GroupStyle.Rollout)] public int Int1;
		[Group("One")] public float Float1;
		[Group("Two")] [Maximum(100)] public float Float2;
		[Group("One")] public bool Bool1;
		[Group("Two")] public bool Bool2;
		[Group("Two")] public int Int2;
	}
}