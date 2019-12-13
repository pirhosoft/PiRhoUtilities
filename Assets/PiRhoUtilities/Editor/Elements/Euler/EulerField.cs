using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class EulerField : BaseField<Quaternion>
	{
		#region Class Names

		public const string Stylesheet = "Euler/EulerStyle.uss";
		public const string UssClassName = "pirho-euler-field";
		public const string LabelUssClassName = UssClassName + "__label";
		public const string InputUssClassName = UssClassName + "__input";

		#endregion

		#region Members

		private readonly Vector3Field _vectorField;

		#endregion

		#region Public Interface

		public EulerField(string label) : base(label, null)
		{
			_vectorField = new Vector3Field();
			_vectorField.AddToClassList(InputUssClassName);
			_vectorField.RegisterCallback<ChangeEvent<Vector3>>(evt =>
			{
				base.value = Quaternion.Euler(evt.newValue);
				evt.StopImmediatePropagation();
			});

			labelElement.AddToClassList(LabelUssClassName);

			AddToClassList(UssClassName);
			this.SetVisualInput(_vectorField);
			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
		}

		public override void SetValueWithoutNotify(Quaternion newValue)
		{
			base.SetValueWithoutNotify(newValue);
			_vectorField.SetValueWithoutNotify(newValue.eulerAngles);
		}

		#endregion

		#region UXML Support

		public EulerField() : this(null) {}

		public new class UxmlFactory : UxmlFactory<EulerField, UxmlTraits> { }

		public new class UxmlTraits : BaseField<Quaternion>.UxmlTraits
		{
			private readonly UxmlFloatAttributeDescription _x = new UxmlFloatAttributeDescription { name = "x", defaultValue = 0.0f };
			private readonly UxmlFloatAttributeDescription _y = new UxmlFloatAttributeDescription { name = "y", defaultValue = 0.0f };
			private readonly UxmlFloatAttributeDescription _z = new UxmlFloatAttributeDescription { name = "z", defaultValue = 0.0f };

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);

				var euler = ve as EulerField;
				var x = _x.GetValueFromBag(bag, cc);
				var y = _y.GetValueFromBag(bag, cc);
				var z = _z.GetValueFromBag(bag, cc);

				euler.SetValueWithoutNotify(Quaternion.Euler(x, y, z));
			}
		}

		#endregion
	}
}