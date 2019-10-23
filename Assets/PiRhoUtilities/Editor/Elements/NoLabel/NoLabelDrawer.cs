using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(NoLabelAttribute))]
	class NoLabelDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var element = this.CreateNextElement(property);

			if (element is PropertyField propertyField)
				propertyField.SetLabel(null);
			else if (element is FieldContainer fieldContainer)
				fieldContainer.SetLabel(null);
			else if (element.GetType().InheritsGeneric(typeof(BaseField<>)))
				BaseFieldExtensions.SetLabel(element, null);

			return element;
		}
	}
}