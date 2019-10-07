using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class TypePickerField : BaseField<string>
	{
		public static readonly string UssClassName = "pirho-scene-picker-field";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string InputUssClassName = UssClassName + "__input";

		private TypePickerControl _control;

		public TypePickerField(string label, string value, Type type, bool showAbstract) : base(label, null)
		{
			Setup(value, type, showAbstract);
		}

		private void Setup(string value, Type type, bool showAbstract)
		{
			_control = new TypePickerControl(value, type, showAbstract);
			_control.AddToClassList(InputUssClassName);
			_control.RegisterCallback<ChangeEvent<string>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(_control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value);
		}

		public override void SetValueWithoutNotify(string newValue)
		{
			base.SetValueWithoutNotify(newValue);
			_control.SetValueWithoutNotify(newValue);
		}

		#region UXML Support

		private const string _invalidTypeError = "(PUTPFIT) failed to setup TypePickerField: the type '{0}' could not be found";

		public TypePickerField() : base(null, null) { }

		public new class UxmlFactory : UxmlFactory<TypePickerField, UxmlTraits> { }

		public new class UxmlTraits : BaseField<string>.UxmlTraits
		{
			private UxmlStringAttributeDescription _type = new UxmlStringAttributeDescription { name = "type", use = UxmlAttributeDescription.Use.Required };
			private UxmlStringAttributeDescription _value = new UxmlStringAttributeDescription { name = "value" };
			private UxmlBoolAttributeDescription _showAbstract = new UxmlBoolAttributeDescription { name = "show-abstract", defaultValue = true };

			public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(element, bag, cc);

				var field = element as TypePickerField;
				var typeName = _type.GetValueFromBag(bag, cc);
				var type = Type.GetType(typeName, false);

				if (type == null)
				{
					Debug.LogErrorFormat(_invalidTypeError, typeName);
				}
				else
				{
					var Name = _value.GetValueFromBag(bag, cc);
					var valueName = _value.GetValueFromBag(bag, cc);
					var showAbstract = _showAbstract.GetValueFromBag(bag, cc);

					field.Setup(valueName, type, showAbstract);
				}
			}
		}

		#endregion
	}
}
