using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(MaximumLengthAttribute))]
	class MaximumLengthDrawer : PropertyDrawer
	{
		private const string _invalidDrawerWarning = "(PUMLDID) invalid drawer for MaximumLengthAttribute on field {0}: the element does not have a TextField";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var element = this.CreateNextElement(property);
			var input = element.Query<TextField>().First();

			if (input != null)
				input.maxLength = (attribute as MaximumLengthAttribute).Length;
			else
				Debug.LogWarningFormat(_invalidDrawerWarning, property.propertyPath);

			return element;
		}
	}
}