using System;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(ConditionalAttribute))]
	class ConditionalDrawer : PropertyDrawer
	{
		public const string UssClassName = "pirho-conditional";

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

			switch (conditionalAttribute.Type)
			{
				case ConditionalAttribute.TestType.Bool:
					ReflectionHelper.SetupValueSourceCallback(conditionalAttribute.ValueSource, property, element, fieldInfo.DeclaringType, true, true, nameof(ConditionalAttribute), nameof(ConditionalAttribute.ValueSource),
						value => UpdateBoolVisibility(element, value, conditionalAttribute.BoolTest));
					break;
				case ConditionalAttribute.TestType.Int:
					ReflectionHelper.SetupValueSourceCallback(conditionalAttribute.ValueSource, property, element, fieldInfo.DeclaringType, 0, true, nameof(ConditionalAttribute), nameof(ConditionalAttribute.ValueSource),
						value => UpdateNumberVisibility(element, value, conditionalAttribute.IntValue, conditionalAttribute.NumberTest));
					break;
				case ConditionalAttribute.TestType.Float:
					ReflectionHelper.SetupValueSourceCallback(conditionalAttribute.ValueSource, property, element, fieldInfo.DeclaringType, 0.0f, true, nameof(ConditionalAttribute), nameof(ConditionalAttribute.ValueSource),
						value => UpdateNumberVisibility(element, value, conditionalAttribute.FloatValue, conditionalAttribute.NumberTest));
					break;
				case ConditionalAttribute.TestType.String:
					ReflectionHelper.SetupValueSourceCallback(conditionalAttribute.ValueSource, property, element, fieldInfo.DeclaringType, string.Empty, true, nameof(ConditionalAttribute), nameof(ConditionalAttribute.ValueSource),
						value => UpdateStringVisibility(element, value, conditionalAttribute.StringValue, conditionalAttribute.StringTest));
					break;
				case ConditionalAttribute.TestType.Enum:
					ReflectionHelper.SetupValueSourceCallback(conditionalAttribute.ValueSource, property, element, fieldInfo.DeclaringType, default(Enum), true, nameof(ConditionalAttribute), nameof(ConditionalAttribute.ValueSource),
						value => UpdateEnumVisibility(element, value, conditionalAttribute.IntValue, conditionalAttribute.EnumTest));
					break;
				case ConditionalAttribute.TestType.Object:
					ReflectionHelper.SetupValueSourceCallback(conditionalAttribute.ValueSource, property, element, fieldInfo.DeclaringType, (Object)null, true, nameof(ConditionalAttribute), nameof(ConditionalAttribute.ValueSource),
						value => UpdateObjectVisibility(element, value, conditionalAttribute.ObjectTest));
					break;
			}
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