using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class ComboBoxField : BaseField<string>
	{
		public static readonly string UssClassName = "pirho-combo-box-field";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string InputUssClassName = UssClassName + "__input";

		public ComboBoxControl Control { get; private set; }

		public ComboBoxField(string label, string value, List<string> options) : base(label, null)
		{
			Setup(value, options);
		}

		private void Setup(string value, List<string> options)
		{
			Control = new ComboBoxControl(value, options);
			Control.AddToClassList(InputUssClassName);
			Control.RegisterCallback<ChangeEvent<string>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(Control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value);
		}

		public override void SetValueWithoutNotify(string newValue)
		{
			base.SetValueWithoutNotify(newValue);
			Control.SetValueWithoutNotify(newValue);
		}

		#region UXML Support

		public ComboBoxField() : base(null, null) { }

		public new class UxmlFactory : UxmlFactory<ComboBoxField, UxmlTraits> { }

		public new class UxmlTraits : BaseField<string>.UxmlTraits
		{
			private UxmlStringAttributeDescription _options = new UxmlStringAttributeDescription { name = "options", use = UxmlAttributeDescription.Use.Required };
			private UxmlStringAttributeDescription _value = new UxmlStringAttributeDescription { name = "value" };

			public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(element, bag, cc);

				var field = element as ComboBoxField;
				var options = _options.GetValueFromBag(bag, cc);
				var value = _value.GetValueFromBag(bag, cc);

				field.Setup(value, options.Split(',').ToList());
			}
		}

		#endregion
	}
}