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

			container.Add(new IconButton(() => Debug.Log("Add Pressed")) { image = Icon.Add.Texture, tooltip = "Add" });
			container.Add(new IconButton(() => Debug.Log("CustomAdd Pressed")) { image = Icon.CustomAdd.Texture, tooltip = "CustomAdd" });
			container.Add(new IconButton(() => Debug.Log("Remove Pressed")) { image = Icon.Remove.Texture, tooltip = "Remove" });
			container.Add(new IconButton(() => Debug.Log("Inspect Pressed")) { image = Icon.Inspect.Texture, tooltip = "Inspect" });
			container.Add(new IconButton(() => Debug.Log("Expanded Pressed")) { image = Icon.Expanded.Texture, tooltip = "Expanded" });
			container.Add(new IconButton(() => Debug.Log("Collapsed Pressed")) { image = Icon.Collapsed.Texture, tooltip = "Collapsed" });
			container.Add(new IconButton(() => Debug.Log("Refresh Pressed")) { image = Icon.Refresh.Texture, tooltip = "Refresh" });
			container.Add(new IconButton(() => Debug.Log("Load Pressed")) { image = Icon.Load.Texture, tooltip = "Load" });
			container.Add(new IconButton(() => Debug.Log("Unload Pressed")) { image = Icon.Unload.Texture, tooltip = "Unload" });
			container.Add(new IconButton(() => Debug.Log("Close Pressed")) { image = Icon.Close.Texture, tooltip = "Close" });
			container.Add(new IconButton(() => Debug.Log("LeftArrow Pressed")) { image = Icon.LeftArrow.Texture, tooltip = "LeftArrow" });
			container.Add(new IconButton(() => Debug.Log("RightArrow Pressed")) { image = Icon.RightArrow.Texture, tooltip = "RightArrow" });
			container.Add(new IconButton(() => Debug.Log("Info Pressed")) { image = Icon.Info.Texture, tooltip = "Info" });
			container.Add(new IconButton(() => Debug.Log("Warning Pressed")) { image = Icon.Warning.Texture, tooltip = "Warning" });
			container.Add(new IconButton(() => Debug.Log("Error Pressed")) { image = Icon.Error.Texture, tooltip = "Error" });
			container.Add(new IconButton(() => Debug.Log("Settings Pressed")) { image = Icon.Settings.Texture, tooltip = "Settings" });
			container.Add(new IconButton(() => Debug.Log("View Pressed")) { image = Icon.View.Texture, tooltip = "View" });
			container.Add(new IconButton(() => Debug.Log("Locked Pressed")) { image = Icon.Locked.Texture, tooltip = "Locked" });
			container.Add(new IconButton(() => Debug.Log("Unlocked Pressed")) { image = Icon.Unlocked.Texture, tooltip = "Unlocked" });

			root.Add(container);
		}
	}
}
