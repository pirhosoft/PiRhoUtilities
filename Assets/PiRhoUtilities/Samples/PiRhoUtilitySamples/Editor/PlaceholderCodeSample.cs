using PiRhoSoft.Utilities.Editor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Samples
{
	public class PlaceholderCodeSample : CodeSample
	{
		public override void Create(VisualElement root)
		{
			var text = new TextField("Distance");
			var placeholder = new Placeholder("Kilometers");
			placeholder.AddToField(text);
			root.Add(text);
		}
	}
}
