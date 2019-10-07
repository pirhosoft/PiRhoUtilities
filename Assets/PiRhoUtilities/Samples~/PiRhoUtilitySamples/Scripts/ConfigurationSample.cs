using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Configuration")]
	public class ConfigurationSample : MonoBehaviour
	{
		[Multiline]
		public string MultilineText;

		[ReadOnly]
		public string ReadOnly = "Disabled control";

		[Stretch]
		public string Stretch;

		[Multiline]
		[Stretch]
		public string MultilineStretch;

		[Placeholder("placeholder")]
		public string Placeholder;

		[ChangeTrigger(nameof(Changed))]
		[Delay]
		public string DelayValidation;
		private void Changed() => Debug.Log("Changed", this);

		[CustomLabel("Show/Hide")]
		public bool Toggle;

		[Conditional(nameof(Toggle), true)]
		public int ConditionalInt;
	}
}