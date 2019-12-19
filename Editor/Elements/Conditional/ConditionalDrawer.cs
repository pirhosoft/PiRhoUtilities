using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(ConditionalAttribute))]
	class ConditionalDrawer : PropertyDrawer
	{
		public const string UssClassName = "pirho-conditional";
		private const string _invalidSourceError = "(PUCDDIS) invalid value source for ConditionalAttribute on field '{0}': a field, method, or property of type '{1}' named '{2}' could not be found";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var conditionalAttribute = attribute as ConditionalAttribute;
			var element = this.CreateNextElement(property);
			element.AddToClassList(UssClassName);

			if (!SetupCondition(element, property, conditionalAttribute))
				Debug.LogWarningFormat(_invalidSourceError, property.propertyPath, conditionalAttribute.Type.ToString(), conditionalAttribute.ValueSource);

			return element;
		}

		private bool SetupCondition(VisualElement element, SerializedProperty property, ConditionalAttribute conditionalAttribute)
		{
			switch (conditionalAttribute.Type)
			{
				case ConditionalAttribute.TestType.Bool:
					return ReflectionHelper.SetupValueSourceCallback(conditionalAttribute.ValueSource, fieldInfo.DeclaringType, property, element, true, true, value => UpdateBoolVisibility(element, value, conditionalAttribute.BoolTest));
				case ConditionalAttribute.TestType.Int:
					return ReflectionHelper.SetupValueSourceCallback(conditionalAttribute.ValueSource, fieldInfo.DeclaringType, property, element, 0, true, value => UpdateNumberVisibility(element, value, conditionalAttribute.IntValue, conditionalAttribute.NumberTest));
				case ConditionalAttribute.TestType.Float:
					return ReflectionHelper.SetupValueSourceCallback(conditionalAttribute.ValueSource, fieldInfo.DeclaringType, property, element, 0.0f, true, value => UpdateNumberVisibility(element, value, conditionalAttribute.FloatValue, conditionalAttribute.NumberTest));
				case ConditionalAttribute.TestType.String:
					return ReflectionHelper.SetupValueSourceCallback(conditionalAttribute.ValueSource, fieldInfo.DeclaringType, property, element, string.Empty, true, value => UpdateStringVisibility(element, value, conditionalAttribute.StringValue, conditionalAttribute.StringTest));
				case ConditionalAttribute.TestType.Enum:
					return ReflectionHelper.SetupValueSourceCallback(conditionalAttribute.ValueSource, fieldInfo.DeclaringType, property, element, default(Enum), true, value => UpdateEnumVisibility(element, value, conditionalAttribute.IntValue, conditionalAttribute.EnumTest));
				case ConditionalAttribute.TestType.Object:
					return ReflectionHelper.SetupValueSourceCallback(conditionalAttribute.ValueSource, fieldInfo.DeclaringType, property, element, (Object)null, true, value => UpdateObjectVisibility(element, value, conditionalAttribute.ObjectTest));
			}

			return false;
		}

		private static void UpdateBoolVisibility(VisualElement element, bool value, BoolTest test)
		{
			element.SetDisplayed((value && test == BoolTest.ShowIfTrue) || (!value && test == BoolTest.ShowIfFalse));
		}

		private static void UpdateNumberVisibility<T>(VisualElement element, T value, T condition, NumberTest test) where T : IComparable<T>
		{
			var comparison = value.CompareTo(condition);
			var visible = false;

			switch (test)
			{
				case NumberTest.ShowIfEqual: visible = comparison == 0; break;
				case NumberTest.ShowIfInequal: visible = comparison != 0; break;
				case NumberTest.ShowIfLessThan: visible = comparison < 0; break;
				case NumberTest.ShowIfGreaterThan: visible = comparison > 0; break;
				case NumberTest.ShowIfLessThanOrEqual: visible = comparison <= 0; break;
				case NumberTest.ShowIfGreaterThanOrEqual: visible = comparison >= 0; break;
			}

			element.SetDisplayed(visible);
		}

		private static void UpdateStringVisibility(VisualElement element, string value, string comparison, StringTest test)
		{
			var visible = false;

			switch (test)
			{
				case StringTest.ShowIfEqual: visible = value == comparison; break;
				case StringTest.ShowIfInequal: visible = value != comparison; break;
				case StringTest.ShowIfEmpty: visible = string.IsNullOrEmpty(value); break;
				case StringTest.ShowIfNotEmpty: visible = string.IsNullOrEmpty(value); break;
			}


			element.SetDisplayed(visible);
		}

		private static void UpdateEnumVisibility(VisualElement element, Enum value, int comparison, EnumTest test)
		{
			var visible = false;

			if (value != null)
			{
				var type = value.GetType();
				var intValue = (int)Enum.Parse(type, value.ToString());

				switch (test)
				{
					case EnumTest.ShowIfEqual: visible = intValue == comparison; break;
					case EnumTest.ShowIfInequal: visible = intValue != comparison; break;
				}
			}

			element.SetDisplayed(visible);
		}

		private static void UpdateObjectVisibility(VisualElement element, Object value, ObjectTest test)
		{
			element.SetDisplayed((value && test == ObjectTest.ShowIfSet) || (!value && test == ObjectTest.ShowIfNotSet));
		}
	}
}