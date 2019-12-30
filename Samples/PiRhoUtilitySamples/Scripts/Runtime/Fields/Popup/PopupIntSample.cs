using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Popup Int")]
	public class PopupIntSample : MonoBehaviour
	{
		[MessageBox("The [Popup] attribute can be applied to an int to constrain the value to a list of values. Optional strings can be provided to display each value. Values and Options can be hard coded or retrieved from another field, method, or property.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Popup(new int[] { 1, 2, 4, 8, 16 }, new string[] { "2^0", "2^1", "2^2", "2^3", "2^4" })]
		public int HardCodedOptions = 1;

		public List<int> FieldValues = new List<int> { 1, 2, 4, 8, 16 };
		[Popup(nameof(FieldValues))]
		public int FromField = 1;

		public List<int> PropertyValues = new List<int> { 1, 2, 4, 8, 16 };
		[Popup(nameof(GetPropertyValues))]
		public int FromProperty = 1;

		public List<int> MethodValues = new List<int> { 1, 2, 4, 8, 16 };
		[Popup(nameof(GetMethodValues))]
		public int FromMethod = 1;

		public List<int> GetPropertyValues => PropertyValues;
		public List<int> GetMethodValues() => MethodValues;
	}
}