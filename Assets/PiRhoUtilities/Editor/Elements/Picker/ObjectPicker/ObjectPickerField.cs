using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Editor
{
	public class ObjectPickerField : BaseField<Object>
	{
		public static readonly string UssClassName = "pirho-object-picker-field";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string InputUssClassName = UssClassName + "__input";

		private ObjectPickerControl _control;

		public ObjectPickerField(string label, Object value, Object owner, Type type) : base(label, null)
		{
			Setup(value, owner, type);
		}

		private void Setup(Object value, Object owner, Type type)
		{
			_control = new ObjectPickerControl(value, owner, type);
			_control.AddToClassList(InputUssClassName);
			_control.RegisterCallback<ChangeEvent<Object>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(_control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value);
		}

		public override void SetValueWithoutNotify(Object newValue)
		{
			base.SetValueWithoutNotify(newValue);
			_control.SetValueWithoutNotify(newValue);
		}

		#region UXML Support

		private const string _invalidTypeError = "(PUOPFIT) failed to setup ObjectPickerField: the type '{0}' could not be found";
		private const string _invalidValueWarning = "(PUOPFIT) failed to set ObjectPickerField value: '{0}' is not a object of type '{1}'";

		public ObjectPickerField() : base(null, null) { }

		public new class UxmlFactory : UxmlFactory<ObjectPickerField, UxmlTraits> { }

		public new class UxmlTraits : BaseField<Object>.UxmlTraits
		{
			private UxmlStringAttributeDescription _type = new UxmlStringAttributeDescription { name = "type", use = UxmlAttributeDescription.Use.Required };
			private UxmlStringAttributeDescription _value = new UxmlStringAttributeDescription { name = "value" };

			public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(element, bag, cc);

				var field = element as ObjectPickerField;
				var typeName = _type.GetValueFromBag(bag, cc);
				var type = Type.GetType(typeName, false);

				if (type == null)
				{
					Debug.LogErrorFormat(_invalidTypeError, typeName);
				}
				else
				{
					var valueName = _value.GetValueFromBag(bag, cc);
					var value = AssetDatabase.LoadAssetAtPath(valueName, type);

					field.Setup(value, null, type);
				}
			}
		}

		#endregion
	}
}
