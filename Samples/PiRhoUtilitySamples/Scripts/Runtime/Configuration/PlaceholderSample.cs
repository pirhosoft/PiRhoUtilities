using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Placeholder")]
	public class PlaceholderSample : MonoBehaviour
	{
		[MessageBox("The [Placeholder] places descriptive text within a text field when it is empty. It can be hard coded or retrived from a field, property, or method.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Placeholder("descriptive text")]
		public string HardCoded = string.Empty;

		public string FieldPlaceholderValue = "field";
		[Placeholder(nameof(FieldPlaceholderValue), true)]
		public string FromField = string.Empty;

		public string PropertyPlaceholderValue = "property";
		[Placeholder(nameof(PropertyPlaceholder), true)]
		public string FromProperty = string.Empty;

		public string MethodPlaceholderValue = "method";
		[Placeholder(nameof(MethodPlaceholder), true)]
		public string FromMethod = string.Empty;

		public string PropertyPlaceholder => PropertyPlaceholderValue;
		public string MethodPlaceholder() => MethodPlaceholderValue;
	}
}