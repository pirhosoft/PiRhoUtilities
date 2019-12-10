using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(AssetPickerAttribute))]
	public class AssetPickerDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUAPDIT) invalid type for AssetPickerAttribute on field {0}: AssetPicker can only be applied to an AssetReference";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var assetAttribute = attribute as AssetPickerAttribute;

			if (this.GetFieldType() == typeof(AssetReference))
				return assetAttribute.Tags == null ? new AssetPickerField(property, assetAttribute.Type, assetAttribute.Tag) : new AssetPickerField(property, assetAttribute.Type, assetAttribute.Tags, assetAttribute.Mode);
			else
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);

			return new FieldContainer(property.displayName);
		}
	}
}
