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
		public const string HeaderButtonsUssClassName = UssClassName + "__header-buttons";
		public const string HeaderButtonUssClassName = UssClassName + "__header-button";
		public const string CollapseButtonUssClassName = UssClassName + "__collapse-button";
		public const string CollapsableUssClassName = UssClassName + "--collapsable";
		public const string ExpandedUssClassName = UssClassName + "--expanded";
		public const string CollapsedUssClassName = UssClassName + "--collapsed";

		#endregion

		#region Defaults

		public const string CollapseTooltip = "Collapse this view";
		public const string ExpandTooltip = "Expand this view";

		public const bool DefaultIsCollapsable = true;
		public const bool DefaultIsCollapsed = false;

		#endregion

		#region Icons

		public static readonly Icon ExpandIcon = Icon.Collapsed;
		public static readonly Icon CollapseIcon = Icon.Expanded;

		#endregion

		#region Private Members

		private Label _labelElement;
		private IconButton _collapseButton;

		private readonly bool _addChildren = true;

		private bool _isCollapsable = DefaultIsCollapsable;
		private bool _isCollapsed = DefaultIsCollapsed;
		private string _label = null;
		private string _tooltip = null;

		#endregion

		#region Public Interface

		public VisualElement Header { get; private set; }
		public VisualElement Content { get; private set; }

		public VisualElement HeaderButtons { get; private set; }

		public Frame()
		{
			BuildUi();
		}

		protected Frame(bool addChildren) : this()
		{
			_addChildren = addChildren;
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
			var button = new IconButton(action) { image = icon, tooltip = tooltip };
			button.AddToClassList(HeaderButtonUssClassName);

			if (!string.IsNullOrEmpty(ussClassName))
				button.AddToClassList(ussClassName);

			HeaderButtons.Add(button);
			return button;
		}

		#endregion

		#region State Setters

		private void SetCollapsable(bool isCollapsable)
		{
			if (_isCollapsable != isCollapsable)
			{
				_isCollapsable = isCollapsable;
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

		#endregion

		#region UI

		public override VisualElement contentContainer => Content;

		private void BuildUi()
		{
			AddToClassList(UssClassName);
			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);

			Header = new VisualElement();
			Header.AddToClassList(HeaderUssClassName);
			hierarchy.Add(Header);

			_collapseButton = new IconButton(() => IsCollapsed = !IsCollapsed) { image = CollapseIcon.Texture, tooltip = CollapseTooltip };
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
			hierarchy.Add(Content);

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
				// TODO: Support binding expanded state directly to a bool?

				BindingExtensions.CreateBind(this, property, GetExpandedProperty, SetExpandedProperty, CompareExpandedProperties);
				Label =_label ?? property.displayName;
				Tooltip = _tooltip ?? property.GetTooltip();

				if (_addChildren && property.HasVisibleChildFields())
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

		private static bool GetExpandedProperty(SerializedProperty property)
		{
			return property.isExpanded;
		}

		private static void SetExpandedProperty(SerializedProperty property, bool value)
		{
			property.isExpanded = value;
		}

		private static bool CompareExpandedProperties(bool value, SerializedProperty property, Func<SerializedProperty, bool> getter)
		{
			var currentValue = getter(property);
			return value == currentValue;
		}

		#endregion

		#region UXML

		public new class UxmlFactory : UxmlFactory<Frame, UxmlTraits> { }
		public new class UxmlTraits : BindableElement.UxmlTraits
		{
			private readonly UxmlStringAttributeDescription _label = new UxmlStringAttributeDescription { name = "label" };
			private readonly UxmlStringAttributeDescription _tooltip = new UxmlStringAttributeDescription { name = "tooltip" };
			private readonly UxmlBoolAttributeDescription _collapsable = new UxmlBoolAttributeDescription { name = "is-collapsable", defaultValue = DefaultIsCollapsable };
			private readonly UxmlBoolAttributeDescription _collapsed = new UxmlBoolAttributeDescription { name = "is-collapsed", defaultValue = DefaultIsCollapsed };

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);

				var frame = ve as Frame;
				frame.Label = _label.GetValueFromBag(bag, cc);
				frame.Tooltip = _tooltip.GetValueFromBag(bag, cc);
				frame.IsCollapsable = _collapsable.GetValueFromBag(bag, cc);
				frame.IsCollapsed = _collapsed.GetValueFromBag(bag, cc);

				// TODO: Figure out how to support header buttons
			}
		}

		#endregion
	}
}
