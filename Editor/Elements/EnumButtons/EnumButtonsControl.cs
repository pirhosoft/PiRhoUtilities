using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class EnumButtonsControl : VisualElement
	{
		private const string _invalidTypeWarning = "(PUEBCIT) failed to set value on EnumButtonsControl: attempted to set value '{0}' of enum '{1}' but the control is using enum type '{2}'";

		public const string Stylesheet = "EnumButtons/EnumButtonsStyle.uss";
		public static readonly string UssClassName = "pirho-enum-buttons";
		public static readonly string ButtonUssClassName = UssClassName + "__button";
		public static readonly string ActiveButtonUssClassName = ButtonUssClassName + "--active";

		public Enum Value { get; private set; }
		public Type Type { get; private set; }
		public bool UseFlags { get; private set; }

		private string[] _names;
		private Array _values;
		private UQueryState<Button> _buttons;

		public EnumButtonsControl(Enum value, bool? useFlags = null)
		{
			Value = value;
			Type = value.GetType();
			UseFlags = useFlags ?? Type.HasAttribute<FlagsAttribute>();

			_names = Enum.GetNames(Type);
			_values = Enum.GetValues(Type);
			_buttons = this.Query<Button>().Build();

			Rebuild();

			AddToClassList(UssClassName);
			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
		}

		public void SetValueWithoutNotify(Enum value)
		{
			if (Type != value.GetType())
			{
				Debug.LogWarningFormat(_invalidTypeWarning, value.ToString(), value.GetType().Name, Type.Name);
			}
			else if (!Value.Equals(value))
			{
				Value = value;
				Refresh();
			}
		}

		private void Rebuild()
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

			Refresh();
		}

		private void Refresh()
		{
			_buttons.ForEach(button =>
			{
				if (UseFlags)
				{
					var current = GetIntFromEnum(Type, Value);
					var buttonValue = GetIntFromEnum(Type, button.userData as Enum);

					button.EnableInClassList(ActiveButtonUssClassName, (buttonValue != 0 && (current & buttonValue) == buttonValue) || (current == 0 && buttonValue == 0));
				}
				else
				{
					button.EnableInClassList(ActiveButtonUssClassName, Value.Equals(button.userData as Enum));
				}
			});
		}

		private void Toggle(int index)
		{
			var value = Value;
			var previousValue = Value;
			var selected = _values.GetValue(index) as Enum;

			if (UseFlags)
			{
				var current = GetIntFromEnum(Type, Value);
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

				value = GetEnumFromInt(Type, current);
			}
			else
			{
				value = selected;
			}

			SetValueWithoutNotify(value);
			this.SendChangeEvent(previousValue, value);
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
}