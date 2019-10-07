using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class Frame : VisualElement
	{
		private const string Stylesheet = "Frame/Frame.uss";

		public static readonly string UssClassName = "pirho-frame";
		public static readonly string HeaderUssClassName = UssClassName + "__header";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string NoLabelUssClassName = UssClassName + "__label--none";
		public static readonly string ContentUssClassName = UssClassName + "__content";
		public static readonly string FooterUssClassName = UssClassName + "__footer";
		public static readonly string HeaderButtonsUssClassName = UssClassName + "__header-buttons";
		public static readonly string FooterButtonsUssClassName = UssClassName + "__footer-buttons";
		public static readonly string HeaderButtonUssClassName = UssClassName + "__header-button";
		public static readonly string FooterButtonUssClassName = UssClassName + "__footer-button";

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