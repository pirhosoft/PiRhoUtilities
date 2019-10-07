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

		private SerializedProperty _property;

		public EulerField(string label, Quaternion value) : base(label, null)
		{
			Setup(value);
		}

		private void Setup(Quaternion value)
		{
			var control = new EulerControl(value);

			AddToClassList(UssClassName);
			labelElement.AddToClassList(LabelUssClassName);
			control.AddToClassList(InputUssClassName);

			this.SetVisualInput(control);
			this.RegisterValueChangedCallback(evt => control.SetValueWithoutNotify(evt.newValue));
			control.RegisterCallback<ChangeEvent<Quaternion>>(evt => base.value = evt.newValue);
		}

		#region UXML Support

		public EulerField() : base(null, null) {}

		public new class UxmlFactory : UxmlFactory<EulerField, UxmlTraits> { }

		public new class UxmlTraits : BaseField<Quaternion>.UxmlTraits
		{
			private UxmlFloatAttributeDescription _x = new UxmlFloatAttributeDescription { name = "x" };
			private UxmlFloatAttributeDescription _y = new UxmlFloatAttributeDescription { name = "y" };
			private UxmlFloatAttributeDescription _z = new UxmlFloatAttributeDescription { name = "z" };

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