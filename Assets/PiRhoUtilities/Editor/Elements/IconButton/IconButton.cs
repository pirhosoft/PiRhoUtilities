using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class IconButton : Image
	{
		#region Class Names

		public const string Stylesheet = "IconButton/IconButton.uss";
		public const string UssClassName = "pirho-icon-button";

		#endregion

		#region

		private IManipulator _manipulator;

		#endregion

		#region Public Interface

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

		#endregion

		#region UXML Support

		public IconButton() : this(null, null, null) { }

		public new class UxmlFactory : UxmlFactory<IconButton, UxmlTraits> { }
		public new class UxmlTraits : Image.UxmlTraits { }

		#endregion
	}
}
