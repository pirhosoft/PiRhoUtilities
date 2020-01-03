using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/InspectTrigger")]
	public class InspectTriggerSample : MonoBehaviour
	{
		[MessageBox("The [InspectTrigger] attribute will call a method on the class when the object is inspected. The method can be public/private, optionally static, and takes no parameters.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[InspectTrigger(nameof(PublicStaticTrigger))]
		public string PublicStatic;

		[InspectTrigger(nameof(PrivateStaticTrigger))]
		public string PrivateStatic;

		[InspectTrigger(nameof(PublicInstanceTrigger))]
		public string PublicInstance;

		[InspectTrigger(nameof(PrivateInstanceTrigger))]
		public string PrivateInstance;

		public static void PublicStaticTrigger()
		{
			Debug.Log("Called a public static method");
		}

		private static void PrivateStaticTrigger()
		{
			Debug.Log("Called a private static method");
		}

		public void PublicInstanceTrigger()
		{
			Debug.Log("Called a public instance method", this);
		}

		private void PrivateInstanceTrigger()
		{
			Debug.Log("Called a private instance method", this);
		}
	}
}