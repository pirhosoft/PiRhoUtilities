using PiRhoSoft.Utilities.Editor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Samples
{
	public class ComboBoxCodeSample : CodeSample
	{
		public override void Create(VisualElement root)
		{
			var comboBox = new ComboBoxField("Ice Cream");

			comboBox.IsDelayed = true;
			comboBox.Options = new List<string>
			{
				"Chocolate",
				"Vanilla",
				"Rocky Road",
				"Cookies and Cream",
				"Mint Chocolate Chip",
				"Moose Tracks"
			};

			comboBox.RegisterValueChangedCallback(evt => Debug.Log($"Selected {evt.newValue}"));
			root.Add(comboBox);
		}
	}
}
