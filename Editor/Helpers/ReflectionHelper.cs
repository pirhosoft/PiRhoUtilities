using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

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

			if (!typeof(FieldType).ImplementsInterface<IList>()) // Don't uses List SerializedProperties because binding against them doesn't work
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
				return new ChangeTrigger<FieldType>(sibling, (changedProperty, oldValue, newValue) => updateAction?.Invoke(newValue));

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
	}
}
