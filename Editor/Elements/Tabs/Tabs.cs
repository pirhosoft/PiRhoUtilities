using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class Tabs : VisualElement
	{
		#region Class Names

		public const string Stylesheet = "Tabs/Tabs.uss";
		public const string UssClassName = "pirho-tabs";
		public const string HeaderUssClassName = UssClassName + "__header";
		public const string ContentUssClassName = UssClassName + "__content";
		public const string TabUssClassName = UssClassName + "__tab";
		public const string TabSelectedUssClassName = TabUssClassName + "--selected";
		public const string PageUssClassName = UssClassName + "__page";
		public const string PageSelectedUssClassName = PageUssClassName + "--selected";

		#endregion

		#region Members

		private TabPage _activePage;

		#endregion

		#region Public Interface

		public VisualElement Header { get; private set; }
		public VisualElement Content { get; private set; }
		public IEnumerable<TabPage> Pages => Content.Children().Cast<TabPage>().Where(page => page != null);
		public TabPage ActivePage => _activePage;

		public override VisualElement contentContainer => Content;

		public Tabs()
		{
			Header = new VisualElement();
			Header.AddToClassList(HeaderUssClassName);

			Content = new VisualElement();
			Content.AddToClassList(ContentUssClassName);

			hierarchy.Add(Header);
			hierarchy.Add(Content);

			AddToClassList(UssClassName);
			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
		}

		public void UpdateTabs()
		{
			var pages = Pages;

			_activePage = null;
			Header.Clear();

			foreach (var page in pages)
			{
				Header.Add(page.Button);

				if (page.IsActive && _activePage == null)
					_activePage = page;
			}

			if (_activePage == null && Content.childCount > 0)
				_activePage = Content[0] as TabPage;

			foreach (var page in pages)
			{
				page.IsActive = page == _activePage;
				page.EnableInClassList(PageSelectedUssClassName, page.IsActive);
				page.Button.EnableInClassList(TabSelectedUssClassName, page.IsActive);
			}
		}

		#endregion

		#region UXML

		public new class UxmlFactory : UxmlFactory<Tabs, UxmlTraits> { }

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get
				{
					yield return new UxmlChildElementDescription(typeof(TabPage));
				}
			}
		}

		#endregion
	}
}
