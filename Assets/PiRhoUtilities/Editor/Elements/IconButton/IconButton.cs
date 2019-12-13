using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class IconButton : Image
	{
		public const string Stylesheet = "IconButton/IconButton.uss";
		public const string UssClassName = "pirho-icon-button";

		private IManipulator _manipulator;

		public IconButton(Texture image, string tooltip, Action action)
		{
			this.image = image;
			this.tooltip = tooltip;
			SetAction(action);

			AddToClassList(UssClassName);
			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
		}

		public void SetAction(Action action)
		{
			if (_manipulator != null)
				this.RemoveManipulator(_manipulator);

			_manipulator = action != null ? new Clickable(action) : null;

			if (_manipulator != null)
				this.AddManipulator(_manipulator);
		}
	}
}
