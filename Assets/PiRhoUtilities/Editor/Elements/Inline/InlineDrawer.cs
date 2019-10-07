using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(InlineAttribute))]
	public class InlineDrawer : PropertyDrawer
	{
		public const string Stylesheet = "Inline/InlineStyle.uss";
		public const string UssClassName = "pirho-inline";
		public const string LabelUssClassName = UssClassName + "__label";
		public const string ChildrenUssClassName = UssClassName + "__children";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var inlineAttribute = attribute as InlineAttribute;

			return CreatePropertyGUI(property, inlineAttribute.ShowMemberLabels);
		}

		protected VisualElement CreatePropertyGUI(SerializedProperty property, bool showMemberLabels)
		{
			var container = new VisualElement();
			var childContainer = new VisualElement();
			childContainer.AddToClassList(ChildrenUssClassName);

			if (!showMemberLabels)
			{
				var label = new FieldContainer(property.displayName, this.GetTooltip());
				label.AddToClassList(LabelUssClassName);
				container.Add(label);
			}

			foreach (var child in property.Children())
			{
				var field = new PropertyField(child);
				if (!showMemberLabels)
					field.SetLabel(null);

				childContainer.Add(field);
			}

			container.Add(childContainer);
			container.AddToClassList(UssClassName);
			container.AddStyleSheet(Configuration.ElementsPath, Stylesheet);

			return container;
		}
	}
}