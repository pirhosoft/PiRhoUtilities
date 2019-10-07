using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(ConditionalAttribute))]
	class ConditionalDrawer : PropertyDrawer
	{
		public const string UssClassName = "pirho-conditional";

		private const string _missingSiblingWarning = "(PUCDMS) invalid property for ConditionalAttribute on field '{0}': the property '{1}' could not be found on type '{2}'";
		private const string _invalidSiblingWarning = "(PUCDIS) invalid property for ConditionalAttribute on field '{0}': the property '{1}' should be a bool, int, float, or string";
		private const string _missingFieldWarning = "(PUCDMF) invalid field for ConditionalAttribute on field '{0}': the field '{1}' could not be found on type '{2}'";
		private const string _invalidFieldWarning = "(PUCDIF) invalid field for ConditionalAttribute on field '{0}': the field '{1}' should be a bool, int, float, or string";
		private const string _missingPropertyWarning = "(PUCDMP) invalid property for ConditionalAttribute on field '{0}': the property '{1}' could not be found on type '{2}'";
		private const string _invalidPropertyWarning = "(PUCDIP) invalid property for ConditionalAttribute on field '{0}': the property '{1}' should be a bool, int, float, or string";
		private const string _missingMethodWarning = "(PUCDMM) invalid method for ConditionalAttribute on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _invalidMethodReturnWarning = "(PUCDIMR) invalid method for ConditionalAttribute on field '{0}': the method '{1}' should return a bool, int, float, or string";
		private const string _invalidMethodParametersWarning = "(PUCDIMP) invalid method for ConditionalAttribute on field '{0}': the method '{1}' should take no parameters";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var element = this.CreateNextElement(property);
			SetupCondition(element, property);
			element.AddToClassList(UssClassName);
			return element;
		}

		private void SetupCondition(VisualElement element, SerializedProperty property)
		{
			var conditionalAttribute = attribute as ConditionalAttribute;

			switch (conditionalAttribute.Source)
			{
				case ConditionalSource.Sibling: SetupSiblingCondition(element, property); break;
				case ConditionalSource.Field: SetupFieldCondition(element, property); break;
				case ConditionalSource.Property: SetupPropertyCondition(element, property); break;
				case ConditionalSource.Method: SetupMethodCondition(element, property); break;
			}
		}

		private void SetupSiblingCondition(VisualElement element, SerializedProperty property)
		{
			var conditionalAttribute = attribute as ConditionalAttribute;
			var sibling = property.GetSibling(conditionalAttribute.SourceName);
			var trigger = CreateChangeTrigger(element, property, sibling);

			element.Add(trigger);
		}

		private PropertyWatcher CreateChangeTrigger(VisualElement element, SerializedProperty property, SerializedProperty sibling)
		{
			var conditionalAttribute = attribute as ConditionalAttribute;

			if (sibling != null)
			{
				switch (sibling.propertyType)
				{
					case SerializedPropertyType.Boolean:
					{
						UpdateVisibility(element, sibling.boolValue, conditionalAttribute.BoolValue, conditionalAttribute.Test);
						return new ChangeTriggerControl<bool>(sibling, (oldValue, newValue) => UpdateVisibility(element, newValue, conditionalAttribute.BoolValue, conditionalAttribute.Test));
					}
					case SerializedPropertyType.Integer:
					{
						UpdateVisibility(element, sibling.intValue, conditionalAttribute.IntValue, conditionalAttribute.Test);
						return new ChangeTriggerControl<int>(sibling, (oldValue, newValue) => UpdateVisibility(element, newValue, conditionalAttribute.IntValue, conditionalAttribute.Test));
					}
					case SerializedPropertyType.Float:
					{
						UpdateVisibility(element, sibling.floatValue, conditionalAttribute.FloatValue, conditionalAttribute.Test);
						return new ChangeTriggerControl<float>(sibling, (oldValue, newValue) => UpdateVisibility(element, newValue, conditionalAttribute.FloatValue, conditionalAttribute.Test));
					}
					case SerializedPropertyType.String:
					{
						UpdateVisibility(element, sibling.stringValue, conditionalAttribute.StringValue, conditionalAttribute.Test);
						return new ChangeTriggerControl<string>(sibling, (oldValue, newValue) => UpdateVisibility(element, newValue, conditionalAttribute.StringValue, conditionalAttribute.Test));
					}
					case SerializedPropertyType.Enum:
					{
						UpdateVisibility(element, sibling.intValue, conditionalAttribute.IntValue, conditionalAttribute.Test);
						return new ChangeTriggerControl<Enum>(sibling, (oldValue, newValue) => UpdateVisibility(element, sibling.intValue, conditionalAttribute.IntValue, conditionalAttribute.Test));
					}
				}

				Debug.LogWarningFormat(_invalidSiblingWarning, property.propertyPath, conditionalAttribute.SourceName);
			}
			else
			{
				Debug.LogWarningFormat(_missingSiblingWarning, property.propertyPath, conditionalAttribute.SourceName, fieldInfo.DeclaringType.Name);
			}

			return null;
		}

		private void SetupFieldCondition(VisualElement element, SerializedProperty property)
		{
			var conditionalAttribute = attribute as ConditionalAttribute;
			var field = fieldInfo.DeclaringType.GetField(conditionalAttribute.SourceName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			if (field != null)
			{
				var owner = field.IsStatic ? null : property.GetOwner<object>();

				if (field.FieldType == typeof(bool))
					element.schedule.Execute(() => UpdateVisibility(element, (bool)field.GetValue(owner), conditionalAttribute.BoolValue, conditionalAttribute.Test)).Every(100);
				else if (field.FieldType == typeof(int))
					element.schedule.Execute(() => UpdateVisibility(element, (int)field.GetValue(owner), conditionalAttribute.IntValue, conditionalAttribute.Test)).Every(100);
				else if (field.FieldType == typeof(float))
					element.schedule.Execute(() => UpdateVisibility(element, (float)field.GetValue(owner), conditionalAttribute.FloatValue, conditionalAttribute.Test)).Every(100);
				else if (field.FieldType == typeof(string))
					element.schedule.Execute(() => UpdateVisibility(element, (string)field.GetValue(owner), conditionalAttribute.StringValue, conditionalAttribute.Test)).Every(100);
				else
					Debug.LogWarningFormat(_invalidFieldWarning, property.propertyPath, conditionalAttribute.SourceName);
			}
			else
			{
				Debug.LogWarningFormat(_missingFieldWarning, property.propertyPath, conditionalAttribute.SourceName, fieldInfo.DeclaringType.Name);
			}
		}

		private void SetupPropertyCondition(VisualElement element, SerializedProperty property)
		{
			var conditionalAttribute = attribute as ConditionalAttribute;
			var propertyInfo = fieldInfo.DeclaringType.GetProperty(conditionalAttribute.SourceName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			if (propertyInfo != null)
			{
				var owner = propertyInfo.GetGetMethod().IsStatic ? null : property.GetOwner<object>();

				if (propertyInfo.PropertyType == typeof(bool))
					element.schedule.Execute(() => UpdateVisibility(element, (bool)propertyInfo.GetValue(owner), conditionalAttribute.BoolValue, conditionalAttribute.Test)).Every(100);
				else if (propertyInfo.PropertyType == typeof(int))
					element.schedule.Execute(() => UpdateVisibility(element, (int)propertyInfo.GetValue(owner), conditionalAttribute.IntValue, conditionalAttribute.Test)).Every(100);
				else if (propertyInfo.PropertyType == typeof(float))
					element.schedule.Execute(() => UpdateVisibility(element, (float)propertyInfo.GetValue(owner), conditionalAttribute.FloatValue, conditionalAttribute.Test)).Every(100);
				else if (propertyInfo.PropertyType == typeof(string))
					element.schedule.Execute(() => UpdateVisibility(element, (string)propertyInfo.GetValue(owner), conditionalAttribute.StringValue, conditionalAttribute.Test)).Every(100);
				else
					Debug.LogWarningFormat(_invalidPropertyWarning, property.propertyPath, conditionalAttribute.SourceName);
			}
			else
			{
				Debug.LogWarningFormat(_missingPropertyWarning, property.propertyPath, conditionalAttribute.SourceName, fieldInfo.DeclaringType.Name);
			}
		}

		private void SetupMethodCondition(VisualElement element, SerializedProperty property)
		{
			var conditionalAttribute = attribute as ConditionalAttribute;
			var methodInfo = fieldInfo.DeclaringType.GetMethod(conditionalAttribute.SourceName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			if (methodInfo != null)
			{
				if (methodInfo.HasSignature(null))
				{
					var owner = methodInfo.IsStatic ? null : property.GetOwner<object>();

					if (methodInfo.ReturnType == typeof(bool))
						element.schedule.Execute(() => UpdateVisibility(element, (bool)methodInfo.Invoke(owner, null), conditionalAttribute.BoolValue, conditionalAttribute.Test)).Every(100);
					else if (methodInfo.ReturnType == typeof(int))
						element.schedule.Execute(() => UpdateVisibility(element, (int)methodInfo.Invoke(owner, null), conditionalAttribute.IntValue, conditionalAttribute.Test)).Every(100);
					else if (methodInfo.ReturnType == typeof(float))
						element.schedule.Execute(() => UpdateVisibility(element, (float)methodInfo.Invoke(owner, null), conditionalAttribute.FloatValue, conditionalAttribute.Test)).Every(100);
					else if (methodInfo.ReturnType == typeof(string))
						element.schedule.Execute(() => UpdateVisibility(element, (string)methodInfo.Invoke(owner, null), conditionalAttribute.StringValue, conditionalAttribute.Test)).Every(100);
					else
						Debug.LogWarningFormat(_invalidMethodReturnWarning, property.propertyPath, conditionalAttribute.SourceName);
				}
				else
				{
					Debug.LogWarningFormat(_invalidMethodParametersWarning, property.propertyPath, conditionalAttribute.SourceName);
				}
			}
			else
			{
				Debug.LogWarningFormat(_missingMethodWarning, property.propertyPath, conditionalAttribute.SourceName, fieldInfo.DeclaringType.Name);
			}
		}

		private static void UpdateVisibility<T>(VisualElement element, T value, T condition, ConditionalTest test) where T : IComparable<T>
		{
			var comparison = value.CompareTo(condition);
			var visible = false;

			switch (test)
			{
				case ConditionalTest.Equal: visible = comparison == 0; break;
				case ConditionalTest.Inequal: visible = comparison != 0; break;
				case ConditionalTest.LessThan: visible = comparison < 0; break;
				case ConditionalTest.GreaterThan: visible = comparison > 0; break;
				case ConditionalTest.LessThanOrEqual: visible = comparison <= 0; break;
				case ConditionalTest.GreaterThanOrEqual: visible = comparison >= 0; break;
			}

			element.SetDisplayed(visible);
		}
	}
}