using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public static class BindingExtensions
	{
		#region Internal Lookups

		private const string _changedInternalsError = "(PUBFECI) failed to setup BindingExtensions: Unity internals have changed";
		private const string _typeName = "UnityEditor.UIElements.BindingExtensions, UnityEditor";

		private const string _serializedObjectUpdateWrapperName = "SerializedObjectUpdateWrapper";
		private static Type _serializedObjectUpdateWrapperType;

		private const string _defaultBindName = "DefaultBind";
		private static MethodInfo _defaultBindEnumMethod;

		private const string _serializedObjectBindingName = "SerializedObjectBinding`1";
		private const string _createBindName = "CreateBind";
		private static MethodInfo _createBindMethod;

		static BindingExtensions()
		{
			var type = Type.GetType(_typeName);

			var serializedObjectUpdateWrapperType = type?.GetNestedType(_serializedObjectUpdateWrapperName, BindingFlags.NonPublic);
			var serializedObjectUpdateWrapperConstructor = serializedObjectUpdateWrapperType?.GetConstructor(new Type[] { typeof(SerializedObject) });

			if (serializedObjectUpdateWrapperConstructor != null)
				_serializedObjectUpdateWrapperType = serializedObjectUpdateWrapperType;

			var defaultBindMethod = type?.GetMethod(_defaultBindName, BindingFlags.Static | BindingFlags.NonPublic);
			var defaultBindEnumMethod = defaultBindMethod?.MakeGenericMethod(typeof(Enum));
			var defaultBindParameters = defaultBindEnumMethod?.GetParameters();

			if (defaultBindEnumMethod != null && defaultBindEnumMethod.IsStatic && defaultBindEnumMethod.HasSignature(null,
				typeof(VisualElement),
				serializedObjectUpdateWrapperType,
				typeof(SerializedProperty),
				typeof(Func<SerializedProperty, Enum>),
				typeof(Action<SerializedProperty, Enum>),
				typeof(Func<Enum, SerializedProperty, Func<SerializedProperty, Enum>, bool>)))
			{
				_defaultBindEnumMethod = defaultBindEnumMethod;
			}

			if (_serializedObjectUpdateWrapperType == null || _defaultBindEnumMethod == null)
				Debug.LogError(_changedInternalsError);

			var serializedObjectBindingType = type?.GetNestedType(_serializedObjectBindingName, BindingFlags.NonPublic);
			var serializedObjectBindingObjectType = serializedObjectBindingType?.MakeGenericType(typeof(object));
			_createBindMethod = serializedObjectBindingObjectType?.GetMethod(_createBindName, BindingFlags.Public | BindingFlags.Static); // TODO: check signature
		}

		#endregion

		#region Helper Methods

		public static void DefaultEnumBind(INotifyValueChanged<Enum> field, SerializedProperty property)
		{
			// 2019.3 only supports flags on EnumFlagsField specifically

			var type = field.value.GetType();
			var wrapper = Activator.CreateInstance(_serializedObjectUpdateWrapperType, property.serializedObject);

			Func<SerializedProperty, Enum> getter = p => Enum.ToObject(type, p.intValue) as Enum;
			Action<SerializedProperty, Enum> setter = (p, v) => p.intValue = (int)Enum.Parse(type, v.ToString());
			Func<Enum, SerializedProperty, Func<SerializedProperty, Enum>, bool> comparer = (v, p, g) => g(p).Equals(v);

			_defaultBindEnumMethod.Invoke(null, new object[] { field, wrapper, property, getter, setter, comparer });
		}

		public static void DefaultManagedReferenceBind(INotifyValueChanged<object> field, SerializedProperty property, Func<SerializedProperty, object> getter, Action<SerializedProperty, object> setter)
		{
			var type = Type.GetType(_typeName);
			var serializedObjectBindingType = type?.GetNestedType(_serializedObjectBindingName, BindingFlags.NonPublic);
			var serializedObjectBindingObjectType = serializedObjectBindingType?.MakeGenericType(typeof(object));
			_createBindMethod = serializedObjectBindingObjectType?.GetMethod(_createBindName, BindingFlags.Public | BindingFlags.Static); // TODO: check signature

			var wrapper = Activator.CreateInstance(_serializedObjectUpdateWrapperType, property.serializedObject);
			Func<object, SerializedProperty, Func<SerializedProperty, object>, bool> comparer = ManagedReferenceEquals;

			_createBindMethod.Invoke(null, new object[] { field, wrapper, property, getter, setter, comparer });
		}

		private static bool ManagedReferenceEquals(object value, SerializedProperty property, Func<SerializedProperty, object> getter)
		{
			var currentValue = getter(property);
			return ReferenceEquals(value, currentValue);
		}

		#endregion
	}
}