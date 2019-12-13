using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(ReferenceAttribute))]
	class ReferenceDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var type = this.GetFieldType();
			var next = this.GetNextDrawer();
			var drawer = new PropertyReferenceDrawer(property, next);
			var field = new ReferenceField(type, drawer);

			field.bindingPath = property.propertyPath; // TODO: other stuff from ConfigureField

			return field;
		}
	}
}
