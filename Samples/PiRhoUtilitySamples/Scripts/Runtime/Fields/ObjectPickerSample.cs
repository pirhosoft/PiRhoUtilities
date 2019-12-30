using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/ObjectPicker")]
	public class ObjectPickerSample : MonoBehaviour
	{
		[MessageBox("The [ObjectPicker] attribute can be applied to any Object field to show all objects of the desired type in a searchable dropdown window similar to the AddComponent menu.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[ObjectPicker]
		public GameObject Picker;
	}
}