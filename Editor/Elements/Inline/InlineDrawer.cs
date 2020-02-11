using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(InlineAttribute))]
	public class InlineDrawer : PropertyDrawer
	{
		public const string Stylesheet = "InlineStyle.uss";
		public const string UssClassName = "pirho-inline";
		public const string LabelUssClassName = UssClassName + "__label";
		public const string ChildrenUssClassName = UssClassName + "__children";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var showMemberLabels = (attribute as InlineAttribute).ShowMemberLabels;
			return new InlineField(property, showMemberLabels);
		}
	}
}