using PiRhoSoft.Utilities.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Samples
{
	public class IconButtonCodeSample : CodeSample
	{
		public override void Create(VisualElement root)
		{
			var container = new VisualElement();
			container.style.flexDirection = FlexDirection.Row;
			container.style.alignItems = Align.Center;

			container.Add(new IconButton(Icon.Add.Texture, "Add", () => Debug.Log("Add Pressed")));
			container.Add(new IconButton(Icon.CustomAdd.Texture, "CustomAdd", () => Debug.Log("CustomAdd Pressed")));
			container.Add(new IconButton(Icon.Remove.Texture, "Remove", () => Debug.Log("Remove Pressed")));
			container.Add(new IconButton(Icon.Inspect.Texture, "Inspect", () => Debug.Log("Inspect Pressed")));
			container.Add(new IconButton(Icon.Expanded.Texture, "Expanded", () => Debug.Log("Expanded Pressed")));
			container.Add(new IconButton(Icon.Collapsed.Texture, "Collapsed", () => Debug.Log("Collapsed Pressed")));
			container.Add(new IconButton(Icon.Refresh.Texture, "Refresh", () => Debug.Log("Refresh Pressed")));
			container.Add(new IconButton(Icon.Load.Texture, "Load", () => Debug.Log("Load Pressed")));
			container.Add(new IconButton(Icon.Unload.Texture, "Unload", () => Debug.Log("Unload Pressed")));
			container.Add(new IconButton(Icon.Close.Texture, "Close", () => Debug.Log("Close Pressed")));
			container.Add(new IconButton(Icon.LeftArrow.Texture, "LeftArrow", () => Debug.Log("LeftArrow Pressed")));
			container.Add(new IconButton(Icon.RightArrow.Texture, "RightArrow", () => Debug.Log("RightArrow Pressed")));
			container.Add(new IconButton(Icon.Info.Texture, "Info", () => Debug.Log("Info Pressed")));
			container.Add(new IconButton(Icon.Warning.Texture, "Warning", () => Debug.Log("Warning Pressed")));
			container.Add(new IconButton(Icon.Error.Texture, "Error", () => Debug.Log("Error Pressed")));
			container.Add(new IconButton(Icon.Settings.Texture, "Settings", () => Debug.Log("Settings Pressed")));
			container.Add(new IconButton(Icon.View.Texture, "View", () => Debug.Log("View Pressed")));
			container.Add(new IconButton(Icon.Locked.Texture, "Locked", () => Debug.Log("Locked Pressed")));
			container.Add(new IconButton(Icon.Unlocked.Texture, "Unlocked", () => Debug.Log("Unlocked Pressed")));

			root.Add(container);
		}
	}
}
