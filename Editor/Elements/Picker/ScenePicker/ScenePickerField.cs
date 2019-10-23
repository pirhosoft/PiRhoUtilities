using System;
using UnityEditor;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class ScenePickerField : BaseField<AssetReference>
	{
		public static readonly string UssClassName = "pirho-scene-picker-field";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string InputUssClassName = UssClassName + "__input";

		public ScenePickerControl Control { get; private set; }

		public ScenePickerField(SerializedProperty property, Action onCreate) : this(property.displayName, property.GetObject<AssetReference>(), onCreate)
		{
			this.ConfigureProperty(property, property.GetTooltip());
		}

		public ScenePickerField(string label, AssetReference value, Action onCreate) : base(label, null)
		{
			Setup(value, onCreate);
		}

		private void Setup(AssetReference value, Action onCreate)
		{
			Control = new ScenePickerControl(value, onCreate);
			Control.AddToClassList(InputUssClassName);
			Control.RegisterCallback<ChangeEvent<AssetReference>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(Control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value);
		}

		public override void SetValueWithoutNotify(AssetReference newValue)
		{
			base.SetValueWithoutNotify(newValue);
			Control.SetValueWithoutNotify(newValue);
		}

		#region UXML Support

		public ScenePickerField() : base(null, null) { }

		public new class UxmlFactory : UxmlFactory<ScenePickerField, UxmlTraits> { }

		public new class UxmlTraits : BaseField<string>.UxmlTraits
		{
			private UxmlStringAttributeDescription _path = new UxmlStringAttributeDescription { name = "path" };

			public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(element, bag, cc);

				var field = element as ScenePickerField;
				var path = _path.GetValueFromBag(bag, cc);
				var guid = AssetDatabase.AssetPathToGUID(path);

				field.Setup(new AssetReference(guid), null);
			}
		}

		#endregion
	}
}
