using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Editor
{
	public static class ReflectionHelper
	{
		#region Log Warnings

		private const string _missingSourceWarning = "(PURHMS) invalid '{0}' for '{1}' on field '{2}': '{3}' could not be found on type '{4}'";
		private const string _invalidSiblingWarning = "(PURHIPR) invalid '{0}' sibling for '{1}' on field '{2}': the SerializedProperty '{3}' should be a {4}";
		private const string _invalidMethodReturnWarning = "(PURHMR) invalid '{0}' method for '{1}' on field '{2}': the method '{3}' should return a '{4}'";
		private const string _invalidMethodParametersWarning = "(PURHIMP) invalid '{0}' method for '{1}' on field '{2}': the method '{3}' has invalid parameters";
		private const string _invalidPropertyWarning = "(PURHIPR) invalid '{0}' property for '{1}' on field '{2}': the property '{3}' should be a readable {4}";
		private const string _invalidFieldWarning = "(PURHIFR) invalid '{0}' field for '{1}' on field '{2}': the field '{3}' should be a {4}";

		#endregion

		private static readonly object[] _oneParameter = new object[1];
		private static readonly object[] _twoParameters = new object[2];

		public static void SetupValueSourceCallback<FieldType>(string sourceName, SerializedProperty property, VisualElement element, Type declaringType, FieldType defaultValue, bool autoUpdate, string attributeName, string sourceParameterName, Action<FieldType> updateAction)
		{
			if (!string.IsNullOrEmpty(sourceName))
			{
				var valueFunction = GetValueFunction(property, declaringType, sourceName, attributeName, sourceParameterName, element, out var needsSchedule, updateAction);
				if (valueFunction != null)
					SetupSchedule(element, () => updateAction(valueFunction()), autoUpdate && needsSchedule);
			}
			else
			{
				updateAction(defaultValue);
			}
		}

		public static Func<FieldType> CreateValueSourceFunction<FieldType>(SerializedProperty property, VisualElement element, Type declaringType, string sourceName, FieldType defaultValue, string attributeName, string sourceParameterName)
		{
			if (!string.IsNullOrEmpty(sourceName))
				return GetValueFunction<FieldType>(property, declaringType, sourceName, attributeName, sourceParameterName, element, out var _, null);

			return () => defaultValue;
		}

		public static Func<FieldType> CreateFunctionCallback<FieldType>(SerializedProperty property, Type declaringType, string sourceName, string attributeName, string sourceParameterName)
		{
			var found = false;
			if (!string.IsNullOrEmpty(sourceName))
				return GetValueFromMethodFunction<FieldType>(declaringType, sourceName, property, attributeName, sourceParameterName, ref found, false);

			return null;
		}

		public static Func<ParameterOne, FieldType> CreateFunctionCallback<ParameterOne, FieldType>(SerializedProperty property, Type declaringType, string sourceName, string attributeName, string sourceParameterName)
		{
			var found = false;

			if (!string.IsNullOrEmpty(sourceName))
				return GetValueFromMethodFunction<ParameterOne, FieldType>(declaringType, sourceName, property, attributeName, sourceParameterName, ref found, false);

			return null;
		}

		public static Func<ParameterOne, ParameterTwo, FieldType> CreateFunctionCallback<ParameterOne, ParameterTwo, FieldType>(SerializedProperty property, Type declaringType, string sourceName, string attributeName, string sourceParameterName)
		{
			var found = false;

			if (!string.IsNullOrEmpty(sourceName))
				return GetValueFromMethodFunction<ParameterOne, ParameterTwo, FieldType>(declaringType, sourceName, property, attributeName, sourceParameterName, ref found, false);

			return null;
		}

		public static Action CreateActionCallback(SerializedProperty property, Type declaringType, string sourceName, string attributeName, string sourceParameterName)
		{
			if (!string.IsNullOrEmpty(sourceName))
				return GetCallback(declaringType, sourceName, property, attributeName, sourceParameterName);

			return null;
		}

		public static Action<ParameterOne> CreateActionCallback<ParameterOne>(SerializedProperty property, Type declaringType, string sourceName, string attributeName, string sourceParameterName)
		{
			if (!string.IsNullOrEmpty(sourceName))
				return GetCallback<ParameterOne>(declaringType, sourceName, property, attributeName, sourceParameterName);

			return null;
		}

		public static Action<ParameterOne, ParameterTwo> CreateActionCallback<ParameterOne, ParameterTwo>(SerializedProperty property, Type declaringType, string sourceName, string attributeName, string sourceParameterName)
		{
			if (!string.IsNullOrEmpty(sourceName))
				return GetCallback<ParameterOne, ParameterTwo>(declaringType, sourceName, property, attributeName, sourceParameterName);

			return null;
		}

		private static Func<FieldType> GetValueFunction<FieldType>(SerializedProperty property, Type declaringType, string sourceName, string attributeName, string sourceParameterName, VisualElement element, out bool needsSchedule, Action<FieldType> updateAction)
		{
			needsSchedule = true;
			var found = false;

			{ // Property
				var changeTrigger = GetSerializedPropertyTrigger(property, sourceName, attributeName, sourceParameterName, ref found, updateAction);
				if (changeTrigger != null)
				{
					needsSchedule = false;
					element.Add(changeTrigger);
					return () => changeTrigger.value;
				}
			}

			{ // Method
				var valueFunction = GetValueFromMethodFunction<FieldType>(declaringType, sourceName, property, attributeName, sourceParameterName, ref found, true);
				if (valueFunction != null)
					return valueFunction;
			}

			{ // Property
				var valueFunction = GetValueFromPropertyFunction<FieldType>(declaringType, sourceName, property, attributeName, sourceParameterName, ref found);
				if (valueFunction != null)
					return valueFunction;
			}

			{ // Field
				var valueFunction = GetValueFromFieldFunction<FieldType>(declaringType, sourceName, property, attributeName, sourceParameterName, ref found, ref needsSchedule);
				if (valueFunction != null)
					return valueFunction;
			}

			if (!found)
				Debug.LogWarningFormat(property.serializedObject.targetObject, _missingSourceWarning, sourceParameterName, attributeName, property.propertyPath, sourceName, declaringType.Name);

			return null;
		}

		private static ChangeTrigger<FieldType> GetSerializedPropertyTrigger<FieldType>(SerializedProperty property, string sourceName, string attributeName, string sourceParameterName, ref bool found, Action<FieldType> updateAction)
		{
			var sibling = property.GetSibling(sourceName);

			if (sibling != null)
			{
				found = true;

				if (!ValidateProperty(sibling, typeof(FieldType)))
					Debug.LogWarningFormat(property.serializedObject.targetObject, _invalidSiblingWarning, sourceParameterName, attributeName, property.propertyPath, nameof(FieldType));
				else
					return new ChangeTrigger<FieldType>(property, (changedProperty, oldValue, newValue) => updateAction?.Invoke(newValue));
			}

			return null;
		}

		private static Func<FieldType> GetValueFromMethodFunction<FieldType>(Type declaringType, string sourceName, SerializedProperty property, string attributeName, string sourceParameterName, ref bool found, bool logErrors)
		{
			var method = declaringType.GetMethod(sourceName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			if (method != null)
			{
				found = true;

				if (typeof(FieldType).IsAssignableFrom(method.ReturnType))
				{
					if (logErrors)
						Debug.LogWarningFormat(property.serializedObject.targetObject, _invalidMethodReturnWarning, sourceParameterName, attributeName, property.propertyPath, sourceName, nameof(FieldType));
				}
				else if (!method.HasParameters())
				{
					if (logErrors)
						Debug.LogWarningFormat(property.serializedObject.targetObject, _invalidMethodParametersWarning, sourceParameterName, attributeName, property.propertyPath, sourceName);
				}
				else
				{
					return () => (FieldType)method.Invoke(method.IsStatic ? null : property.GetOwner<object>(), null);
				}
			}

			return null;
		}

		private static Func<ParameterOne, FieldType> GetValueFromMethodFunction<ParameterOne, FieldType>(Type declaringType, string sourceName, SerializedProperty property, string attributeName, string sourceParameterName, ref bool found, bool logErrors)
		{
			var method = declaringType.GetMethod(sourceName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			if (method != null)
			{
				found = true;

				if (typeof(FieldType).IsAssignableFrom(method.ReturnType))
				{
					if (logErrors)
						Debug.LogWarningFormat(property.serializedObject.targetObject, _invalidMethodReturnWarning, sourceParameterName, attributeName, property.propertyPath, sourceName, nameof(FieldType));
				}
				else if (!method.HasParameters(typeof(ParameterOne)))
				{
					if (logErrors)
						Debug.LogWarningFormat(property.serializedObject.targetObject, _invalidMethodParametersWarning, sourceParameterName, attributeName, property.propertyPath, sourceName);
				}
				else
				{
					return (parameterOne) =>
					{
						_oneParameter[0] = parameterOne;
						return (FieldType)method.Invoke(method.IsStatic ? null : property.GetOwner<object>(), _oneParameter);
					};
				}
			}

			return null;
		}

		private static Func<ParameterOne, ParameterTwo, FieldType> GetValueFromMethodFunction<ParameterOne, ParameterTwo, FieldType>(Type declaringType, string sourceName, SerializedProperty property, string attributeName, string sourceParameterName, ref bool found, bool logErrors)
		{
			var method = declaringType.GetMethod(sourceName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			if (method != null)
			{
				found = true;

				if (typeof(FieldType).IsAssignableFrom(method.ReturnType))
				{
					if (logErrors)
						Debug.LogWarningFormat(property.serializedObject.targetObject, _invalidMethodReturnWarning, sourceParameterName, attributeName, property.propertyPath, sourceName, nameof(FieldType));
				}
				else if (!method.HasParameters(typeof(ParameterOne), typeof(ParameterTwo)))
				{
					if (logErrors)
						Debug.LogWarningFormat(property.serializedObject.targetObject, _invalidMethodParametersWarning, sourceParameterName, attributeName, property.propertyPath, sourceName);
				}
				else
				{
					return (parameterOne, parameterTwo) =>
					{
						_twoParameters[0] = parameterOne;
						_twoParameters[1] = parameterTwo;
						return (FieldType)method.Invoke(method.IsStatic ? null : property.GetOwner<object>(), _twoParameters);
					};
				}
			}

			return null;
		}

		private static Action GetCallback(Type declaringType, string sourceName, SerializedProperty property, string attributeName, string sourceParameterName)
		{
			var method = declaringType.GetMethod(sourceName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			if (method != null)
			{
				if (method.HasParameters())
					return () => method.Invoke(method.IsStatic ? null : property.GetOwner<object>(), null);
			}

			return null;
		}

		private static Action<ParameterOne> GetCallback<ParameterOne>(Type declaringType, string sourceName, SerializedProperty property, string attributeName, string sourceParameterName)
		{
			var method = declaringType.GetMethod(sourceName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			if (method != null)
			{
				if (method.HasParameters(typeof(ParameterOne)))
				{
					return (parameterOne) =>
					{
						_oneParameter[0] = parameterOne;
						method.Invoke(method.IsStatic ? null : property.GetOwner<object>(), _oneParameter);
					};
				}
			}

			return null;
		}

		private static Action<ParameterOne, ParameterTwo> GetCallback<ParameterOne, ParameterTwo>(Type declaringType, string sourceName, SerializedProperty property, string attributeName, string sourceParameterName)
		{
			var method = declaringType.GetMethod(sourceName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			if (method != null)
			{
				if (!method.HasParameters(typeof(ParameterOne), typeof(ParameterTwo)))
				{
					return (parameterOne, parameterTwo) =>
					{
						_twoParameters[0] = parameterOne;
						_twoParameters[1] = parameterTwo;
						method.Invoke(method.IsStatic ? null : property.GetOwner<object>(), _twoParameters);
					};
				}
			}

			return null;
		}

		private static Func<FieldType> GetValueFromPropertyFunction<FieldType>(Type declaringType, string sourceName, SerializedProperty property, string attributeName, string sourceParameterName, ref bool found)
		{
			var prop = declaringType.GetProperty(sourceName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			if (prop != null)
			{
				found = true;

				if (typeof(FieldType).IsAssignableFrom(prop.PropertyType) || !prop.CanRead)
					Debug.LogWarningFormat(property.serializedObject.targetObject, _invalidPropertyWarning, sourceParameterName, attributeName, property.propertyPath, sourceName, nameof(FieldType));
				else
					return () => (FieldType)prop.GetValue(prop.GetGetMethod(true).IsStatic ? null : property.GetOwner<object>());
			}

			return null;
		}

		private static Func<FieldType> GetValueFromFieldFunction<FieldType>(Type declaringType, string sourceName, SerializedProperty property, string attributeName, string sourceParameterName, ref bool found, ref bool needsSchedule)
		{
			var field = declaringType.GetField(sourceName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field != null)
			{
				found = true;

				if (field.IsLiteral || field.IsInitOnly)
					needsSchedule = false;

				if (typeof(FieldType).IsAssignableFrom(field.FieldType))
					Debug.LogWarningFormat(property.serializedObject.targetObject, _invalidFieldWarning, sourceParameterName, attributeName, property.propertyPath, sourceName, property.type);
				else
					return () => (FieldType)field.GetValue(field.IsStatic ? null : property.GetOwner<object>());
			}

			return null;
		}

		private static void SetupSchedule(VisualElement element, Action action, bool autoUpdate)
		{
			if (autoUpdate)
				element.schedule.Execute(action).Every(100);
			else
				action.Invoke();
		}

		private static bool ValidateProperty(SerializedProperty property, Type type)
		{
			switch (property.propertyType)
			{
				case SerializedPropertyType.Generic: return false;
				case SerializedPropertyType.Integer: return type == typeof(int);
				case SerializedPropertyType.Boolean: return type == typeof(bool);
				case SerializedPropertyType.Float: return type == typeof(float);
				case SerializedPropertyType.String: return type == typeof(string);
				case SerializedPropertyType.Color: return type == typeof(Color);
				case SerializedPropertyType.ObjectReference: return typeof(Object).IsAssignableFrom(type);
				case SerializedPropertyType.LayerMask: return type == typeof(LayerMask);
				case SerializedPropertyType.Enum: return type == typeof(Enum) || type.IsEnum;
				case SerializedPropertyType.Vector2: return type == typeof(Vector2);
				case SerializedPropertyType.Vector3: return type == typeof(Vector3);
				case SerializedPropertyType.Vector4: return type == typeof(Vector4);
				case SerializedPropertyType.Rect: return type == typeof(Rect);
				case SerializedPropertyType.ArraySize: return type == typeof(int);
				case SerializedPropertyType.Character: return type == typeof(char);
				case SerializedPropertyType.AnimationCurve: return type == typeof(AnimationCurve);
				case SerializedPropertyType.Bounds: return type == typeof(Bounds);
				case SerializedPropertyType.Gradient: return type == typeof(Gradient);
				case SerializedPropertyType.Quaternion: return type == typeof(Quaternion);
				case SerializedPropertyType.ExposedReference: return false;
				case SerializedPropertyType.FixedBufferSize: return false;
				case SerializedPropertyType.Vector2Int: return type == typeof(Vector2Int);
				case SerializedPropertyType.Vector3Int: return type == typeof(Vector3Int);
				case SerializedPropertyType.RectInt: return type == typeof(RectInt);
				case SerializedPropertyType.BoundsInt: return type == typeof(BoundsInt);
				case SerializedPropertyType.ManagedReference: var managed = property.GetManagedReferenceValue(); return managed == null || type.IsAssignableFrom(managed.GetType());
				default: return false;
			}
		}
	}
}
