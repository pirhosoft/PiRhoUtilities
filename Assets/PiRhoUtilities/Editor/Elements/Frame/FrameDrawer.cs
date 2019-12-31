using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(FrameAttribute))]
	class FrameDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var frameAttribute = attribute as FrameAttribute;
			var frame = new Frame
			{
				IsCollapsable = frameAttribute.IsCollapsable,
				bindingPath = property.propertyPath
			};

			// TODO: other stuff from ConfigureField

			return frame;
		}
	}
}
