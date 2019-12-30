using PiRhoSoft.Utilities.Editor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Samples
{
	public class PopupCodeSample : CodeSample
	{
		public override void Create(VisualElement root)
		{
			var stringPopup = new PopupStringField("Constant Symbols");
			stringPopup.value = "\u03D5";
			stringPopup.RegisterValueChangedCallback(evt => Debug.Log($"Selected {evt.newValue}"));
			stringPopup.SetValues(
				new List<string> { "\u03C0", "\u03D5", "e" },
				new List<string> { "Pi", "Phi", "Euler" });
			root.Add(stringPopup);

			var floatPopup = new PopupFloatField("Constant Values");
			floatPopup.value = 1.61803f;
			floatPopup.RegisterValueChangedCallback(evt => Debug.Log($"Selected {evt.newValue}"));
			floatPopup.SetValues(
				new List<float> { 3.14159f, 1.61803f, 2.71828f },
				new List<string> { "Pi", "Phi", "Euler" });
			root.Add(floatPopup);
		}
	}
}
