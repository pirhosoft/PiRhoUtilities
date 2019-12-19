using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Samples
{
	public class ChangeTriggerUxmlSample : UxmlSample
	{
		// This uses the same classes defined in ChangeTriggerCodeSample

		public override void Setup(VisualElement root)
		{
			var asset = ScriptableObject.CreateInstance<ChangeTriggerCodeSample.SampleAsset>();
			var obj = new SerializedObject(asset);

			root.Q<ChangeTriggerInt>("int-trigger").SetAction(PropertyChanged);
			root.Q<ChangeTriggerBool>("bool-trigger").SetAction(PropertyChanged);
			root.Q<ChangeTriggerFloat>("float-trigger").SetAction(PropertyChanged);
			root.Q<ChangeTriggerString>("string-trigger").SetAction(PropertyChanged);
			root.Q<ChangeTriggerColor>("color-trigger").SetAction(PropertyChanged);
			root.Q<ChangeTriggerObject>("object-trigger").SetAction(PropertyChanged);
			root.Q<ChangeTriggerLayerMask>("layer-mask-trigger").SetAction(PropertyChanged);
			root.Q<ChangeTriggerEnum>("enum-trigger").SetAction(PropertyChanged);
			root.Q<ChangeTriggerVector2>("vector2-trigger").SetAction(PropertyChanged);
			root.Q<ChangeTriggerVector3>("vector3-trigger").SetAction(PropertyChanged);
			root.Q<ChangeTriggerVector4>("vector4-trigger").SetAction(PropertyChanged);
			root.Q<ChangeTriggerRect>("rect-trigger").SetAction(PropertyChanged);
			root.Q<ChangeTriggerArraySize>("array-size-trigger").SetAction(PropertyChanged);
			root.Q<ChangeTriggerCharacter>("character-trigger").SetAction(PropertyChanged);
			root.Q<ChangeTriggerAnimationCurve>("animation-curve-trigger").SetAction(PropertyChanged);
			root.Q<ChangeTriggerBounds>("bounds-trigger").SetAction(PropertyChanged);
			root.Q<ChangeTriggerGradient>("gradient-trigger").SetAction(PropertyChanged);
			root.Q<ChangeTriggerQuaternion>("quaternion-trigger").SetAction(PropertyChanged);
			root.Q<ChangeTriggerVector2Int>("vector2-int-trigger").SetAction(PropertyChanged);
			root.Q<ChangeTriggerVector3Int>("vector3-int-trigger").SetAction(PropertyChanged);
			root.Q<ChangeTriggerRectInt>("rect-int-trigger").SetAction(PropertyChanged);
			root.Q<ChangeTriggerBoundsInt>("bounds-int-trigger").SetAction(PropertyChanged);
			root.Q<ChangeTriggerReference>("reference-trigger").SetAction(PropertyChanged);

			root.Bind(obj);
		}

		private void PropertyChanged<T>(SerializedProperty property, T oldValue, T newValue)
		{
			Debug.Log($"{property.propertyPath} changed from {oldValue} to {newValue}");
		}
	}
}
