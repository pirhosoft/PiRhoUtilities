using UnityEngine.UIElements;

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

		private bool _isExpanded;
		private Image _icon;

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

			AddToClassList(UssClassName);
			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);

			Setup();
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
			_icon = new Image();
			_icon.AddToClassList(IconUssClassName);
			_icon.style.width = ExpandedIcon.Texture.width;
			_icon.style.height = ExpandedIcon.Texture.height;
			_icon.AddManipulator(new Clickable(() => IsExpanded = !IsExpanded));

			Header.Add(_icon);
			_icon.PlaceBehind(Label);

			Refresh();
		}

		private void Refresh()
		{
			_icon.image = IsExpanded ? ExpandedIcon.Texture : CollapsedIcon.Texture;

			EnableInClassList(ExpandedUssClassName, IsExpanded);
			EnableInClassList(CollapsedUssClassName, !IsExpanded);
		}
	}
}