using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class Frame : BindableElement, INotifyValueChanged<bool>
	{
		#region Class Names

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
		public const string CollapseButtonUssClassName = UssClassName + "__collapse-button";
		public const string CollapsableUssClassName = UssClassName + "--collapsable";
		public const string ExpandedUssClassName = UssClassName + "--expanded";
		public const string CollapsedUssClassName = UssClassName + "--collapsed";

		#endregion

		#region Strings

		private const string CollapseTooltip = "Collapse this view";
		private const string ExpandTooltip = "Expand this view";

		#endregion

		#region Icons

		public static readonly Icon ExpandIcon = Icon.Collapsed;
		public static readonly Icon CollapseIcon = Icon.Expanded;

		#endregion

		public VisualElement Header { get; private set; }
		public VisualElement Content { get; private set; }
		public VisualElement Footer { get; private set; }

		public VisualElement HeaderButtons { get; private set; }
		public VisualElement FooterButtons { get; private set; }

		private Label _labelElement;
		private IconButton _collapseButton;

		private bool _isCollapsable = true;
		private bool _isCollapsed = false;
		private string _label = null;
		private string _tooltip = null;

		public Frame()
		{
			BuildUi();
		}

		public bool IsCollapsable
		{
			get => _isCollapsable;
			set => SetCollapsable(value);
		}

		public bool IsCollapsed
		{
			get => _isCollapsed;
			set => SetCollapsed(value);
		}

		public string Label
		{
			get => _label;
			set => SetLabel(value);
		}

		public string Tooltip
		{
			get => _tooltip;
			set => SetTooltip(value);
		}

		public IconButton AddHeaderButton(Texture icon, string tooltip, string ussClassName, Action action)
		{
			var button = new IconButton(icon, tooltip, action);
			button.AddToClassList(HeaderButtonUssClassName);

			if (!string.IsNullOrEmpty(ussClassName))
				button.AddToClassList(ussClassName);

			HeaderButtons.Add(button);
			return button;
		}

		public IconButton AddFooterButton(Texture icon, string tooltip, string ussClassName, Action action)
		{
			var button = new IconButton(icon, tooltip, action);
			button.AddToClassList(FooterButtonUssClassName);

			if (!string.IsNullOrEmpty(ussClassName))
				button.AddToClassList(ussClassName);

			FooterButtons.Add(button);
			return button;
		}

		private void SetCollapsable(bool isCollapsable)
		{
			if (_isCollapsable != isCollapsable)
			{
				_isCollapsable = isCollapsable;

				if (!isCollapsable && _isCollapsed)
					IsCollapsed = false;
				else
					UpdateCollapse();
			}
		}

		private void SetCollapsed(bool isCollapsed)
		{
			var previous = _isCollapsed;

			if (isCollapsed != previous)
			{
				SetCollapsedWithoutNotify(isCollapsed);
				this.SendChangeEvent(previous, isCollapsed);
			}
		}

		private void SetCollapsedWithoutNotify(bool isCollapsed)
		{
			_isCollapsed = isCollapsed;
			UpdateCollapse();
		}

		private void SetLabel(string label)
		{
			_label = label;
			UpdateLabel();
		}

		private void SetTooltip(string tooltip)
		{
			_tooltip = tooltip;
			UpdateLabel();
		}

		#region UI

		private void BuildUi()
		{
			AddToClassList(UssClassName);
			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);

			Header = new VisualElement();
			Header.AddToClassList(HeaderUssClassName);
			Add(Header);

			_collapseButton = new IconButton(CollapseIcon.Texture, CollapseTooltip, () => IsCollapsed = !IsCollapsed);
			_collapseButton.AddToClassList(CollapseButtonUssClassName);
			Header.Add(_collapseButton);

			_labelElement = new Label();
			_labelElement.AddToClassList(LabelUssClassName);
			Header.Add(_labelElement);

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

			UpdateCollapse();
			UpdateLabel();
		}

		private void UpdateCollapse()
		{
			EnableInClassList(CollapsableUssClassName, _isCollapsable);
			EnableInClassList(ExpandedUssClassName, !_isCollapsed);
			EnableInClassList(CollapsedUssClassName, _isCollapsed);

			_collapseButton.image = _isCollapsed ? ExpandIcon.Texture : CollapseIcon.Texture;
			_collapseButton.tooltip = _isCollapsed ? ExpandTooltip : CollapseTooltip;
		}

		private void UpdateLabel()
		{
			_labelElement.EnableInClassList(NoLabelUssClassName, string.IsNullOrEmpty(_label));
			_labelElement.text = _label;
			_labelElement.tooltip = _tooltip;
		}

		#endregion

		#region Binding

		bool INotifyValueChanged<bool>.value
		{
			get => !IsCollapsed;
			set => IsCollapsed = !value;
		}

		void INotifyValueChanged<bool>.SetValueWithoutNotify(bool newValue)
		{
			SetCollapsedWithoutNotify(!newValue);
		}

		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);

			if (this.TryGetPropertyBindEvent(evt, out var property))
			{
				BindingExtensions.CreateBind(this, property, GetExpandedProperty, SetExpandedProperty, CompareExpandedProperties);
				Label =_label ?? property.displayName;
				Tooltip = _tooltip ?? property.GetTooltip();

				if (property.HasVisibleChildFields())
				{
					Content.Clear();

					foreach (var child in property.Children())
					{
						var field = new PropertyField(child);
						Content.Add(field);
					}
				}

				evt.StopPropagation();
			}
		}

		private bool GetExpandedProperty(SerializedProperty property)
		{
			return property.isExpanded;
		}

		private void SetExpandedProperty(SerializedProperty property, bool value)
		{
			property.isExpanded = value;
		}

		private bool CompareExpandedProperties(bool value, SerializedProperty property, Func<SerializedProperty, bool> getter)
		{
			var currentValue = getter(property);
			return value == currentValue;
		}

		#endregion

		#region UXML

		public new class UxmlFactory : UxmlFactory<Frame, UxmlTraits> { }

		public new class UxmlTraits : BindableElement.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);

				var frame = ve as Frame;

				// label, tooltip, iscollapsable, iscollapsed, header buttons, footer buttons, content
			}
		}

		#endregion
	}
}
