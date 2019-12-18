using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(SliderAttribute))]
	class SliderDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUSLDIT) invalid type for SliderAttribute on field {0}: Slider can only be applied to int, float, or Vector2 fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var sliderAttribute = attribute as SliderAttribute;

			if (property.propertyType == SerializedPropertyType.Integer)
			{
				var slider = new SliderIntField();

				ReflectionHelper.SetupValueSourceCallback(sliderAttribute.MinimumSource, property, slider, fieldInfo.DeclaringType, Mathf.RoundToInt(sliderAttribute.Minimum), sliderAttribute.AutoUpdate, nameof(SliderAttribute), nameof(SliderAttribute.MinimumSource), minimum => slider.Minimum = minimum);
				ReflectionHelper.SetupValueSourceCallback(sliderAttribute.MinimumSource, property, slider, fieldInfo.DeclaringType, Mathf.RoundToInt(sliderAttribute.Maximum), sliderAttribute.AutoUpdate, nameof(SliderAttribute), nameof(SliderAttribute.MaximumSource), maximum => slider.Maximum = maximum);

				return slider.ConfigureProperty(property);
			}
			else if (property.propertyType == SerializedPropertyType.Float)
			{
				var slider = new SliderFloatField();

				ReflectionHelper.SetupValueSourceCallback(sliderAttribute.MinimumSource, property, slider, fieldInfo.DeclaringType, sliderAttribute.Minimum, sliderAttribute.AutoUpdate, nameof(SliderAttribute), nameof(SliderAttribute.MinimumSource), minimum => slider.Minimum = minimum);
				ReflectionHelper.SetupValueSourceCallback(sliderAttribute.MinimumSource, property, slider, fieldInfo.DeclaringType, sliderAttribute.Maximum, sliderAttribute.AutoUpdate, nameof(SliderAttribute), nameof(SliderAttribute.MaximumSource), maximum => slider.Maximum = maximum);

				return slider.ConfigureProperty(property);
			}
			else if (property.propertyType == SerializedPropertyType.Vector2)
			{
				var slider = new MinMaxSliderField();

				ReflectionHelper.SetupValueSourceCallback(sliderAttribute.MinimumSource, property, slider, fieldInfo.DeclaringType, Mathf.RoundToInt(sliderAttribute.Minimum), sliderAttribute.AutoUpdate, nameof(SliderAttribute), nameof(SliderAttribute.MinimumSource), minimum => slider.Minimum = minimum);
				ReflectionHelper.SetupValueSourceCallback(sliderAttribute.MinimumSource, property, slider, fieldInfo.DeclaringType, Mathf.RoundToInt(sliderAttribute.Maximum), sliderAttribute.AutoUpdate, nameof(SliderAttribute), nameof(SliderAttribute.MaximumSource), maximum => slider.Maximum = maximum);

				return slider.ConfigureProperty(property);
			}
			else
			{
				Debug.LogWarningFormat(property.serializedObject.targetObject, _invalidTypeWarning, property.propertyPath);
				return new FieldContainer(property.displayName);
			}
		}
	}
}