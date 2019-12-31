using PiRhoSoft.Utilities.Editor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Samples
{
	public class FrameCodeSample : CodeSample
	{
		public override void Create(VisualElement root)
		{
			var frame = new Frame();
			frame.IsCollapsable = true;
			frame.IsCollapsed = false;
			frame.Label = "Sample Frame";
			frame.Tooltip = "This is an example of a standalone frame";
			frame.AddHeaderButton(Icon.Info.Texture, "Header Button", null, () => Debug.Log("Header button clicked"));
			root.Add(frame);

			frame.Add(new IntegerField("Child Int"));
			frame.Add(new Slider("Child Slider"));
			frame.Add(new Toggle("Child Toggle"));

			var isCollapsableToggle = new Toggle(nameof(Frame.IsCollapsable));
			isCollapsableToggle.value = frame.IsCollapsable;
			isCollapsableToggle.RegisterValueChangedCallback(e => frame.IsCollapsable = e.newValue);
			root.Add(isCollapsableToggle);

			var isCollapsedToggle = new Toggle(nameof(Frame.IsCollapsed));
			isCollapsedToggle.value = frame.IsCollapsed;
			isCollapsedToggle.RegisterValueChangedCallback(e => frame.IsCollapsed = e.newValue);
			root.Add(isCollapsedToggle);

			var labelText = new TextField(nameof(Frame.Label));
			labelText.value = frame.Label;
			labelText.RegisterValueChangedCallback(e => frame.Label = e.newValue);
			root.Add(labelText);

			var tooltipText = new TextField(nameof(Frame.Tooltip));
			tooltipText.value = frame.Tooltip;
			tooltipText.RegisterValueChangedCallback(e => frame.Tooltip = e.newValue);
			root.Add(tooltipText);
		}
	}
}
