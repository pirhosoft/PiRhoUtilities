using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class ComboBoxField : BaseField<string>
	{
		#region Class Names

		public const string Stylesheet = "ComboBox/ComboBoxStyle.uss";
		public const string UssClassName = "pirho-combo-box-field";
		public const string LabelUssClassName = UssClassName + "__label";
		public const string InputUssClassName = UssClassName + "__input";

		#endregion

		#region Defaults

		public const bool DefaultIsDelayed = false;

		#endregion

		#region Members

		private readonly ComboBoxControl _comboBox;

		#endregion

		#region Public Interface

		public bool IsDelayed
		{
			get => _comboBox.TextField.isDelayed;
			set => _comboBox.TextField.isDelayed = value;
		}

		public List<string> Options
		{
			get => _comboBox.Options;
			set => _comboBox.Options = value;
		}

		public ComboBoxField(string label) : base(label, null)
		{
			_comboBox = new ComboBoxControl();
			_comboBox.AddToClassList(InputUssClassName);
			_comboBox.RegisterCallback<ChangeEvent<string>>(evt =>
			{
				base.value = evt.newValue;
				evt.StopImmediatePropagation();
			});

			labelElement.AddToClassList(LabelUssClassName);

			AddToClassList(UssClassName);
			this.SetVisualInput(_comboBox);
			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
		}

		public ComboBoxField(string label, List<string> options) : this(label)
		{
			Options = options;
		}

		public ComboBoxField(List<string> options) : this(null, options)
		{
		}

		public override void SetValueWithoutNotify(string newValue)
		{
			base.SetValueWithoutNotify(newValue);
			_comboBox.SetValueWithoutNotify(newValue);
		}

		#endregion

		#region Visual Input

		private class ComboBoxControl : VisualElement
		{
			public const string InputTextUssClassName = InputUssClassName + "__text";
			public const string InputButtonUssClassName = InputUssClassName + "__button";

			public TextField TextField { get; private set; }

			private List<string> _options;
			public List<string> Options
			{ 
				get => _options;
				set
				{
					_options = value;
					_menu = new GenericMenu();

					if (_options != null)
					{
						foreach (var option in _options)
							_menu.AddItem(new GUIContent(option), false, () => SelectItem(option));
					}

					_dropdownButton.SetEnabled(_options != null && _options.Count > 0);
				}
			}

			private readonly VisualElement _dropdownButton;

			private GenericMenu _menu;

			public ComboBoxControl()
			{
				TextField = new TextField { isDelayed = DefaultIsDelayed };
				TextField.AddToClassList(InputTextUssClassName);

				var arrow = new VisualElement();
				arrow.AddToClassList(BasePopupField<string, string>.arrowUssClassName);

				_dropdownButton = new VisualElement();
				_dropdownButton.AddToClassList(BasePopupField<string, string>.inputUssClassName);
				_dropdownButton.AddToClassList(InputButtonUssClassName);
				_dropdownButton.AddManipulator(new Clickable(OpenDropdown));
				_dropdownButton.SetEnabled(false);
				_dropdownButton.Add(arrow);

				Add(TextField);
				Add(_dropdownButton);
			}

			public void SetValueWithoutNotify(string value)
			{
				TextField.SetValueWithoutNotify(value);
			}

			private void OpenDropdown()
			{
				_menu?.DropDown(_dropdownButton.worldBound);
			}

			private void SelectItem(string option)
			{
				this.SendChangeEvent(TextField.value, option);
			}
		}

		#endregion

		#region UXML Support

		public ComboBoxField() : this(null, null) { }

		public new class UxmlFactory : UxmlFactory<ComboBoxField, UxmlTraits> { }
		public new class UxmlTraits : BaseFieldTraits<string, UxmlStringAttributeDescription>
		{
			private readonly UxmlStringAttributeDescription _options = new UxmlStringAttributeDescription { name = "options" };
			private readonly UxmlBoolAttributeDescription _delayed = new UxmlBoolAttributeDescription { name = "is-delayed", defaultValue = DefaultIsDelayed };

			public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(element, bag, cc);

				var field = element as ComboBoxField;
				var options = _options.GetValueFromBag(bag, cc);

				field.Options = options.Split(',').ToList();
				field.IsDelayed = _delayed.GetValueFromBag(bag, cc);
			}
		}

		#endregion
	}
}