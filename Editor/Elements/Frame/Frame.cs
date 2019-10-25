using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class Frame : VisualElement
	{
		public const string Stylesheet = "Frame/Frame.uss";
		public const string UssClassName = "pirho-frame";
		public const string HeaderUssClassName = UssClassName + "__header";
		public const string LabelUssClassName = UssClassName + "__label";
		public const string NoLabelUssClassName = UssClassName + "__label--none";
		public const string ContentUssClassName = UssClassName + "__content";
		public const string FooterUssClassName = UssClassName + "__footer";
		public const string HeaderButtonsUssClassName = UssClassName + "__header-buttons";
		public const string FooterButtonsUssClassName = UssClassName + "__footer-buttons";
		public const string HeaderButtonUssClassName = UssClassName + "__header-button";
		public const string FooterButtonUssClassName = UssClassName + "__footer-button";

		public VisualElement Header { get; private set; }
		public VisualElement Content { get; private set; }
		public VisualElement Footer { get; private set; }

		public Label Label { get; private set; }
		public VisualElement HeaderButtons { get; private set; }
		public VisualElement FooterButtons { get; private set; }

		public Frame()
		{
			AddToClassList(UssClassName);
			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);

			Setup();
		}

		public void SetLabel(string label, string tooltip = null)
		{
			Label.text = label;
			Label.tooltip = tooltip;
			Label.EnableInClassList(NoLabelUssClassName, string.IsNullOrEmpty(label));
		}

		public IconButton AddHeaderButton(Texture icon, string tooltip, string ussClassName, Action action)
		{
			var button = new IconButton(icon, tooltip, action);
			button.AddToClassList(HeaderButtonUssClassName);

			if (!string.IsNullOrEmpty(ussClassName))
				button.AddToClassList(ussClassName);

			HeaderButtons?.Add(button);
			return button;
		}

		public IconButton AddFooterButton(Texture icon, string tooltip, string ussClassName, Action action)
		{
			var button = new IconButton(icon, tooltip, action);
			button.AddToClassList(FooterButtonUssClassName);

			if (!string.IsNullOrEmpty(ussClassName))
				button.AddToClassList(ussClassName);

			FooterButtons?.Add(button);
			return button;
		}

		private void Setup()
		{
			Header = new VisualElement();
			Header.AddToClassList(HeaderUssClassName);
			Add(Header);

			Label = new Label();
			Label.AddToClassList(LabelUssClassName);
			Header.Add(Label);

			HeaderButtons = new VisualElement();
			HeaderButtons.AddToClassList(HeaderButtonsUssClassName);
			Header.Add(HeaderButtons);

			Content = new VisualElement();
			Content.AddToClassList(ContentUssClassName);
			Add(Content);

			Footer = new VisualElement();
			Footer.AddToClassList(FooterUssClassName);
			Add(Footer);

			FooterButtons = new VisualElement();
			FooterButtons.AddToClassList(FooterButtonsUssClassName);
			Footer.Add(FooterButtons);
		}
	}
}