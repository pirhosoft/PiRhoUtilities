using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public static class VisualElementExtensions
	{
		#region Internal Lookups

		private const string _changedInternalsError = "(PUVEECI) failed to setup VisualElement: Unity internals have changed";

		private const string _serializedPropertyBindEventName = "UnityEditor.UIElements.SerializedPropertyBindEvent, UnityEditor";
		private static Type _serializedPropertyBindEventType;
		private static string _bindPropertyName = "bindProperty";
		private static PropertyInfo _bindPropertyProperty;

		static VisualElementExtensions()
		{
			var serializedPropertyBindEventType = Type.GetType(_serializedPropertyBindEventName);
			var bindPropertyProperty = serializedPropertyBindEventType?.GetProperty(_bindPropertyName, BindingFlags.Instance | BindingFlags.Public);

			if (serializedPropertyBindEventType != null && bindPropertyProperty != null && bindPropertyProperty.PropertyType == typeof(SerializedProperty))
			{
				_serializedPropertyBindEventType = serializedPropertyBindEventType;
				_bindPropertyProperty = bindPropertyProperty;
			}

			if (_serializedPropertyBindEventType == null || _bindPropertyProperty == null)
				Debug.LogError(_changedInternalsError);
		}

		#endregion

		#region Events

		public static bool TryGetPropertyBindEvent(this VisualElement element, EventBase evt, out SerializedProperty property)
		{
			property = evt.GetType() == _serializedPropertyBindEventType
				? _bindPropertyProperty?.GetValue(evt) as SerializedProperty
				: null;

			return property != null;
		}

		public static void SendChangeEvent<T>(this VisualElement element, T previous, T current)
		{
			using (var changeEvent = ChangeEvent<T>.GetPooled(previous, current))
			{
				changeEvent.target = element;
				element.SendEvent(changeEvent);
			}
		}

		#endregion

		#region Style

		private const string _missingStylesheetError = "(PUEHMS) failed to load stylesheet: the asset '{0}' could not be found";
		private const string _missingUxmlError = "(PUEHMU) failed to load uxml: the asset '{0}' could not be found";

		public static void AddStyleSheet(this VisualElement element, string editorPath, string path)
		{
			var fullPath = Path.Combine(editorPath, path);
			var stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(fullPath);

			if (stylesheet != null)
				element.styleSheets.Add(stylesheet);
			else
				Debug.LogErrorFormat(_missingStylesheetError, fullPath);
		}

		public static void AddUxml(this VisualElement element, string editorPath, string path)
		{
			var fullPath = Path.Combine(editorPath, path);
			var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(fullPath);

			if (uxml != null)
				uxml.CloneTree(element);
			else
				Debug.LogErrorFormat(_missingUxmlError, fullPath);
		}

		public static VisualElement GetRootElement(this VisualElement element)
		{
			while (element.parent != null)
				element = element.parent;
			return element;
		}

		public static void SetDisplayed(this VisualElement element, bool displayed)
		{
			element.style.display = displayed ? DisplayStyle.Flex : DisplayStyle.None;
		}

		public static void AlternateClass(this VisualElement element, string validClass, string invalidClass, bool isValid)
		{
			if (isValid)
			{
				if (!element.ClassListContains(validClass))
					element.AddToClassList(validClass);

				if (element.ClassListContains(invalidClass))
					element.RemoveFromClassList(invalidClass);
			}
			else
			{
				if (!element.ClassListContains(invalidClass))
					element.AddToClassList(invalidClass);

				if (element.ClassListContains(validClass))
					element.RemoveFromClassList(validClass);
			}
		}

		#endregion

		#region Property Configuration

		private const string _labelName = "label";

		public static void SetFieldLabel(this VisualElement field, string label)
		{
			if (field is PropertyField propertyField)
			{
				propertyField.label = label;

				// if label is being cleared it will be automatically set to the property name on binding so it then needs to be reset
				// TODO: figure out a better way to do this

				if (string.IsNullOrEmpty(label))
				{
					field.schedule.Execute(() =>
					{
						propertyField.label = label;
						var baseField = field.Q(className: BaseFieldExtensions.UssClassName);
						baseField.SetFieldLabel(label);
					}).StartingIn(0);
				}
			}
			else if (field is FieldContainer fieldContainer)
			{
				fieldContainer.SetLabel(label);
			}
			else if (field is Foldout foldout)
			{
				foldout.text = label;

				// clear the binding of the property to the foldout label
				var foldoutToggle = foldout.Q<Toggle>(className: Foldout.toggleUssClassName);
				var foldoutLabel = foldoutToggle.Q<Label>(className: Toggle.textUssClassName);

				foldoutLabel.bindingPath = null;
				foldoutLabel.binding.Release();
			}
			else if (field.GetType().InheritsGeneric(typeof(BaseField<>)))
			{
				// label is public but this allows access without knowing the generic type of the BaseField
				field.GetType().GetProperty(_labelName, BindingFlags.Instance | BindingFlags.Public).SetValue(field, label);
			}
		}

		public static void ConfigureAsField(this VisualElement element, Label label, SerializedProperty property)
		{
			label.tooltip = property.GetTooltip();
		}

		#endregion
	}
}
