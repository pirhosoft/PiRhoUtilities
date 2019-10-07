using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(GroupAttribute))]
	class GroupDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var group = attribute as GroupAttribute;
			var parent = property.GetParent();

			Frame frame = null;
			
			foreach (var sibling in parent.Children())
			{
				var field = fieldInfo.DeclaringType.GetField(sibling.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

				if (field != null && field.TryGetAttribute<GroupAttribute>(out var groupAttribute) && groupAttribute.Name == group.Name)
				{
					if (frame != null)
					{
						var element = PropertyDrawerExtensions.CreateNextElement(field, groupAttribute, sibling);
						frame.Content.Add(element);
					}
					else if (SerializedProperty.EqualContents(property, sibling))
					{
						// this property is first and is responsible for drawing
						frame = group.Style == GroupStyle.Frame ? new Frame() : new RolloutControl(true);
						frame.Label.text = group.Name;

						var element = this.CreateNextElement(sibling);
						frame.Content.Add(element);
					}
					else
					{
						// a different property was first and handled the drawing
						break;
					}
				}
			}

			return frame ?? new VisualElement();
		}
	}
}