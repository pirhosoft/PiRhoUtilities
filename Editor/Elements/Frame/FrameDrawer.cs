using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(FrameAttribute))]
	class FrameDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var attribute = this.attribute as FrameAttribute;
			var frame = new Frame();

			frame.IsCollapsable = attribute.IsCollapsable;
			frame.bindingPath = property.propertyPath;
			// TODO: other stuff from ConfigureField

			return frame;
		}
	}
}
