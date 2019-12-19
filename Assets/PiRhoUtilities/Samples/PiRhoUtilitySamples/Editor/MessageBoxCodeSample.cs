using PiRhoSoft.Utilities.Editor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Samples
{
	public class MessageBoxCodeSample : CodeSample
	{
		public override void Create(VisualElement root)
		{
			var info = new MessageBox(MessageBoxType.Info, "Here is something that might be useful to know");
			root.Add(info);

			var warning = new MessageBox(MessageBoxType.Warning, "Something happened but everything is still going to work");
			root.Add(warning);

			var error = new MessageBox(MessageBoxType.Error, "Something failed that will probably cause unwanted behavior");
			root.Add(error);
		}
	}
}
