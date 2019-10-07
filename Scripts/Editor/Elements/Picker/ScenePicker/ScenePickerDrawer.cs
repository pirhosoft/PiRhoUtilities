using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(ScenePickerAttribute))]
	public class ScenePickerDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUSPDIT) invalid type for ScenePickerAttribute on field {0}: ScenePicker can only be applied to an AssetReference";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var tooltip = this.GetTooltip();
			var method = fieldInfo.DeclaringType.GetMethod((attribute as ScenePickerAttribute)?.CreateMethod, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			void onCreate() => method?.Invoke(method.IsStatic ? null : property.serializedObject.targetObject, null);

			if (this.GetFieldType() == typeof(AssetReference))
			{
				var scene = property.GetObject<AssetReference>();
				var picker = new ScenePickerField(property.displayName, scene, onCreate);
				return picker.ConfigureProperty(property, tooltip);
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
			}

			return new FieldContainer(property.displayName);
		}
	}
}
