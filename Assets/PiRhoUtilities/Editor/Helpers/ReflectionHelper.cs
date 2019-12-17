using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Editor
{
	[Flags]
	public enum ReflectionSource
	{
		None = 0,
		SerializedProperty = 1 << 0,
		Method = 1 << 1,
		Property = 1 << 2,
		Field = 1 << 3,
		All = int.MaxValue
	}

	public static class ReflectionHelper
	{
		private const string _missingSourceWarning = "(PURHMS) invalid '{0}' for '{1}' on field '{2}': '{3}' could not be found on type '{4}'";
		private const string _invalidSiblingWarning = "(PURHIPR) invalid '{0}' sibling for '{1}' on field '{2}': the SerializedProperty '{3}' should be a {4}";
		private const string _invalidMethodReturnWarning = "(PURHMR) invalid '{0}' method for '{1}' on field '{2}': the method '{3}' should return a '{4}'";
		private const string _invalidMethodParametersWarning = "(PURHIMP) invalid '{0}' method for '{1}' on field '{2}': the method '{3}' has invalid parameters";
		private const string _invalidPropertyWarning = "(PURHIPR) invalid '{0}' property for '{1}' on field '{2}': the property '{3}' should be a readable {4}";
		private const string _invalidFieldWarning = "(PURHIFR) invalid '{0}' field for '{1}' on field '{2}': the field '{3}' should be a {4}";

		private static readonly object[] _oneParameter = new object[1];
		private static readonly object[] _twoParameters = new object[2];

		public static void SetupValueSourceCallback<FieldType>(string sourceName, ReflectionSource validSources, SerializedProperty property, VisualElement element, Type declaringType, FieldType defaultValue, bool autoUpdate, string attributeName, string sourceParameterName, Action<FieldType> updateAction)
		{
			if (!string.IsNullOrEmpty(sourceName))
			{
				var valueFunction = GetValueFunction(validSources, property, declaringType, sourceName, attributeName, sourceParameterName, element, out var needsSchedule, updateAction);
				if (valueFunction != null)
					SetupSchedule(element, () => updateAction.Invoke(valueFunction()), autoUpdate && needsSchedule);
			}
			else
			{
				updateAction.Invoke(defaultValue);
			}
		}

		public static Func<FieldType> CreateValueSourceFunction<FieldType>(SerializedProperty property, VisualElement element, Type declaringType, string sourceName, ReflectionSource validSources, FieldType defaultValue, string attributeName, string sourceParameterName)
		{
			if (!string.IsNullOrEmpty(sourceName))
				return GetValueFunction<FieldType>(validSources, property, declaringType, sourceName, attributeName, sourceParameterName, element, out var _, null);

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

		private static Func<FieldType> GetValueFunction<FieldType>(ReflectionSource validSources, SerializedProperty property, Type declaringType, string sourceName, string attributeName, string sourceParameterName, VisualElement element, out bool needsSchedule, Action<FieldType> updateAction)
		{
			needsSchedule = true;
			var found = false;

			if ((validSources & ReflectionSource.SerializedProperty) > 0)
			{
				var changeTrigger = GetSerializedPropertyTrigger(property, sourceName, attributeName, sourceParameterName, ref found, updateAction);
				if (changeTrigger != null)
				{
					needsSchedule = false;
					element.Add(changeTrigger);
					return () => changeTrigger.value;
				}
			}

			if ((validSources & ReflectionSource.Method) > 0)
			{
				var valueFunction = GetValueFromMethodFunction<FieldType>(declaringType, sourceName, property, attributeName, sourceParameterName, ref found, true);
				if (valueFunction != null)
					return valueFunction;
			}

			if ((validSources & ReflectionSource.Property) > 0)
			{
				var valueFunction = GetValueFromPropertyFunction<FieldType>(declaringType, sourceName, property, attributeName, sourceParameterName, ref found);
				if (valueFunction != null)
					return valueFunction;
			}

			if ((validSources & ReflectionSource.Field) > 0)
			{
				var valueFunction = GetValueFromFieldFunction<FieldType>(declaringType, sourceName, property, attributeName, sourceParameterName, ref found, ref needsSchedule);
				if (valueFunction != null)
					return valueFunction;
			}

			if (!found)
				Debug.LogWarningFormat(property.serializedObject.targetObject, _missingSourceWarning, sourceParameterName, attributeName, property.propertyPath, sourceName, declaringType.Name);

			return null;
		}

		private static ChangeTriggerControl<FieldType> GetSerializedPropertyTrigger<FieldType>(SerializedProperty property, string sourceName, string attributeName, string sourceParameterName, ref bool found, Action<FieldType> updateAction)
		{
			var sibling = property.GetSibling(sourceName);

			if (sibling != null)
			{
				found = true;

				if (!ValidateProperty(sibling, typeof(FieldType)))
					Debug.LogWarningFormat(property.serializedObject.targetObject, _invalidSiblingWarning, sourceParameterName, attributeName, property.propertyPath, nameof(FieldType));
				else
					return new ChangeTriggerControl<FieldType>(property, (oldValue, newValue) => updateAction?.Invoke(newValue));
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
				case SerializedPropertyType.Boolean: return type == typeof(bool);
				case SerializedPropertyType.Integer: return type == typeof(int);
				case SerializedPropertyType.Float: return type == typeof(float);
				case SerializedPropertyType.String: return type == typeof(string);
				case SerializedPropertyType.Enum: return typeof(Enum).IsAssignableFrom(type);
				case SerializedPropertyType.ObjectReference: return typeof(Object).IsAssignableFrom(type);
				default: return false;
			}
		}
	}
}
