using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Editor
{
	public static class ChangeTrigger
	{
		public static PropertyWatcher Create(SerializedProperty property, Action<SerializedProperty> onChanged)
		{
			switch (property.propertyType)
			{
				case SerializedPropertyType.Generic: return null;
				case SerializedPropertyType.Integer: return CreateTrigger<int>(property, onChanged);
				case SerializedPropertyType.Boolean: return CreateTrigger<bool>(property, onChanged);
				case SerializedPropertyType.Float: return CreateTrigger<float>(property, onChanged);
				case SerializedPropertyType.String: return CreateTrigger<string>(property, onChanged);
				case SerializedPropertyType.Color: return CreateTrigger<Color>(property, onChanged);
				case SerializedPropertyType.ObjectReference: return CreateTrigger<Object>(property, onChanged);
				case SerializedPropertyType.LayerMask: return CreateTrigger<LayerMask>(property, onChanged);
				case SerializedPropertyType.Enum: return CreateTrigger<Enum>(property, onChanged);
				case SerializedPropertyType.Vector2: return CreateTrigger<Vector2>(property, onChanged);
				case SerializedPropertyType.Vector3: return CreateTrigger<Vector3>(property, onChanged);
				case SerializedPropertyType.Vector4: return CreateTrigger<Vector4>(property, onChanged);
				case SerializedPropertyType.Rect: return CreateTrigger<Rect>(property, onChanged);
				case SerializedPropertyType.ArraySize: return CreateTrigger<int>(property, onChanged);
				case SerializedPropertyType.Character: return CreateTrigger<int>(property, onChanged);
				case SerializedPropertyType.AnimationCurve: return CreateTrigger<AnimationCurve>(property, onChanged);
				case SerializedPropertyType.Bounds: return CreateTrigger<Bounds>(property, onChanged);
				case SerializedPropertyType.Gradient: return CreateTrigger<Gradient>(property, onChanged);
				case SerializedPropertyType.Quaternion: return CreateTrigger<Quaternion>(property, onChanged);
				case SerializedPropertyType.ExposedReference: return CreateTrigger<Object>(property, onChanged);
				case SerializedPropertyType.FixedBufferSize: return null;
				case SerializedPropertyType.Vector2Int: return CreateTrigger<Vector2Int>(property, onChanged);
				case SerializedPropertyType.Vector3Int: return CreateTrigger<Vector3Int>(property, onChanged);
				case SerializedPropertyType.RectInt: return CreateTrigger<RectInt>(property, onChanged);
				case SerializedPropertyType.BoundsInt: return CreateTrigger<BoundsInt>(property, onChanged);
				case SerializedPropertyType.ManagedReference: return CreateTrigger<object>(property, onChanged);
			}

			return null;
		}

		private static PropertyWatcher CreateTrigger<T>(SerializedProperty property, Action<SerializedProperty> onChanged)
		{
			var trigger = new ChangeTrigger<T>();
			trigger.Watch(property);
			trigger.SetAction(onChanged);
			return trigger;
		}
	}

	public class ChangeTrigger<T> : PropertyWatcher<T>
	{
		private Action<SerializedProperty, T, T> _onChanged;

		public ChangeTrigger()
		{
		}

		public ChangeTrigger(SerializedProperty property, Action<SerializedProperty, T, T> onChanged)
		{
			Watch(property);
			SetAction(onChanged);
		}

		public void SetAction(Action action)
		{
			_onChanged = (property, oldValue, newValue) => action();
		}

		public void SetAction(Action<T> action)
		{
			_onChanged = (property, oldValue, newValue) => action(newValue);
		}

		public void SetAction(Action<T, T> action)
		{
			_onChanged = (property, oldValue, newValue) => action(oldValue, newValue);
		}

		public void SetAction(Action<SerializedProperty> action)
		{
			_onChanged = (property, oldValue, newValue) => action(property);
		}

		public void SetAction(Action<SerializedProperty, T> action)
		{
			_onChanged = (property, oldValue, newValue) => action(property, newValue);
		}

		public void SetAction(Action<SerializedProperty, T, T> action)
		{
			_onChanged = action;
		}

		protected override void OnChanged(SerializedProperty property, T previousValue, T newValue)
		{
			_onChanged?.Invoke(property, previousValue, newValue);
		}
	}

	#region Uxml Implementations

	// While these can be used from code as well they are only necessary for uxml.

	public class ChangeTriggerInt : ChangeTrigger<int> { public new class UxmlFactory : UxmlFactory<ChangeTriggerInt, UxmlTraits> { } }
	public class ChangeTriggerBool : ChangeTrigger<bool> { public new class UxmlFactory : UxmlFactory<ChangeTriggerBool, UxmlTraits> { } }
	public class ChangeTriggerFloat : ChangeTrigger<float> { public new class UxmlFactory : UxmlFactory<ChangeTriggerFloat, UxmlTraits> { } }
	public class ChangeTriggerString : ChangeTrigger<string> { public new class UxmlFactory : UxmlFactory<ChangeTriggerString, UxmlTraits> { } }
	public class ChangeTriggerColor : ChangeTrigger<Color> { public new class UxmlFactory : UxmlFactory<ChangeTriggerColor, UxmlTraits> { } }
	public class ChangeTriggerObject : ChangeTrigger<Object> { public new class UxmlFactory : UxmlFactory<ChangeTriggerObject, UxmlTraits> { } }
	public class ChangeTriggerLayerMask : ChangeTrigger<int> { public new class UxmlFactory : UxmlFactory<ChangeTriggerLayerMask, UxmlTraits> { } }
	public class ChangeTriggerEnum : ChangeTrigger<Enum> { public new class UxmlFactory : UxmlFactory<ChangeTriggerEnum, UxmlTraits> { } }
	public class ChangeTriggerVector2 : ChangeTrigger<Vector2> { public new class UxmlFactory : UxmlFactory<ChangeTriggerVector2, UxmlTraits> { } }
	public class ChangeTriggerVector3 : ChangeTrigger<Vector3> { public new class UxmlFactory : UxmlFactory<ChangeTriggerVector3, UxmlTraits> { } }
	public class ChangeTriggerVector4 : ChangeTrigger<Vector4> { public new class UxmlFactory : UxmlFactory<ChangeTriggerVector4, UxmlTraits> { } }
	public class ChangeTriggerRect : ChangeTrigger<Rect> { public new class UxmlFactory : UxmlFactory<ChangeTriggerRect, UxmlTraits> { } }
	public class ChangeTriggerArraySize : ChangeTrigger<int> { public new class UxmlFactory : UxmlFactory<ChangeTriggerArraySize, UxmlTraits> { } }
	public class ChangeTriggerCharacter : ChangeTrigger<char> { public new class UxmlFactory : UxmlFactory<ChangeTriggerCharacter, UxmlTraits> { } }
	public class ChangeTriggerAnimationCurve : ChangeTrigger<AnimationCurve> { public new class UxmlFactory : UxmlFactory<ChangeTriggerAnimationCurve, UxmlTraits> { } }
	public class ChangeTriggerBounds : ChangeTrigger<Bounds> { public new class UxmlFactory : UxmlFactory<ChangeTriggerBounds, UxmlTraits> { } }
	public class ChangeTriggerGradient : ChangeTrigger<Gradient> { public new class UxmlFactory : UxmlFactory<ChangeTriggerGradient, UxmlTraits> { } }
	public class ChangeTriggerQuaternion : ChangeTrigger<Quaternion> { public new class UxmlFactory : UxmlFactory<ChangeTriggerQuaternion, UxmlTraits> { } }
	public class ChangeTriggerVector2Int : ChangeTrigger<Vector2Int> { public new class UxmlFactory : UxmlFactory<ChangeTriggerVector2Int, UxmlTraits> { } }
	public class ChangeTriggerVector3Int : ChangeTrigger<Vector3Int> { public new class UxmlFactory : UxmlFactory<ChangeTriggerVector3Int, UxmlTraits> { } }
	public class ChangeTriggerRectInt : ChangeTrigger<RectInt> { public new class UxmlFactory : UxmlFactory<ChangeTriggerRectInt, UxmlTraits> { } }
	public class ChangeTriggerBoundsInt : ChangeTrigger<BoundsInt> { public new class UxmlFactory : UxmlFactory<ChangeTriggerBoundsInt, UxmlTraits> { } }
	public class ChangeTriggerReference : ChangeTrigger<object> { public new class UxmlFactory : UxmlFactory<ChangeTriggerReference, UxmlTraits> { } }

	#endregion
}
