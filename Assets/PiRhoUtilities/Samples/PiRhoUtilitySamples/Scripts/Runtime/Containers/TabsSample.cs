using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Tabs")]
	public class TabsSample : MonoBehaviour
	{
		[MessageBox("The [Tabs] attribute displays fields together in a tab like view.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Tabs("Tabs", "One")] public int Int1;
		[Tabs("Tabs", "One")] public float Float1;
		[Tabs("Tabs", "Two")] [Maximum(100)] public float Float2;
		[Tabs("Tabs", "One")] public bool Bool1;
		[Tabs("Tabs", "Two")] public bool Bool2;
		[Tabs("Tabs", "Two")] public int Int2;
	}
}