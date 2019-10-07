using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class IconButton : Image
	{
		public const string Stylesheet = "IconButton/IconButton.uss";
		public const string UssClassName = "pirho-icon-button";

		public IconButton(Texture image, string tooltip, Action action)
		{
			this.image = image;
			this.tooltip = tooltip;
			this.AddManipulator(new Clickable(action));

			AddToClassList(UssClassName);
			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
		}
	}
}