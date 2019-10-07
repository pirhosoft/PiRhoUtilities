using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class EnumButtonsField : BaseField<Enum>
	{
		public static readonly string UssClassName = "pirho-enum-buttons-field";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string InputUssClassName = UssClassName + "__input";

		private EnumButtonsControl _control;

		public EnumButtonsField(string label, Enum value, bool? useFlags = null) : base(label, null)
		{
			Setup(value, useFlags);
		}

		private void Setup(Enum value, bool? useFlags = null)
		{
			_control = new EnumButtonsControl(value, useFlags);
			_control.AddToClassList(InputUssClassName);
			_control.RegisterCallback<ChangeEvent<Enum>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(_control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value);
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

		#region UXML Support

		private const string _invalidTypeError = "(PUEBFIT) failed to setup EnumButtonsField: the type '{0}' is not an enum";
		private const string _invalidValueWarning = "(PUEBFIT) failed to set EnumButtonsField value: '{0}' is not a valid value for the enum '{1}'";

		public EnumButtonsField() : base(null, null) { }

		public new class UxmlFactory : UxmlFactory<EnumButtonsField, UxmlTraits> { }

		public new class UxmlTraits : BaseField<Enum>.UxmlTraits
		{
			private UxmlStringAttributeDescription _type = new UxmlStringAttributeDescription { name = "type", use = UxmlAttributeDescription.Use.Required };
			private UxmlBoolAttributeDescription _flags = new UxmlBoolAttributeDescription { name = "flags" };
			private UxmlStringAttributeDescription _value = new UxmlStringAttributeDescription { name = "value" };

			public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(element, bag, cc);

				var field = element as EnumButtonsField;
				var typeName = _type.GetValueFromBag(bag, cc);

				var type = Type.GetType(typeName, false);

				if (type == null || !type.IsEnum)
				{
					Debug.LogErrorFormat(_invalidTypeError, typeName);
				}
				else
				{
					var flags = true;
					var useFlags = _flags.TryGetValueFromBag(bag, cc, ref flags) ? (bool?)flags : null;
					var valueName = _value.GetValueFromBag(bag, cc);
					var value = ParseValue(type, valueName);

					field.Setup(value as Enum, useFlags);
				}
			}

			private Enum ParseValue(Type type, string value)
			{
				if (!string.IsNullOrEmpty(value))
				{
					try
					{
						return Enum.Parse(type, value) as Enum;
					}
					catch (Exception exception) when (exception is ArgumentException || exception is OverflowException)
					{
						Debug.LogWarningFormat(_invalidValueWarning, value, type.Name);
					}
				}

				return Enum.ToObject(type, 0) as Enum;
			}
		}

		#endregion
	}
}