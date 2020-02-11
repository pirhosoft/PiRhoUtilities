using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class InlineField : VisualElement
	{
		public const string Stylesheet = "InlineStyle.uss";
		public const string UssClassName = "pirho-inline";
		public const string LabelUssClassName = UssClassName + "__label";
		public const string ChildrenUssClassName = UssClassName + "__children";

		public InlineField(SerializedProperty property, bool showMemberLabels)
		{
			var childContainer = new VisualElement();
			childContainer.AddToClassList(ChildrenUssClassName);

			if (!showMemberLabels)
			{
				var label = new FieldContainer(property.displayName);
				label.AddToClassList(LabelUssClassName);
				Add(label);
			}

			foreach (var child in property.Children())
			{
				var field = new PropertyField(child);
				if (!showMemberLabels)
					field.SetFieldLabel(null);

				childContainer.Add(field);
			}

			Add(childContainer);
			AddToClassList(UssClassName);
			this.AddStyleSheet(Stylesheet);
		}
	}
}
