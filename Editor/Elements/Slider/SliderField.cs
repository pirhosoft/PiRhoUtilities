using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public abstract class SliderBaseField<ValueType> : BaseField<ValueType>
	{
		public static readonly string UssClassName = "pirho-slider-field";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string InputUssClassName = UssClassName + "__input";

		public BaseSliderControl<ValueType> Control { get; protected set; }

		public SliderBaseField(string label) : base(label, null)
		{
		}

		protected void Setup(ValueType value)
		{
			Control.AddToClassList(InputUssClassName);
			Control.RegisterCallback<ChangeEvent<ValueType>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(Control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value);
		}

		public override void SetValueWithoutNotify(ValueType newValue)
		{
			base.SetValueWithoutNotify(newValue);
			Control.SetValueWithoutNotify(newValue);
		}
	}

	public class SliderIntField : SliderBaseField<int>
	{
		public SliderIntField(SerializedProperty property, int min, int max) : this(property.displayName, property.intValue, min, max)
		{
			this.ConfigureProperty(property, property.GetTooltip());
		}

		public SliderIntField(string label, int value, int min, int max) : base(label)
		{
			Setup(value, min, max);
		}

		private void Setup(int value, int min, int max)
		{
			Control = new SliderIntControl(value, min, max);

			Setup(value);
		}

		#region UXML Support

		public SliderIntField() : base(null) { }

		public new class UxmlFactory : UxmlFactory<SliderIntField, UxmlTraits> { }

		public new class UxmlTraits : BaseField<int>.UxmlTraits
		{
			private UxmlIntAttributeDescription _value = new UxmlIntAttributeDescription { name = "value" };
			private UxmlIntAttributeDescription _minimum = new UxmlIntAttributeDescription { name = "minimum", use = UxmlAttributeDescription.Use.Required };
			private UxmlIntAttributeDescription _maximum = new UxmlIntAttributeDescription { name = "maximum", use = UxmlAttributeDescription.Use.Required };

			public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(element, bag, cc);

				var field = element as SliderIntField;
				var value = _value.GetValueFromBag(bag, cc);
				var min = _minimum.GetValueFromBag(bag, cc);
				var max = _maximum.GetValueFromBag(bag, cc);

				field.Setup(value, min, max);
			}
		}

		#endregion

	}

	public class SliderFloatField : SliderBaseField<float>
	{
		public SliderFloatField(SerializedProperty property, float min, float max) : this(property.displayName, property.floatValue, min, max)
		{
			this.ConfigureProperty(property, property.GetTooltip());
		}

		public SliderFloatField(string label, float value, float min, float max) : base(label)
		{
			Setup(value, min, max);
		}

		private void Setup(float value, float min, float max)
		{
			Control = new SliderFloatControl(value, min, max);

			Setup(value);
		}

		#region UXML Support

		public SliderFloatField() : base(null) { }

		public new class UxmlFactory : UxmlFactory<SliderFloatField, UxmlTraits> { }

		public new class UxmlTraits : BaseField<float>.UxmlTraits
		{
			private UxmlFloatAttributeDescription _value = new UxmlFloatAttributeDescription { name = "value" };
			private UxmlFloatAttributeDescription _minimum = new UxmlFloatAttributeDescription { name = "minimum", use = UxmlAttributeDescription.Use.Required };
			private UxmlFloatAttributeDescription _maximum = new UxmlFloatAttributeDescription { name = "maximum", use = UxmlAttributeDescription.Use.Required };

			public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(element, bag, cc);

				var field = element as SliderFloatField;
				var value = _value.GetValueFromBag(bag, cc);
				var min = _minimum.GetValueFromBag(bag, cc);
				var max = _maximum.GetValueFromBag(bag, cc);

				field.Setup(value, min, max);
			}
		}

		#endregion
	}

	public class MinMaxSliderField : SliderBaseField<Vector2>
	{
		public MinMaxSliderField(SerializedProperty property, float min, float max) : this(property.displayName, property.vector2Value, min, max)
		{
			this.ConfigureProperty(property, property.GetTooltip());
		}

		public MinMaxSliderField(string label, Vector2 value, float min, float max) : base(label)
		{
			Setup(value, min, max);
		}

		private void Setup(Vector2 value, float min, float max)
		{
			Control = new MinMaxSliderControl(value, min, max);

			Setup(value);
		}

		#region UXML Support

		public MinMaxSliderField() : base(null) { }

		public new class UxmlFactory : UxmlFactory<MinMaxSliderField, UxmlTraits> { }

		public new class UxmlTraits : BaseField<Enum>.UxmlTraits
		{
			private UxmlFloatAttributeDescription _maxValue = new UxmlFloatAttributeDescription { name = "minimumValue" };
			private UxmlFloatAttributeDescription _minValue = new UxmlFloatAttributeDescription { name = "maximumValue" };
			private UxmlFloatAttributeDescription _minimum = new UxmlFloatAttributeDescription { name = "minimum", use = UxmlAttributeDescription.Use.Required };
			private UxmlFloatAttributeDescription _maximum = new UxmlFloatAttributeDescription { name = "maximum", use = UxmlAttributeDescription.Use.Required };

			public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(element, bag, cc);

				var field = element as MinMaxSliderField;
				var maxValue = _minValue.GetValueFromBag(bag, cc);
				var minValue = _maxValue.GetValueFromBag(bag, cc);
				var min = _minimum.GetValueFromBag(bag, cc);
				var max = _maximum.GetValueFromBag(bag, cc);

				field.Setup(new Vector2(minValue, maxValue), min, max);
			}
		}

		#endregion
	}
}