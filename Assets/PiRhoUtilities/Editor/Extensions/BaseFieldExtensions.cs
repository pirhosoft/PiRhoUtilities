using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public static class BaseFieldExtensions
	{
		#region Internal Lookups

		private const string _changedInternalsError = "(PUBFECI) failed to setup BaseField: Unity internals have changed";

		public static readonly string UssClassName = BaseField<int>.ussClassName;
		public static readonly string LabelUssClassName = BaseField<int>.labelUssClassName;
		public static readonly string NoLabelVariantUssClassName = BaseField<int>.noLabelVariantUssClassName;

		private const string _labelName = "label";
		private const string _visualInputName = "visualInput";

		private const string _configureFieldName = "ConfigureField";
		private static MethodInfo _configureFieldMethod;
		private static object[] _configureFieldParameters = new object[2];
		private static PropertyField _configureFieldInstance;

		static BaseFieldExtensions()
		{
			var configureFieldMethod = typeof(PropertyField).GetMethod(_configureFieldName, BindingFlags.Instance | BindingFlags.NonPublic);

			if (configureFieldMethod != null && configureFieldMethod.HasSignature(typeof(VisualElement), null, typeof(SerializedProperty)))
			{
				_configureFieldMethod = configureFieldMethod;
				_configureFieldInstance = new PropertyField();
			}

			if (_configureFieldMethod == null)
				Debug.LogError(_changedInternalsError);
		}

		// can't do these lookups in a static constructor since they are dependent on the generic type

		private static PropertyInfo GetProperty<T>(string name, BindingFlags flags)
		{
			return GetProperty(typeof(BaseField<T>), name, flags);
		}

		private static PropertyInfo GetProperty(Type type, string name, BindingFlags flags)
		{
			var property = type.GetProperty(name, flags);

			if (property == null)
				Debug.LogError(_changedInternalsError);

			return property;
		}

		#endregion

		#region Helper Methods

		public static void SetLabel(VisualElement element, string label)
		{
			// label is public but this allows access without knowing the generic type of the BaseField

			GetProperty(element.GetType(), _labelName, BindingFlags.Instance | BindingFlags.Public).SetValue(element, label);
		}

		#endregion

		#region Extension Methods

		public static VisualElement GetVisualInput<T>(this BaseField<T> field)
		{
			return GetProperty<T>(_visualInputName, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(field) as VisualElement;
		}

		public static void SetVisualInput<T>(this BaseField<T> field, VisualElement element)
		{
			GetProperty<T>(_visualInputName, BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(field, element);
		}

		public static VisualElement ConfigureProperty<T>(this BaseField<T> field, SerializedProperty property, string tooltip)
		{
			field.labelElement.tooltip = tooltip; // it seems like this should happen internally somewhere but it doesn't

			// ConfigureField is effectively static, with one unimportant exception, so it can be called with a dummy
			// instance. The exception is label, which will be null on the dummy instance resulting in the desired
			// effect anyway (using the property name for the label).

			var method = _configureFieldMethod?.MakeGenericMethod(field.GetType(), typeof(T));

			_configureFieldParameters[0] = field;
			_configureFieldParameters[1] = property;

			return method?.Invoke(_configureFieldInstance, _configureFieldParameters) as VisualElement;
		}

		#endregion
	}
}