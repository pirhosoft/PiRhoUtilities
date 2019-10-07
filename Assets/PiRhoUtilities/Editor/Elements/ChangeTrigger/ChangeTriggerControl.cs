using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Editor
{
	public static class ChangeTriggerControl
	{
		public static PropertyWatcher Create(SerializedProperty property, Action onChanged)
		{
			switch (property.propertyType)
			{
				case SerializedPropertyType.Generic: return null;
				case SerializedPropertyType.Integer: return new ChangeTriggerControl<int>(property, (oldValue, newValue) => onChanged());
				case SerializedPropertyType.Boolean: return new ChangeTriggerControl<bool>(property, (oldvalue, newValue) => onChanged());
				case SerializedPropertyType.Float: return new ChangeTriggerControl<float>(property, (oldvalue, newValue) => onChanged());
				case SerializedPropertyType.String: return new ChangeTriggerControl<string>(property, (oldvalue, newValue) => onChanged());
				case SerializedPropertyType.Color: return new ChangeTriggerControl<Color>(property, (oldvalue, newValue) => onChanged());
				case SerializedPropertyType.ObjectReference: return new ChangeTriggerControl<Object>(property, (oldvalue, newValue) => onChanged());
				case SerializedPropertyType.LayerMask: return new ChangeTriggerControl<LayerMask>(property, (oldvalue, newValue) => onChanged());
				case SerializedPropertyType.Enum: return new ChangeTriggerControl<Enum>(property, (oldvalue, newValue) => onChanged());
				case SerializedPropertyType.Vector2: return new ChangeTriggerControl<Vector2>(property, (oldvalue, newValue) => onChanged());
				case SerializedPropertyType.Vector3: return new ChangeTriggerControl<Vector3>(property, (oldvalue, newValue) => onChanged());
				case SerializedPropertyType.Vector4: return new ChangeTriggerControl<Vector4>(property, (oldvalue, newValue) => onChanged());
				case SerializedPropertyType.Rect: return new ChangeTriggerControl<Rect>(property, (oldvalue, newValue) => onChanged());
				case SerializedPropertyType.ArraySize: return new ChangeTriggerControl<int>(property, (oldvalue, newValue) => onChanged());
				case SerializedPropertyType.Character: return new ChangeTriggerControl<int>(property, (oldvalue, newValue) => onChanged());
				case SerializedPropertyType.AnimationCurve: return new ChangeTriggerControl<AnimationCurve>(property, (oldvalue, newValue) => onChanged());
				case SerializedPropertyType.Bounds: return new ChangeTriggerControl<Bounds>(property, (oldvalue, newValue) => onChanged());
				case SerializedPropertyType.Gradient: return new ChangeTriggerControl<Gradient>(property, (oldvalue, newValue) => onChanged());
				case SerializedPropertyType.Quaternion: return new ChangeTriggerControl<Quaternion>(property, (oldvalue, newValue) => onChanged());
				case SerializedPropertyType.ExposedReference: return new ChangeTriggerControl<Object>(property, (oldvalue, newValue) => onChanged());
				case SerializedPropertyType.FixedBufferSize: return null;
				case SerializedPropertyType.Vector2Int: return new ChangeTriggerControl<Vector2Int>(property, (oldvalue, newValue) => onChanged());
				case SerializedPropertyType.Vector3Int: return new ChangeTriggerControl<Vector3Int>(property, (oldvalue, newValue) => onChanged());
				case SerializedPropertyType.RectInt: return new ChangeTriggerControl<RectInt>(property, (oldvalue, newValue) => onChanged());
				case SerializedPropertyType.BoundsInt: return new ChangeTriggerControl<BoundsInt>(property, (oldvalue, newValue) => onChanged());
			}

			return null;
		}
	}

	public class ChangeTriggerControl<T> : PropertyWatcher<T>
	{
		private readonly Action<T, T> _onChanged;

		public ChangeTriggerControl(SerializedProperty property, Action<T, T> onChanged) : base(null)
		{
			_onChanged = onChanged;

			// binding needs to be set up after _onChanged is assigned in case the change is triggered immediately
			// TODO: this probably indicates a problem somewhere else since the value should be initialized to the
			// current value
			Watch(property);
		}

		protected override void OnChanged(T previousValue, T newValue)
		{
			_onChanged(previousValue, newValue);
		}
	}
}