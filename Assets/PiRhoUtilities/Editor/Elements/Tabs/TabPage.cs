using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class TabPage : VisualElement
	{
		#region Members

		private string _label;
		private string _tooltip;
		private bool _isActive;
		private Tabs _tabs;
		private Button _button;

		#endregion

		#region Public Interface

		public string Label { get => _label; set => SetLabel(value); }
		public string Tooltip { get => _tooltip; set => SetTooltip(value); }
		public bool IsActive { get => _isActive; internal set => _isActive = value; }

		public Tabs Tabs => _tabs;
		public Button Button => _button;

		public TabPage() : this(null)
		{
		}

		public TabPage(string label)
		{
			BuildUi();
			SetLabel(label);
		}

		public void Activate()
		{
			var current = _tabs.ActivePage;

			if (current != this)
			{
				if (current != null)
					current._isActive = false;

				_isActive = true;
				_tabs?.UpdateTabs();
			}
		}

		#endregion

		#region Ui

		private void BuildUi()
		{
			AddToClassList(Tabs.PageUssClassName);

			_button = new Button(Activate);
			_button.AddToClassList(Tabs.TabUssClassName);

			RegisterCallback<AttachToPanelEvent>(OnAttached);
			RegisterCallback<DetachFromPanelEvent>(OnDetached);
		}

		private void SetLabel(string value)
		{
			_label = value;
			_button.text = value;
		}

		private void SetTooltip(string value)
		{
			_tooltip = value;
			_button.tooltip = value;
		}

		private void OnAttached(AttachToPanelEvent evt)
		{
			_tabs = parent as Tabs;
			_tabs?.UpdateTabs();
		}

		private void OnDetached(DetachFromPanelEvent evt)
		{
			_tabs?.UpdateTabs();
			_tabs = null;
		}

		#endregion

		#region Uxml

		public new class UxmlFactory : UxmlFactory<TabPage, UxmlTraits> { }
		public new class UxmlTraits : VisualElement.UxmlTraits
		{
			private readonly UxmlStringAttributeDescription _label = new UxmlStringAttributeDescription { name = "label" };
			private readonly UxmlStringAttributeDescription _tooltip = new UxmlStringAttributeDescription { name = "tooltip" };
			private readonly UxmlBoolAttributeDescription _isActive = new UxmlBoolAttributeDescription { name = "is-active" };

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);

				var page = ve as TabPage;
				page.Label = _label.GetValueFromBag(bag, cc);
				page.Tooltip = _tooltip.GetValueFromBag(bag, cc);
				page.IsActive = _isActive.GetValueFromBag(bag, cc);
			}
		}

		#endregion
	}
}
