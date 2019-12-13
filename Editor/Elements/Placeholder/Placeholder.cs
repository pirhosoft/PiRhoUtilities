using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class Placeholder : Label
	{
		#region Class Names

		public const string Stylesheet = "Placeholder/PlaceholderStyle.uss";
		public const string UssClassName = "pirho-placeholder";

		#endregion

		#region Log Messages

		private const string _invalidParentWarning = "(PUPCIP) Invalid parent for Placeholder: Placeholders can only be added to TextFields";

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

		public Placeholder(string text) : base(text)
		{
			pickingMode = PickingMode.Ignore;

			AddToClassList(UssClassName);
			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
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

		#endregion

		#region UXML Support

		public Placeholder() : this(null) { }

		public new class UxmlFactory : UxmlFactory<Placeholder, UxmlTraits> { }
		public new class UxmlTraits : Label.UxmlTraits
		{
			public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(element, bag, cc);

				var field = element as Placeholder;

				if (field.parent is TextField textField)
					field.AddToField(textField);
				else
				{
					field.RemoveFromHierarchy();
					Debug.LogWarning(_invalidParentWarning);
				}
			}
		}

		#endregion
	}
}