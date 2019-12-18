using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(ChangeTriggerAttribute))]
	class ChangeTriggerDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUCTDIT) invalid type for ChangeTriggerAttribute on field '{0}': ChangeTrigger can only be applied to serializable fields";
		private const string _invalidMethodWarning = "(PUCTDIM) invalid method for ChangeTriggerAttribute on field '{0}': the method '{1}' should take 0, 1, or 2 parameters of type '{2}'";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var element = this.CreateNextElement(property);
			var changeAttribute = attribute as ChangeTriggerAttribute;
			var change = CreateControl(property, fieldInfo.DeclaringType, changeAttribute.Method);

			if (change != null)
				element.Add(change);

			return element;
		}

		private PropertyWatcher CreateControl(SerializedProperty property, Type declaringType, string method)
		{
			switch (property.propertyType)
			{
				case SerializedPropertyType.Integer: return CreateControl<int>(property, declaringType, method);
				case SerializedPropertyType.Boolean: return CreateControl<bool>(property, declaringType, method);
				case SerializedPropertyType.Float: return CreateControl<float>(property, declaringType, method);
				case SerializedPropertyType.String: return CreateControl<string>(property, declaringType, method);
				case SerializedPropertyType.Color: return CreateControl<Color>(property, declaringType, method);
				case SerializedPropertyType.ObjectReference: return CreateControl<Object>(property, declaringType, method);
				case SerializedPropertyType.LayerMask: return CreateControl<int>(property, declaringType, method);
				case SerializedPropertyType.Enum: return CreateControl<Enum>(property, declaringType, method);
				case SerializedPropertyType.Vector2: return CreateControl<Vector2>(property, declaringType, method);
				case SerializedPropertyType.Vector3: return CreateControl<Vector3>(property, declaringType, method);
				case SerializedPropertyType.Vector4: return CreateControl<Vector4>(property, declaringType, method);
				case SerializedPropertyType.Rect: return CreateControl<Rect>(property, declaringType, method);
				case SerializedPropertyType.ArraySize: return CreateControl<int>(property, declaringType, method);
				case SerializedPropertyType.Character: return CreateControl<char>(property, declaringType, method);
				case SerializedPropertyType.AnimationCurve: return CreateControl<AnimationCurve>(property, declaringType, method);
				case SerializedPropertyType.Bounds: return CreateControl<Bounds>(property, declaringType, method);
				case SerializedPropertyType.Gradient: return CreateControl<Gradient>(property, declaringType, method);
				case SerializedPropertyType.Quaternion: return CreateControl<Quaternion>(property, declaringType, method);
				case SerializedPropertyType.ExposedReference: return CreateControl<Object>(property, declaringType, method);
				case SerializedPropertyType.FixedBufferSize: return CreateControl<int>(property, declaringType, method);
				case SerializedPropertyType.Vector2Int: return CreateControl<Vector2Int>(property, declaringType, method);
				case SerializedPropertyType.Vector3Int: return CreateControl<Vector3Int>(property, declaringType, method);
				case SerializedPropertyType.RectInt: return CreateControl<RectInt>(property, declaringType, method);
				case SerializedPropertyType.BoundsInt: return CreateControl<BoundsInt>(property, declaringType, method);
				case SerializedPropertyType.ManagedReference: return CreateControl<object>(property, declaringType, method);
			}

			Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath, this.GetFieldType().Name);
			return null;
		}

		private PropertyWatcher CreateControl<T>(SerializedProperty property, Type declaringType, string method)
		{
			var none = ReflectionHelper.CreateActionCallback(property, declaringType, method, nameof(ChangeTriggerAttribute), nameof(ChangeTriggerAttribute.Method));
			if (none != null)
			{
				return new ChangeTrigger<T>(property, (_, oldValue, newValue) =>
				{
					if (!EditorApplication.isPlaying)
						none();;
				});
			}
			else
			{
				var one = ReflectionHelper.CreateActionCallback<T>(property, declaringType, method, nameof(ChangeTriggerAttribute), nameof(ChangeTriggerAttribute.Method));
				if (one != null)
				{
					return new ChangeTrigger<T>(property, (_, oldValue, newValue) =>
					{
						if (!EditorApplication.isPlaying)
							one(newValue);
					});
				}
				else
				{
					var two = ReflectionHelper.CreateActionCallback<T, T>(property, declaringType, method, nameof(ChangeTriggerAttribute), nameof(ChangeTriggerAttribute.Method));
					if (two != null)
					{
						return new ChangeTrigger<T>(property, (_, oldValue, newValue) =>
						{
							if (!EditorApplication.isPlaying)
								two(oldValue, newValue);
						});
					}
				}
			}

			Debug.LogWarningFormat(_invalidMethodWarning, property.propertyPath, method, typeof(T).Name);
			return null;
		}
	}
}