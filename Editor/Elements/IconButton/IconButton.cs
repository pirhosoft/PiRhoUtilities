using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class IconButton : Image
	{
		#region Log Messages

		private const string _missingIconWarning = "(PUEIBMI) unknown icon '{0}' for IconButton: the icon could not be found";

		#endregion 

		#region Class Names

		public const string Stylesheet = "IconButton/IconButton.uss";
		public const string UssClassName = "pirho-icon-button";

		#endregion

		#region Private Members

		private Clickable _clickable;

		#endregion

		#region Public Interface

		public event Action Clicked
		{
			add
			{
				if (_clickable == null)
					_clickable = new Clickable(value);
				else
					_clickable.clicked += value;
			}
			remove
			{
				if (_clickable != null)
					_clickable.clicked -= value;
			}
		}

		public IconButton() : this(null)
		{
		}

		public IconButton(Action clickEvent)
		{
			Clicked += clickEvent;

			AddToClassList(UssClassName);
			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
		}

		#endregion

		#region UXML Support

		private static readonly Dictionary<string, Icon> _icons = new Dictionary<string, Icon>
		{
			{ "Add", Icon.Add },
			{ "CustomAdd", Icon.CustomAdd },
			{ "Remove", Icon.Remove },
			{ "Inspect", Icon.Inspect },
			{ "Expanded", Icon.Expanded },
			{ "Collapsed", Icon.Collapsed },
			{ "Refresh", Icon.Refresh },
			{ "Load", Icon.Load },
			{ "Unload", Icon.Unload },
			{ "Close", Icon.Close },
			{ "LeftArrow", Icon.LeftArrow },
			{ "RightArrow", Icon.RightArrow },
			{ "Info", Icon.Info },
			{ "Warning", Icon.Warning },
			{ "Error", Icon.Error },
			{ "Settings", Icon.Settings },
			{ "View", Icon.View },
			{ "Locked", Icon.Locked },
			{ "Unlocked", Icon.Unlocked }
		};

		public new class UxmlFactory : UxmlFactory<IconButton, UxmlTraits> { }
		public new class UxmlTraits : Image.UxmlTraits
		{
			private readonly UxmlStringAttributeDescription _icon = new UxmlStringAttributeDescription { name = "icon", defaultValue = "Add" };

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);

				var button = ve as IconButton;
				var name = _icon.GetValueFromBag(bag, cc);

				if (_icons.TryGetValue(name, out var icon))
				{
					button.image = icon.Texture;
				}
				else
				{
					Debug.LogWarningFormat(_missingIconWarning, name);
					button.image = Icon.Add.Texture;
				}
			}
		}

		#endregion
	}
}
