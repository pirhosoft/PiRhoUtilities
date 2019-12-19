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
		private static readonly Type[] _noParamters = new Type[0];
		private static readonly Type[] _oneParamter = new Type[1];
		private static readonly Type[] _twoParamters = new Type[2];

		private static readonly object[] _oneArgument = new object[1];
		private static readonly object[] _twoArguments = new object[2];

		public static bool SetupValueSourceCallback<FieldType>(string sourceName, Type declaringType, SerializedProperty property, VisualElement element, FieldType defaultValue, bool autoUpdate, Action<FieldType> updateAction)
		{
			if (!string.IsNullOrEmpty(sourceName))
			{
				var valueFunction = GetValueFunction(sourceName, declaringType, property, element, out var needsSchedule, updateAction);
				if (valueFunction == null)
					return false;

				SetupSchedule(element, () => updateAction(valueFunction()), autoUpdate && needsSchedule);
			}
			else
			{
				updateAction(defaultValue);
			}

			return true;
		}

		public static Func<FieldType> CreateValueSourceFunction<FieldType>(string sourceName, SerializedProperty property, VisualElement element, Type declaringType, FieldType defaultValue)
		{
			if (!string.IsNullOrEmpty(sourceName))
				return GetValueFunction<FieldType>(sourceName, declaringType, property, element, out var _, null);

			return () => defaultValue;
		}

		public static Func<FieldType> CreateFunctionCallback<FieldType>(string sourceName, Type declaringType, SerializedProperty property)
		{
			if (!string.IsNullOrEmpty(sourceName))
				return GetValueFromMethodFunction<FieldType>(sourceName, declaringType, property);

			return null;
		}

		public static Func<ParameterOne, FieldType> CreateFunctionCallback<ParameterOne, FieldType>(string sourceName, Type declaringType, SerializedProperty property)
		{
			if (!string.IsNullOrEmpty(sourceName))
				return GetValueFromMethodFunction<ParameterOne, FieldType>(sourceName, declaringType, property);

			return null;
		}

		public static Func<ParameterOne, ParameterTwo, FieldType> CreateFunctionCallback<ParameterOne, ParameterTwo, FieldType>(string sourceName, Type declaringType, SerializedProperty property)
		{
			if (!string.IsNullOrEmpty(sourceName))
				return GetValueFromMethodFunction<ParameterOne, ParameterTwo, FieldType>(sourceName, declaringType, property);

			return null;
		}

		public static Action CreateActionCallback(string sourceName, Type declaringType, SerializedProperty property)
		{
			if (!string.IsNullOrEmpty(sourceName))
				return GetCallback(sourceName, declaringType, property);

			return null;
		}

		public static Action<ParameterOne> CreateActionCallback<ParameterOne>(string sourceName, Type declaringType, SerializedProperty property)
		{
			if (!string.IsNullOrEmpty(sourceName))
				return GetCallback<ParameterOne>(sourceName, declaringType, property);

			return null;
		}

		public static Action<ParameterOne, ParameterTwo> CreateActionCallback<ParameterOne, ParameterTwo>(string sourceName, Type declaringType, SerializedProperty property)
		{
			if (!string.IsNullOrEmpty(sourceName))
				return GetCallback<ParameterOne, ParameterTwo>(sourceName, declaringType, property);

			return null;
		}

		private static Func<FieldType> GetValueFunction<FieldType>(string sourceName, Type declaringType, SerializedProperty property, VisualElement element, out bool needsSchedule, Action<FieldType> updateAction)
		{
			needsSchedule = true;

			{ // Property
				var changeTrigger = GetSerializedPropertyTrigger(sourceName, property, updateAction);
				if (changeTrigger != null)
				{
					needsSchedule = false;
					element.Add(changeTrigger);
					return () => changeTrigger.value;
				}
			}

			{ // Method
				var valueFunction = GetValueFromMethodFunction<FieldType>(sourceName, declaringType, property);
				if (valueFunction != null)
					return valueFunction;
			}

			{ // Property
				var valueFunction = GetValueFromPropertyFunction<FieldType>(sourceName, declaringType, property);
				if (valueFunction != null)
					return valueFunction;
			}

			{ // Field
				var valueFunction = GetValueFromFieldFunction<FieldType>(sourceName, declaringType, property, ref needsSchedule);
				if (valueFunction != null)
					return valueFunction;
			}

			return null;
		}

		private static ChangeTrigger<FieldType> GetSerializedPropertyTrigger<FieldType>(string sourceName, SerializedProperty property, Action<FieldType> updateAction)
		{
			var sibling = property.GetSibling(sourceName);

			if (sibling != null)
			{
				if (ValidateProperty(sibling, typeof(FieldType)))
					return new ChangeTrigger<FieldType>(property, (changedProperty, oldValue, newValue) => updateAction?.Invoke(newValue));
			}

			return null;
		}

		private static Func<FieldType> GetValueFromMethodFunction<FieldType>(string sourceName, Type declaringType, SerializedProperty property)
		{
			var method = declaringType.GetMethod(sourceName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, null, CallingConventions.Standard | CallingConventions.HasThis, _noParamters, null);

			if (method != null)
			{
				if (typeof(FieldType).IsAssignableFrom(method.ReturnType))
				{
					var owner = method.IsStatic ? null : property.GetOwner<object>();
					return () => (FieldType)method.Invoke(owner, null);
				}
			}

			return null;
		}

		private static Func<ParameterOne, FieldType> GetValueFromMethodFunction<ParameterOne, FieldType>(string sourceName, Type declaringType, SerializedProperty property)
		{
			_oneParamter[0] = typeof(ParameterOne);

			var method = declaringType.GetMethod(sourceName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, null, CallingConventions.Standard | CallingConventions.HasThis, _oneParamter, null);

			if (method != null)
			{
				if (typeof(FieldType).IsAssignableFrom(method.ReturnType))
				{
					var owner = method.IsStatic ? null : property.GetOwner<object>();
					return (parameterOne) =>
					{
						_oneArgument[0] = parameterOne;
						return (FieldType)method.Invoke(owner, _oneArgument);
					};
				}
			}

			return null;
		}

		private static Func<ParameterOne, ParameterTwo, FieldType> GetValueFromMethodFunction<ParameterOne, ParameterTwo, FieldType>(string sourceName, Type declaringType, SerializedProperty property)
		{
			_twoParamters[0] = typeof(ParameterOne);
			_twoParamters[1] = typeof(ParameterTwo);

			var method = declaringType.GetMethod(sourceName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, null, CallingConventions.Standard | CallingConventions.HasThis, _twoParamters, null);

			if (method != null)
			{
				if (typeof(FieldType).IsAssignableFrom(method.ReturnType))
				{
					var owner = method.IsStatic ? null : property.GetOwner<object>();
					return (parameterOne, parameterTwo) =>
					{
						_twoArguments[0] = parameterOne;
						_twoArguments[1] = parameterTwo;
						return (FieldType)method.Invoke(owner, _twoArguments);
					};
				}
			}

			return null;
		}

		private static Action GetCallback(string sourceName, Type declaringType, SerializedProperty property)
		{
			var method = declaringType.GetMethod(sourceName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, null, CallingConventions.Standard | CallingConventions.HasThis, _noParamters, null);

			if (method != null)
			{
				var owner = method.IsStatic ? null : property.GetOwner<object>();
				return () => method.Invoke(owner, null);
			}

			return null;
		}

		private static Action<ParameterOne> GetCallback<ParameterOne>(string sourceName, Type declaringType, SerializedProperty property)
		{
			_oneParamter[0] = typeof(ParameterOne);

			var method = declaringType.GetMethod(sourceName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, null, CallingConventions.Standard | CallingConventions.HasThis, _oneParamter, null);

			if (method != null)
			{
				var owner = method.IsStatic ? null : property.GetOwner<object>();
				return (parameterOne) =>
				{
					_oneArgument[0] = parameterOne;
					method.Invoke(owner, _oneArgument);
				};
			}

			return null;
		}

		private static Action<ParameterOne, ParameterTwo> GetCallback<ParameterOne, ParameterTwo>(string sourceName, Type declaringType, SerializedProperty property)
		{
			_twoParamters[0] = typeof(ParameterOne);
			_twoParamters[1] = typeof(ParameterTwo);

			var method = declaringType.GetMethod(sourceName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, null, CallingConventions.Standard | CallingConventions.HasThis, _twoParamters, null);

			if (method != null)
			{
				var owner = method.IsStatic ? null : property.GetOwner<object>();
				return (parameterOne, parameterTwo) =>
				{
					_twoArguments[0] = parameterOne;
					_twoArguments[1] = parameterTwo;
					method.Invoke(owner, _twoArguments);
				};
			}

			return null;
		}

		private static Func<FieldType> GetValueFromPropertyFunction<FieldType>(string sourceName, Type declaringType, SerializedProperty property)
		{
			var prop = declaringType.GetProperty(sourceName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, null, typeof(FieldType), _noParamters, null);

			if (prop != null)
			{
				if (prop.CanRead)
				{
					var owner = prop.GetGetMethod(true).IsStatic ? null : property.GetOwner<object>();
					return () => (FieldType)prop.GetValue(owner);
				}
			}

			return null;
		}

		private static Func<FieldType> GetValueFromFieldFunction<FieldType>(string sourceName, Type declaringType, SerializedProperty property, ref bool needsSchedule)
		{
			var field = declaringType.GetField(sourceName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (field != null)
			{
				if (field.IsLiteral || field.IsInitOnly)
					needsSchedule = false;

				if (typeof(FieldType) == field.FieldType)
				{
					var owner = field.IsStatic ? null : property.GetOwner<object>();
					return () => (FieldType)field.GetValue(owner);
				}
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
