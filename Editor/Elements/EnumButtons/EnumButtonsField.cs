using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class EnumButtonsField : BaseField<Enum>
	{
		public const string Stylesheet = "EnumButtons/EnumButtonsStyle.uss";
		public const string UssClassName = "pirho-enum-buttons-field";
		public const string LabelUssClassName = UssClassName + "__label";
		public const string InputUssClassName = UssClassName + "__input";
		public const string ButtonUssClassName = InputUssClassName + "__button";
		public const string ActiveButtonUssClassName = ButtonUssClassName + "--active";
		public const string FirstButtonUssClassName = ButtonUssClassName + "--first";
		public const string LastButtonUssClassName = ButtonUssClassName + "--last";

		private const string _invalidTypeWarning = "(PUEBFIT) failed to setup EnumButtonsField: the type '{0}' is not an enum";
		private const string _invalidValueWarning = "(PUEBFIV) failed to set EnumButtonsField value: '{0}' is not a valid value for the enum '{1}'";

		public Type Type
		{
			get => _control.Type;
			set => _control.Type = value;
		}

		public bool UseFlags
		{
			get => _control.UseFlags;
			set => _control.UseFlags = value;
		}

		private readonly EnumButtonsControl _control;

		public EnumButtonsField(string label) : base(label, null)
		{
			_control = new EnumButtonsControl();
			_control.AddToClassList(InputUssClassName);
			_control.RegisterCallback<ChangeEvent<Enum>>(evt =>
			{
				base.value = evt.newValue;
				evt.StopImmediatePropagation();
			});

			labelElement.AddToClassList(LabelUssClassName);

			AddToClassList(UssClassName);
			this.SetVisualInput(_control);
			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
		}

		public EnumButtonsField(string label, Type type) : this(label)
		{
			Type = type;

			// Initialize this so that the binding can look up the type
			base.SetValueWithoutNotify(Enum.ToObject(type, 0) as Enum);
		}

		public EnumButtonsField(Type type) : this(null, type)
		{
		}

		public override void SetValueWithoutNotify(Enum newValue)
		{
			base.SetValueWithoutNotify(newValue);
			_control.SetValueWithoutNotify(newValue);
		}

		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);

			if (this.TryGetPropertyBindEvent(evt, out var property))
			{
				BindingExtensions.DefaultEnumBind(this, property);
				evt.StopPropagation();
			}
		}

		private class EnumButtonsControl : VisualElement
		{
			private Type _type;
			public Type Type
			{
				get => _type;
				set => SetType(value);
			}

			private bool? _useFlags;
			public bool UseFlags
			{
				get => _useFlags.GetValueOrDefault(Type.HasAttribute<FlagsAttribute>());
				set => _useFlags = value;
			}

			private readonly UQueryState<Button> _buttons;

			private Enum _value;
			private string[] _names;
			private Array _values;

			public EnumButtonsControl()
			{
				_buttons = this.Query<Button>().Build();
			}

			public void SetValueWithoutNotify(Enum value)
			{
				if (value == null || Type != value.GetType())
				{
					Debug.LogWarningFormat(_invalidValueWarning, value, Type);
				}
				else if (!Equals(_value, value))
				{
					_value = value;
					_buttons.ForEach(button =>
					{
						var index = IndexOf(button);

						button.EnableInClassList(FirstButtonUssClassName, index == 0);
						button.EnableInClassList(LastButtonUssClassName, index == _names.Length - 1);

						if (UseFlags)
						{
							var current = GetIntFromEnum(Type, _value);
							var buttonValue = GetIntFromEnum(Type, button.userData as Enum);

							button.EnableInClassList(ActiveButtonUssClassName, (buttonValue != 0 && (current & buttonValue) == buttonValue) || (current == 0 && buttonValue == 0));
						}
						else
						{
							button.EnableInClassList(ActiveButtonUssClassName, _value.Equals(button.userData as Enum));
						}
					});
				}
			}

			private void SetType(Type type)
			{
				if (type != _type)
				{
					_type = type;

					Clear();

					if (_type == null || !_type.IsEnum)
					{
						Debug.LogWarningFormat(_invalidTypeWarning, _type);
					}
					else
					{
						_names = Enum.GetNames(_type);
						_values = Enum.GetValues(_type);

						var value = _values.Length > 0 ? _values.GetValue(0) as Enum : Enum.ToObject(type, 0) as Enum;

						Rebuild();
						SetValueWithoutNotify(value);
					}
				}
			}

			private void Rebuild()
			{
				if (_names.Length > 0)
				{
					for (var i = 0; i < _names.Length; i++)
					{
						var index = i;
						var button = new Button(() => Toggle(index))
						{
							text = _names[i],
							userData = _values.GetValue(i)
						};

						button.AddToClassList(ButtonUssClassName);
						Add(button);
					}
				}
			}

			private void Toggle(int index)
			{
				var selected = _values.GetValue(index) as Enum;

				if (UseFlags)
				{
					var current = GetIntFromEnum(Type, _value);
					var buttonValue = GetIntFromEnum(Type, selected);

					if ((buttonValue != 0 && (current & buttonValue) == buttonValue) || (current == 0 && buttonValue == 0))
					{
						if (buttonValue != ~0)
							current &= ~buttonValue;
					}
					else
					{
						if (buttonValue == 0)
							current = 0;
						else
							current |= buttonValue;
					}

					selected = GetEnumFromInt(Type, current);
				}

				this.SendChangeEvent(_value, selected);
			}

			private Enum GetEnumFromInt(Type type, int value)
			{
				return Enum.ToObject(type, value) as Enum;
			}

			private int GetIntFromEnum(Type type, Enum value)
			{
				return (int)Enum.Parse(type, value.ToString());
			}
		}

		#region UXML Support

		public EnumButtonsField() : base(null, null) { }

		public new class UxmlFactory : UxmlFactory<EnumButtonsField, UxmlTraits> { }
		public new class UxmlTraits : BaseField<Enum>.UxmlTraits
		{
			private readonly UxmlStringAttributeDescription _type = new UxmlStringAttributeDescription { name = "type", use = UxmlAttributeDescription.Use.Required };
			private readonly UxmlBoolAttributeDescription _flags = new UxmlBoolAttributeDescription { name = "flags" };
			private readonly UxmlStringAttributeDescription _value = new UxmlStringAttributeDescription { name = "value" };

			public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(element, bag, cc);

				var field = element as EnumButtonsField;
				var typeName = _type.GetValueFromBag(bag, cc);
				var type = Type.GetType(typeName, false);

				var flags = false;
				if (_flags.TryGetValueFromBag(bag, cc, ref flags))
					field.UseFlags = flags;

				field.Type = type;

				var valueName = _value.GetValueFromBag(bag, cc);
				if (!string.IsNullOrEmpty(valueName))
				{
					if (TryParseValue(type, valueName, out var value))
						field.SetValueWithoutNotify(value);
					else
						Debug.LogWarningFormat(_invalidValueWarning, valueName, type.Name);
				}
			}

			private bool TryParseValue(Type type, string valueName, out Enum value)
			{
				try
				{
					value = Enum.Parse(type, valueName) as Enum;
					return true;
				}
				catch (Exception exception) when (exception is ArgumentException || exception is OverflowException)
				{
					value = null;
					return false;
				}
			}
		}

		#endregion
	}
}