using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Popup String")]
	public class PopupStringSample : MonoBehaviour
	{
		[MessageBox("The [Popup] attribute can be applied to a string to constrain the value to a list of options. The options can be hard coded or retrieved from another field, method, or property.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Popup(new string[] { "One Fish", "Two Fish", "Red Fish", "Blue Fish" })]
		public string HardCodedOptions = "One Fish";

		public List<string> PropertyField = new List<string> { "Field Option One", "Field Option Two", "Field Option Three" };
		[Popup(nameof(PropertyField))]
		public string FromField = "Field Option One";

		public List<string> PropertyOptions = new List<string> { "Property Option One", "Property Option Two", "Property Option Three" };
		[Popup(nameof(GetPropertyOptions))]
		public string FromProperty = "Property Option One";

		public List<string> MethodOptions = new List<string> { "Method Option One", "Method Option Two", "Method Option Three" };
		[Popup(nameof(GetMethodOptions))]
		public string FromMethod = "Method Option One";

		public List<string> GetPropertyOptions => PropertyOptions;
		public List<string> GetMethodOptions() => MethodOptions;
	}
}