using System.Collections.Generic;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class Tabs : VisualElement
	{
		public const string Stylesheet = "Tabs/Tabs.uss";
		public const string UssClassName = "pirho-tabs";
		public const string HeaderUssClassName = UssClassName + "__header";
		public const string ContentUssClassName = UssClassName + "__content";
		public const string TabUssClassName = UssClassName + "__tab";
		public const string TabSelectedUssClassName = TabUssClassName + "--selected";
		public const string TabContentUssClassName = TabUssClassName + "__content";
		public const string TabContentSelectedUssClassName = TabContentUssClassName + "--selected";

		public VisualElement Header { get; private set; }
		public VisualElement Content { get; private set; }

		private readonly Dictionary<string, Tab> _tabs = new Dictionary<string, Tab>();
		private Tab _currentTab;

		public Tabs()
		{
			Header = new VisualElement();
			Header.AddToClassList(HeaderUssClassName);

			Content = new VisualElement();
			Content.AddToClassList(ContentUssClassName);

			Add(Header);
			Add(Content);

			AddToClassList(UssClassName);
			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
		}

		public void AddElement(string tabName, VisualElement element)
		{
			if (_tabs.TryGetValue(tabName, out var tab))
			{
				tab.Content.Add(element);
			}
			else
			{
				AddTab(tabName);
				AddElement(tabName, element);
			}
		}

		private void AddTab(string tabName)
		{
			var tab = new Tab() { text = tabName };
			tab.clicked += () =>
			{
				if (_currentTab != tab)
					SelectTab(tab);
			};

			if (_tabs.Count == 0)
				SelectTab(tab);

			_tabs.Add(tabName, tab);

			Header.Add(tab);
			Content.Add(tab.Content);
		}

		private void SelectTab(Tab tab)
		{
			_currentTab?.RemoveFromClassList(TabSelectedUssClassName);
			_currentTab?.Content.RemoveFromClassList(TabContentSelectedUssClassName);
			_currentTab = tab;
			_currentTab.AddToClassList(TabSelectedUssClassName);
			_currentTab.Content.AddToClassList(TabContentSelectedUssClassName);
		}

		private class Tab : Button
		{
			public VisualElement Content { get; private set; }

			public Tab()
			{
				AddToClassList(TabUssClassName);

				Content = new VisualElement();
				Content.AddToClassList(TabContentUssClassName);
			}
		}
	}
}
