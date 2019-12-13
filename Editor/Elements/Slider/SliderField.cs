using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public abstract class SliderField<ValueType> : BaseField<ValueType>
	{
		public const string Stylesheet = "Slider/SliderStyle.uss";
		public const string UssClassName = "pirho-slider-field";
		public const string LabelUssClassName = UssClassName + "__label";
		public const string InputUssClassName = UssClassName + "__input";

		protected readonly SliderControl _control;

		public ValueType Minimum
		{
			get => _control.Minimum;
			set => _control.Minimum = value;
		}

		public ValueType Maximum
		{
			get => _control.Maximum;
			set => _control.Maximum = value;
		}

		protected SliderField(string label, SliderControl control) : base(label, control)
		{
			_control = control;
			_control.AddToClassList(InputUssClassName);
			_control.RegisterCallback<ChangeEvent<ValueType>>(evt =>
			{
				base.value = evt.newValue;
				evt.StopImmediatePropagation();
			});

			labelElement.AddToClassList(LabelUssClassName);

			AddToClassList(UssClassName);
			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
		}

		public override void SetValueWithoutNotify(ValueType newValue)
		{
			base.SetValueWithoutNotify(newValue);
			_control.SetValueWithoutNotify(newValue);
		}

		protected abstract class SliderControl : VisualElement
		{
			public const string SliderUssClassName = InputUssClassName + "__slider";
			public const string TextUssClassName = InputUssClassName + "__text";

			public abstract ValueType Minimum { get; set; }
			public abstract ValueType Maximum { get; set; }

			public abstract void SetValueWithoutNotify(ValueType value);
		}

	#region UXML Support

		public class UxmlTraits<AttributeType> : BaseFieldTraits<ValueType, AttributeType> where AttributeType : TypedUxmlAttributeDescription<ValueType>, new()
		{
			private readonly AttributeType _minimum = new AttributeType { name = "minimum" };
			private readonly AttributeType _maximum = new AttributeType { name = "maximum" };

			public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
			{
				var field = element as SliderField<ValueType>;
				field.Minimum = _minimum.GetValueFromBag(bag, cc);
				field.Maximum = _maximum.GetValueFromBag(bag, cc);

				base.Init(element, bag, cc);
			}
		}
	}

	#endregion
}