using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Execution")]
	public class ExecutionSample : MonoBehaviour
	{
		[Button(nameof(Clicked), Label = "Click")]
		[ChangeTrigger(nameof(Changed))]
		public bool Toggle;

		private void Clicked() => Debug.Log("Clicked", this);
		private void Changed(bool oldValue, bool newValue) => Debug.Log($"Changed from {oldValue} to {newValue}", this);
	}
}