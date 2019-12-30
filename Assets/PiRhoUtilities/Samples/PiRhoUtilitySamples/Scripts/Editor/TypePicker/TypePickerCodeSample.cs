using PiRhoSoft.Utilities.Editor;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Samples
{
	public class TypePickerCodeSample : CodeSample
	{
		public override void Create(VisualElement root)
		{
			var picker = new TypePickerField("Behaviour Type", typeof(MonoBehaviour), false);
			picker.RegisterValueChangedCallback(evt => { var type = string.IsNullOrEmpty(evt.newValue) ? null : Type.GetType(evt.newValue); Debug.Log($"Selected type {type?.Name ?? "none"}"); });
			root.Add(picker);
		}
	}
}
