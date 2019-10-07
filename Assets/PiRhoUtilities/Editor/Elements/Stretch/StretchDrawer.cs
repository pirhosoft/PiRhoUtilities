using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(StretchAttribute))]
	class StretchDrawer : PropertyDrawer
	{
		public const string Stylesheet = "Stretch/StretchStyle.uss";
		public const string UssClassName = "pirho-stretch";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var element = this.CreateNextElement(property);

			element.AddToClassList(UssClassName);
			element.AddStyleSheet(Configuration.ElementsPath, Stylesheet);

			return element;
		}
	}
}