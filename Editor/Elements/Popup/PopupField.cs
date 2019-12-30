using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class PopupField<T> : BaseField<T>
	{
		#region Class Names

		public const string UssClassName = "pirho-popup-field";
		public const string LabelUssClassName = UssClassName + "__label";
		public const string InputUssClassName = UssClassName + "__input";

		#endregion

		#region Log Messages

		private const string _invalidValuesError = "(PUPFIV) invalid values for PopupField: Values must not be null and must have a Count > 0";
		private const string _invalidOptionsWarning = "(PUPFIO) invalid Options for PopupField: the number of Options does not match the number of Values";
		private const string _missingValueWarning = "(PUPFMV) the value of PopupField did not exsist in the list of values: Changing to the first valid value";

		#endregion

		#region Private Members

		private UnityEditor.UIElements.PopupField<T> _popup;
		private List<T> _values;
		private List<string> _options;

		#endregion

		#region Public Interface

		public List<T> Values
		{
			get => _values;
			set => SetValues(value);
		}

		public List<string> Options
		{
			get => _options;
			set => SetValues(_values, value);
		}

		public PopupField() : this(null)
		{
		}

		public PopupField(string label) : base(label, null)
		{
			AddToClassList(UssClassName);
		}

		public PopupField(string label, List<T> values, List<string> options = null) : this(label)
		{
			SetValues(values, options);
		}

		public PopupField(List<T> values, List<string> options = null) : this(null, values, options)
		{
		}

		public void SetValues(List<T> values, List<string> options = null)
		{
			_options = options;

			if (_values != values)
			{
				DestroyPopup();

				_values = values;

				if (_values != null && _values.Count > 0)
				{
					if (!_values.Contains(value))
					{
						base.value = _values[0];
						Debug.LogWarningFormat(_missingValueWarning);
					}

					CreatePopup();
				}
				else
				{
					_values = null;
					Debug.LogErrorFormat(_invalidValuesError);
				}
			}

			if (_options != null && _values != null && _options.Count != _values.Count)
			{
				_options = null;
				Debug.LogWarningFormat(_invalidOptionsWarning);
			}
		}

		public override void SetValueWithoutNotify(T newValue)
		{
			base.SetValueWithoutNotify(newValue);
			_popup?.SetValueWithoutNotify(newValue);
		}

		#endregion

		#region Popup Management

		private void CreatePopup()
		{
			_popup?.RemoveFromHierarchy();
			_popup = new UnityEditor.UIElements.PopupField<T>(_values, value, Format, Format);
			_popup.AddToClassList(InputUssClassName);
			_popup.RegisterCallback<ChangeEvent<T>>(evt =>
			{
				base.value = evt.newValue;
				evt.StopImmediatePropagation();
			});

			this.SetVisualInput(_popup);
		}

		private void DestroyPopup()
		{
			_popup?.RemoveFromHierarchy();
			_popup = null;
		}

		private string Format(T value)
		{
			var index = _values.IndexOf(value);

			if (_options == null || index < 0 || index >= _options.Count)
				return value.ToString();

			return _options[index];
		}

		#endregion

		#region UXML Support

		public class UxmlTraits<AttributeType> : BaseFieldTraits<T, AttributeType> where AttributeType : TypedUxmlAttributeDescription<T>, new()
		{
			private readonly UxmlStringAttributeDescription _options = new UxmlStringAttributeDescription { name = "options" };

			public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(element, bag, cc);

				var field = element as PopupField<T>;
				var options = _options.GetValueFromBag(bag, cc);

				field.Options = options.Split(',').ToList();
			}
		}

		#endregion
	}

	#region Concrete Classes

	public class PopupIntField : PopupField<int>
	{
		public PopupIntField() { }
		public PopupIntField(string label) : base(label) { }
		public PopupIntField(string label, List<int> values, List<string> options = null) : base(label, values, options) { }
		public PopupIntField(List<int> values, List<string> options = null) : base(values, options) { }

		public new class UxmlFactory : UxmlFactory<PopupIntField, UxmlTraits> { }
		public new class UxmlTraits : UxmlTraits<UxmlIntAttributeDescription>
		{
			private readonly UxmlStringAttributeDescription _values = new UxmlStringAttributeDescription { name = "values" };

			public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(element, bag, cc);

				var field = element as PopupIntField;
				var values = _values.GetValueFromBag(bag, cc);

				field.Values = values.Split(',').Where(value => int.TryParse(value, out var _)).Select(value => int.Parse(value)).ToList();
			}
		}
	}

	public class PopupFloatField : PopupField<float>
	{
		public PopupFloatField() { }
		public PopupFloatField(string label) : base(label) { }
		public PopupFloatField(string label, List<float> values, List<string> options = null) : base(label, values, options) { }
		public PopupFloatField(List<float> values, List<string> options = null) : base(values, options) { }

		public new class UxmlFactory : UxmlFactory<PopupFloatField, UxmlTraits> { }
		public new class UxmlTraits : UxmlTraits<UxmlFloatAttributeDescription>
		{
			private readonly UxmlStringAttributeDescription _values = new UxmlStringAttributeDescription { name = "values" };

			public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(element, bag, cc);

				var field = element as PopupFloatField;
				var values = _values.GetValueFromBag(bag, cc);

				field.Values = values.Split(',').Where(value => float.TryParse(value, out var _)).Select(value => float.Parse(value)).ToList();
			}
		}
	}

	public class PopupStringField : PopupField<string>
	{
		public PopupStringField() { }
		public PopupStringField(string label) : base(label) { }
		public PopupStringField(string label, List<string> values, List<string> options = null) : base(label, values, options) { }
		public PopupStringField(List<string> values, List<string> options = null) : base(values, options) { }

		public new class UxmlFactory : UxmlFactory<PopupStringField, UxmlTraits> { }
		public new class UxmlTraits : UxmlTraits<UxmlStringAttributeDescription>
		{
			private readonly UxmlStringAttributeDescription _values = new UxmlStringAttributeDescription { name = "values" };

			public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(element, bag, cc);

				var field = element as PopupStringField;
				var values = _values.GetValueFromBag(bag, cc);

				field.Values = values.Split(',').ToList();
			}
		}
	}

	#endregion
}
