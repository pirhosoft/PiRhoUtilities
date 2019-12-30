using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/ComboBox")]
	public class ComboBoxSample : MonoBehaviour
	{
		[MessageBox("The [ComboxBox] attribute can be applied to a string to add a popup with a list of optional options. The options can be hard coded or retrieved from another field, method, or property.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[ComboBox(new string[] { "One Fish", "Two Fish", "Red Fish", "Blue Fish" })]
		public string HardCodedOptions;

		public List<string> FieldOptions = new List<string> { "Field Option One", "Field Option Two", "Field Option Three" };
		[ComboBox(nameof(FieldOptions))]
		public string FromField = "I can be any value or a predefined option";

		public List<string> PropertyOptions = new List<string> { "Property Option One", "Property Option Two", "Property Option Three" };
		[ComboBox(nameof(GetPropertyOptions))]
		public string FromProperty = "I can be any value or a predefined option";

		public List<string> MethodOptions = new List<string> { "Method Option One", "Method Option Two", "Method Option Three" };
		[ComboBox(nameof(GetMethodOptions))]
		public string FromMethod = "I can be any value or a predefined option";

		public List<string> GetPropertyOptions => PropertyOptions;
		public List<string> GetMethodOptions() => MethodOptions;
	}
}