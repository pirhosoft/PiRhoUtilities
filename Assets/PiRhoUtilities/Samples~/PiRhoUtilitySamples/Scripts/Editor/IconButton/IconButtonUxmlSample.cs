using PiRhoSoft.Utilities.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Samples
{
	public class IconButtonUxmlSample : UxmlSample
	{
		public override void Setup(VisualElement root)
		{
			root.Query<IconButton>().ForEach(button =>
			{
				button.Clicked += () => Debug.Log($"{button.image.name} Pressed");
			});
		}
	}
}
