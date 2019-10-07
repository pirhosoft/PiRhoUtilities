using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(MultilineAttribute))]
	class MultilineDrawer : PropertyDrawer
	{
		private const string _invalidDrawerWarning = "(PUMDID) invalid drawer for MultilineAttribute on field {0}: the element does not have a TextField";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var element = this.CreateNextElement(property);
			var input = element.Query<TextField>().First();

			if (input != null)
				input.multiline = true;
			else
				Debug.LogWarningFormat(_invalidDrawerWarning, property.propertyPath);

			return element;
		}
	}
}