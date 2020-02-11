using System.Reflection;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class Placeholder : Label
	{
		#region Class Names

		public const string Stylesheet = "PlaceholderStyle.uss";
		public const string UssClassName = "pirho-placeholder";

		#endregion

		#region Cached Reflection Info

		private static readonly PropertyInfo _textInputProperty;
		private static readonly PropertyInfo _textProperty;

		static Placeholder()
		{
			_textInputProperty = typeof(TextField).GetProperty("textInput", BindingFlags.NonPublic | BindingFlags.Instance);
			_textProperty = _textInputProperty?.PropertyType?.GetProperty("text", BindingFlags.Public | BindingFlags.Instance);
		}

		#endregion

		#region Public Interface

		public Placeholder() : this(null)
		{
		}

		public Placeholder(string text) : base(text)
		{
			pickingMode = PickingMode.Ignore;

			AddToClassList(UssClassName);
			this.AddStyleSheet(Stylesheet);

			RegisterCallback<AttachToPanelEvent>(OnAttached);
		}

		public void AddToField(TextField textField)
		{
			// Add this specifically to the input field in case the TextField has a label
			var input = textField.Q(className: TextField.inputUssClassName);
			input.Add(this);

			UpdateDisplayed(textField);

			textField.RegisterCallback<KeyDownEvent>(evt => UpdateDisplayed(textField));
		}

		#endregion

		#region State Management

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

		private void OnAttached(AttachToPanelEvent evt)
		{
			if (parent is TextField textField)
				AddToField(textField);
		}

		#endregion

		#region Events

		public override void HandleEvent(EventBase evt)
		{
			// Capture ChangeEvents so they aren't handled by the parent TextField.

			if (evt is ChangeEvent<string>)
			{
				evt.StopPropagation();
				evt.PreventDefault();
			}
			else
			{
				base.HandleEvent(evt);
			}
		}

		#endregion

		#region UXML Support

		public new class UxmlFactory : UxmlFactory<Placeholder, UxmlTraits> { }

		#endregion
	}
}
