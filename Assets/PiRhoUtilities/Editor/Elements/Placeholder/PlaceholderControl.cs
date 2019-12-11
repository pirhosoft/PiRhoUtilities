using System.Reflection;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class PlaceholderControl : Label
	{
		public const string Stylesheet = "Placeholder/PlaceholderStyle.uss";
		public const string UssClassName = "pirho-placeholder";

		private static readonly PropertyInfo _textInputProperty;
		private static readonly PropertyInfo _textProperty;

		static PlaceholderControl()
		{
			_textInputProperty = typeof(TextField).GetProperty("textInput", BindingFlags.NonPublic | BindingFlags.Instance);
			_textProperty = _textInputProperty?.PropertyType?.GetProperty("text", BindingFlags.Public | BindingFlags.Instance);
		}

		public PlaceholderControl(string text)
		{
			this.text = text;
			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
			AddToClassList(UssClassName);

			pickingMode = PickingMode.Ignore;
		}

		public void AddToField(TextField textField)
		{
			// Add this specifically to the input field in case the TextField has a label
			var input = textField.Q(className: TextField.inputUssClassName);
			input.Add(this);

			UpdateDisplayed(textField);

			textField.RegisterCallback<KeyDownEvent>(evt => UpdateDisplayed(textField));
		}

		private void UpdateDisplayed(TextField field)
		{
			// Execute a frame later because text won't be updated yet
			schedule.Execute(() =>
			{
				var textInput = _textInputProperty.GetValue(field);
				var text = _textProperty.GetValue(textInput) as string;

				this.SetDisplayed(string.IsNullOrEmpty(text));
			}).StartingIn(0);
		}
	}
}