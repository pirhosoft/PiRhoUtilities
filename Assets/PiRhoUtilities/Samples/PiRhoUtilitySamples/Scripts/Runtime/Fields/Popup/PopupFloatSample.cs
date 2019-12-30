using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Popup Float")]
	public class PopupFloatSample : MonoBehaviour
	{
		[MessageBox("The [Popup] attribute can be applied to a float to constrain the value to a list of values. Optional strings can be provided to display each value. Values and Options can be hard coded or retrieved from another field, method, or property.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Popup(new float[] { 0.0f, 0.25f, 0.5f, 0.75f, 1.0f }, new string[] { "Zero", "1/4", "1/2", "3/4", "One" })]
		public float HardCodedOptions = 0.0f;

		public List<float> FieldValues = new List<float> { 0.0f, 0.25f, 0.5f, 0.75f, 1.0f };
		[Popup(nameof(FieldValues))]
		public float FromField = 0.0f;

		public List<float> PropertyValues = new List<float> { 0.0f, 0.25f, 0.5f, 0.75f, 1.0f };
		[Popup(nameof(GetPropertyValues))]
		public float FromProperty = 0.0f;

		public List<float> MethodValues = new List<float> { 0.0f, 0.25f, 0.5f, 0.75f, 1.0f };
		[Popup(nameof(GetMethodValues))]
		public float FromMethod = 0.0f;

		public List<float> GetPropertyValues => PropertyValues;
		public List<float> GetMethodValues() => MethodValues;
	}
}