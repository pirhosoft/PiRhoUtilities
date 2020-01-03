using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Custom Label")]
	public class CustomLabelSample : MonoBehaviour
	{
		[MessageBox("The [CustomLabel] attribute sets the label of a field to something other than the friendly name. It can be hard coded or retrieved from another field, method, or property.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[CustomLabel("Hard Coded (Custom)")]
		public string HardCoded;

		public int PropertyValue = 0;
		[CustomLabel(nameof(PropertyLabel), true)]
		public string FromProperty;

		public int MethodValue = 0;
		[CustomLabel(nameof(MethodLabel), true)]
		public string FromMethod;

		public string PropertyLabel => $"From Property: {PropertyValue}";
		public string MethodLabel() => $"From Method: {MethodValue}";
	}
}