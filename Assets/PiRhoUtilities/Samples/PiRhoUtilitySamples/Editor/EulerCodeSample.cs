using PiRhoSoft.Utilities.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Samples
{
	public class EulerCodeSample : CodeSample
	{
		public override void Create(VisualElement root)
		{
			var field = new EulerField("Euler Angles");
			field.value = Quaternion.identity;
			field.RegisterValueChangedCallback(e => Debug.Log($"Quaternion is {e.newValue}"));
			root.Add(field);
		}
	}
}
