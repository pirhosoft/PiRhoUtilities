using System;
using UnityEditor;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class AssetPickerField : BaseField<AssetReference>
	{
		public const string UssClassName = "pirho-scene-picker-field";
		public const string LabelUssClassName = UssClassName + "__label";
		public const string InputUssClassName = UssClassName + "__input";

		public AssetPickerControl Control { get; private set; }

		public AssetPickerField(SerializedProperty property, Type type, string tag = null) : this(property.displayName, property.GetObject<AssetReference>(), type, tag)
		{
			this.ConfigureProperty(property);
		}

		public AssetPickerField(SerializedProperty property, Type type, string[] tags, Addressables.MergeMode mode) : this(property.displayName, property.GetObject<AssetReference>(), type, tags, mode)
		{
			this.ConfigureProperty(property);
		}

		public AssetPickerField(string label, AssetReference value, Type type, string tag = null) : base(label, null)
		{
			Setup(value, type, tag, null, Addressables.MergeMode.None);
		}

		public AssetPickerField(string label, AssetReference value, Type type, string[] tags, Addressables.MergeMode mode) : base(label, null)
		{
			Setup(value, type, null, tags, mode);
		}

		private void Setup(AssetReference value, Type type, string tag, string[] tags, Addressables.MergeMode mode)
		{
			Control = tags == null ? new AssetPickerControl(value, type, tag) : new AssetPickerControl(value, type, tags, mode);
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

		public AssetPickerField() : base(null, null) { }

		public new class UxmlFactory : UxmlFactory<ScenePickerField, UxmlTraits> { }

		public new class UxmlTraits : BaseField<string>.UxmlTraits
		{
			private UxmlStringAttributeDescription _path = new UxmlStringAttributeDescription { name = "path" };

			public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(element, bag, cc);

				var field = element as AssetPickerField;
				var path = _path.GetValueFromBag(bag, cc);
				var guid = AssetDatabase.AssetPathToGUID(path);

				field.Setup(new AssetReference(guid), null, null, null, Addressables.MergeMode.None);
			}
		}

		#endregion
	}
}
