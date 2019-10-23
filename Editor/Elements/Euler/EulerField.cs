using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class EulerField : BaseField<Quaternion>
	{
		public static readonly string UssClassName = "pirho-euler-field";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string InputUssClassName = UssClassName + "__input";

		public EulerControl Control { get; private set; }

		public EulerField(SerializedProperty property) : this(property.displayName, property.quaternionValue)
		{
			this.ConfigureProperty(property, property.GetTooltip());
		}

		public EulerField(string label, Quaternion value) : base(label, null)
		{
			Setup(value);
		}

		private void Setup(Quaternion value)
		{
			Control = new EulerControl(value);
			Control.AddToClassList(InputUssClassName);
			Control.RegisterCallback<ChangeEvent<Quaternion>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(Control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value);
		}

		public override void SetValueWithoutNotify(Quaternion newValue)
		{
			base.SetValueWithoutNotify(newValue);
			Control.SetValueWithoutNotify(newValue);
		}

		#region UXML Support

		public EulerField() : base(null, null) {}

		public new class UxmlFactory : UxmlFactory<EulerField, UxmlTraits> { }

		public new class UxmlTraits : BaseField<Quaternion>.UxmlTraits
		{
			private readonly UxmlFloatAttributeDescription _x = new UxmlFloatAttributeDescription { name = "x" };
			private readonly UxmlFloatAttributeDescription _y = new UxmlFloatAttributeDescription { name = "y" };
			private readonly UxmlFloatAttributeDescription _z = new UxmlFloatAttributeDescription { name = "z" };

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);

				var euler = ve as EulerField;
				var x = _x.GetValueFromBag(bag, cc);
				var y = _y.GetValueFromBag(bag, cc);
				var z = _z.GetValueFromBag(bag, cc);

				euler.Setup(Quaternion.Euler(x, y, z));
			}
		}

		#endregion
	}
}