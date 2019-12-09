using UnityEngine;

namespace PiRhoSoft.Utilities.Editor
{
	public class RolloutControl : Frame
	{
		public new const string Stylesheet = "Rollout/RolloutStyle.uss";
		public new const string UssClassName = "pirho-rollout";
		public const string ExpandedUssClassName = UssClassName + "--expanded";
		public const string CollapsedUssClassName = UssClassName + "--collapsed";
		public const string IconUssClassName = UssClassName + "__icon";

		public static readonly Icon ExpandedIcon = Icon.Expanded;
		public static readonly Icon CollapsedIcon = Icon.Collapsed;

		private Texture _iconTexture => IsExpanded ? ExpandedIcon.Texture : CollapsedIcon.Texture;
		private string _tooltip => IsExpanded ? "Collapse this view" : "Expand this view";

		private bool _isExpanded;
		private IconButton _icon;

		public bool IsExpanded
		{
			get { return _isExpanded; }
			set
			{
				var previous = _isExpanded;

				if (previous != value)
				{
					SetValueWithoutNotify(value);
					this.SendChangeEvent(previous, value);
				}
			}
		}

		public RolloutControl(bool isExpanded) : base()
		{
			_isExpanded = isExpanded;

			Setup();

			AddToClassList(UssClassName);
			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
		}

		public void SetValueWithoutNotify(bool isExpanded)
		{
			if (_isExpanded != isExpanded)
			{
				_isExpanded = isExpanded;
				Refresh();
			}
		}

		private void Setup()
		{
			_icon = new IconButton(_iconTexture, _tooltip, () => IsExpanded = !IsExpanded);
			_icon.AddToClassList(IconUssClassName);

			Header.Insert(0, _icon);

			Refresh();
		}

		private void Refresh()
		{
			_icon.image = _iconTexture;
			_icon.tooltip = _tooltip;

			EnableInClassList(ExpandedUssClassName, IsExpanded);
			EnableInClassList(CollapsedUssClassName, !IsExpanded);
		}
	}
}