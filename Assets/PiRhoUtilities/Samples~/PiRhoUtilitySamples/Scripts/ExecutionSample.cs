using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Execution")]
	public class ExecutionSample : MonoBehaviour
	{
		[Button(nameof(Clicked), Label = "Click")]
		[ChangeTrigger(nameof(Changed))]
		[InspectTrigger(nameof(Inspect))]
		public bool Toggle;

		private void Clicked() => Debug.Log("Clicked", this);
		private void Changed(bool oldValue, bool newValue) => Debug.Log($"Changed from {oldValue} to {newValue}", this);
		private void Inspect() => Debug.Log("Object selected", this);
	}
}