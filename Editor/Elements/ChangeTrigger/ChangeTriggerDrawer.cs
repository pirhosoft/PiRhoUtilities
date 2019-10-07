using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(ChangeTriggerAttribute))]
	class ChangeTriggerDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUCTDIT) invalid type for ChangeTriggerAttribute on field '{0}': ChangeTrigger cannot be applied to '{1}' fields";
		private const string _missingMethodWarning = "(PUCTDMM) invalid method for ChangeTriggerAttribute on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _invalidMethodWarning = "(PUCTDIM) invalid method for ChangeTriggerAttribute on field '{0}': the method '{1}' should take 0, 1, or 2 parameters of type '{2}'";

		private static readonly object[] _oneParameter = new object[1];
		private static readonly object[] _twoParameters = new object[2];

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var element = this.CreateNextElement(property);

			var change = attribute as ChangeTriggerAttribute;
			var method = fieldInfo.DeclaringType.GetMethod(change.Method, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			if (method != null)
			{
				var owner = method.IsStatic ? null : property.GetOwner<object>();
				var type = this.GetFieldType();
				var control = CreateControl(property, type, method, owner);

				if (control != null)
					element.Add(control);
			}
			else
			{
				Debug.LogWarningFormat(_missingMethodWarning, property.propertyPath, change.Method, fieldInfo.DeclaringType.Name);
			}

			return element;
		}

		private PropertyWatcher CreateControl(SerializedProperty property, Type type, MethodInfo method, object owner)
		{
			switch (property.propertyType)
			{
				case SerializedPropertyType.Generic: return InvalidType(property);
				case SerializedPropertyType.Integer: return CreateControl(property, property.intValue, method, owner);
				case SerializedPropertyType.Boolean: return CreateControl(property, property.boolValue, method, owner);
				case SerializedPropertyType.Float: return CreateControl(property, property.floatValue, method, owner);
				case SerializedPropertyType.String: return CreateControl(property, property.stringValue, method, owner);
				case SerializedPropertyType.Color: return CreateControl(property, property.colorValue, method, owner);
				case SerializedPropertyType.ObjectReference: return CreateControl(property, property.objectReferenceValue, method, owner);
				case SerializedPropertyType.LayerMask: return CreateControl(property, property.intValue, method, owner);
				case SerializedPropertyType.Enum: return CreateControl(property, Enum.ToObject(type, property.intValue) as Enum, method, owner);
				case SerializedPropertyType.Vector2: return CreateControl(property, property.vector2Value, method, owner);
				case SerializedPropertyType.Vector3: return CreateControl(property, property.vector3Value, method, owner);
				case SerializedPropertyType.Vector4: return CreateControl(property, property.vector4Value, method, owner);
				case SerializedPropertyType.Rect: return CreateControl(property, property.rectValue, method, owner);
				case SerializedPropertyType.ArraySize: return CreateControl(property, property.intValue, method, owner);
				case SerializedPropertyType.Character: return CreateControl(property, property.intValue, method, owner);
				case SerializedPropertyType.AnimationCurve: return CreateControl(property, property.animationCurveValue, method, owner);
				case SerializedPropertyType.Bounds: return CreateControl(property, property.boundsValue, method, owner);
				case SerializedPropertyType.Gradient: return CreateControl(property, property.GetGradientValue(), method, owner);
				case SerializedPropertyType.Quaternion: return CreateControl(property, property.quaternionValue, method, owner);
				case SerializedPropertyType.ExposedReference: return CreateControl(property, property.exposedReferenceValue, method, owner);
				case SerializedPropertyType.FixedBufferSize: return CreateControl(property, property.intValue, method, owner);
				case SerializedPropertyType.Vector2Int: return CreateControl(property, property.vector2IntValue, method, owner);
				case SerializedPropertyType.Vector3Int: return CreateControl(property, property.vector3IntValue, method, owner);
				case SerializedPropertyType.RectInt: return CreateControl(property, property.rectIntValue, method, owner);
				case SerializedPropertyType.BoundsInt: return CreateControl(property, property.boundsIntValue, method, owner);
			}

			return InvalidType(property);
		}

		private PropertyWatcher CreateControl<T>(SerializedProperty property, T value, MethodInfo method, object owner)
		{
			if (method.HasSignature(null))
				return new ChangeTriggerControl<T>(property, (oldValue, newValue) => OnChanged(method, owner));
			else if (method.HasSignature(null, typeof(T)))
				return new ChangeTriggerControl<T>(property, (oldValue, newValue) => OnChanged(newValue, method, owner));
			else if (method.HasSignature(null, typeof(T), typeof(T)))
				return new ChangeTriggerControl<T>(property, (oldValue, newValue) => OnChanged(oldValue, newValue, method, owner));

			Debug.LogWarningFormat(_invalidMethodWarning, property.propertyPath, method.Name, typeof(T).Name);
			return null;
		}

		private PropertyWatcher InvalidType(SerializedProperty property)
		{
			Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath, this.GetFieldType().Name);
			return null;
		}

		private static void OnChanged(MethodInfo method, object owner)
		{
			if (!EditorApplication.isPlaying)
				method.Invoke(owner, null);
		}

		private static void OnChanged<T>(T newValue, MethodInfo method, object owner)
		{
			if (!EditorApplication.isPlaying)
			{
				_oneParameter[0] = newValue;
				method.Invoke(owner, _oneParameter);
			}
		}

		private static void OnChanged<T>(T oldValue, T newValue, MethodInfo method, object owner)
		{
			if (!EditorApplication.isPlaying)
			{
				_twoParameters[0] = oldValue;
				_twoParameters[1] = newValue;
				method.Invoke(owner, _twoParameters);
			}
		}
	}
}