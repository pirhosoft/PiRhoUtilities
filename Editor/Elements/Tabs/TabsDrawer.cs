using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(TabsAttribute))]
	class TabsDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var tabsAttribute = attribute as TabsAttribute;
			var parent = property.GetParent();

			Tabs tabs = null;
			
			foreach (var sibling in parent.Children())
			{
				var field = fieldInfo.DeclaringType.GetField(sibling.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

				if (field != null && field.TryGetAttribute<TabsAttribute>(out var siblingAttribute) && siblingAttribute.Group == tabsAttribute.Group)
				{
					if (tabs != null)
					{
						var page = tabs.GetPage(siblingAttribute.Tab);

						if (page == null)
						{
							page = new TabPage(siblingAttribute.Tab);
							tabs.Add(page);
						}

						var element = PropertyDrawerExtensions.CreateNextElement(field, siblingAttribute, sibling);
						page.Add(element);
					}
					else if (SerializedProperty.EqualContents(property, sibling))
					{
						// this property is first and is responsible for drawing
						tabs = new Tabs();

						var page = new TabPage(siblingAttribute.Tab);
						var element = this.CreateNextElement(sibling);
						page.Add(element);
						tabs.Add(page);
					}
					else
					{
						// a different property was first and handled the drawing
						break;
					}
				}
			}

			return tabs ?? new VisualElement();
		}
	}
}