using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/TypePicker")]
	public class TypePickerSample : MonoBehaviour
	{
		[MessageBox("The [TypePicker] attribute can be applied to a string field to show a searchable dropdown window of all desired types similar to the AddComponent menu.", MessageBoxType.Info, Location = TraitLocation.Above)]
		[TypePicker(typeof(Component))]
		public string Picker;
	}
}