using PiRhoSoft.Utilities.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Samples
{
	public class ObjectPickerCodeSample : CodeSample
	{
		public override void Create(VisualElement root)
		{
			var picker = new ObjectPickerField("Uxml", typeof(VisualTreeAsset));
			picker.RegisterValueChangedCallback(e => Debug.Log($"Selected {e.newValue.name}"));
			root.Add(picker);
		}
	}
}
