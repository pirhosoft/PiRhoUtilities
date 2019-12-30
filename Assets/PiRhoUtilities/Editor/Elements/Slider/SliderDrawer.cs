using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(SliderAttribute))]
	class SliderDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUSLDIT) invalid type for SliderAttribute on field {0}: Slider can only be applied to int, float, or Vector2 fields";
		private const string _invalidMinimumSourceError = "(PUSLDIMNS) invalid minimum source for SliderAttribute on field '{0}': a field, method, or property of type '{1}' named '{2}' could not be found";
		private const string _invalidMaximumSourceError = "(PUSLDIMXS) invalid maximum source for SliderAttribute on field '{0}': a field, method, or property of type '{1}' named '{2}' could not be found";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var sliderAttribute = attribute as SliderAttribute;

			if (property.propertyType == SerializedPropertyType.Integer)
				return CreateSlider(new SliderIntField(), sliderAttribute, property, Mathf.RoundToInt(sliderAttribute.Minimum), Mathf.RoundToInt(sliderAttribute.Maximum));
			else if (property.propertyType == SerializedPropertyType.Float)
				return CreateSlider(new SliderFloatField(), sliderAttribute, property, sliderAttribute.Minimum, sliderAttribute.Maximum);
			else if (property.propertyType == SerializedPropertyType.Vector2)
				return CreateMinMaxSlider(new MinMaxSliderField(), sliderAttribute, property,sliderAttribute.Minimum, sliderAttribute.Maximum);
			else
				Debug.LogWarningFormat(property.serializedObject.targetObject, _invalidTypeWarning, property.propertyPath);

			return new FieldContainer(property.displayName);
		}

		private VisualElement CreateSlider<T>(SliderField<T> slider, SliderAttribute sliderAttribute, SerializedProperty property, T defaultMinimum, T defaultMaximum)
		{
			void setMin(T value) => slider.Minimum = value;
			void setMax(T value) => slider.Maximum = value;

			if (!ReflectionHelper.SetupValueSourceCallback(sliderAttribute.MinimumSource, fieldInfo.DeclaringType, property, slider, defaultMinimum, sliderAttribute.AutoUpdate, setMin))
				Debug.LogWarningFormat(_invalidMinimumSourceError, property.propertyPath, nameof(T), sliderAttribute.MinimumSource);

			if (!ReflectionHelper.SetupValueSourceCallback(sliderAttribute.MaximumSource, fieldInfo.DeclaringType, property, slider, defaultMaximum, sliderAttribute.AutoUpdate, setMax))
				Debug.LogWarningFormat(_invalidMaximumSourceError, property.propertyPath, nameof(T), sliderAttribute.MaximumSource);

			return slider.ConfigureProperty(property);
		}

		private VisualElement CreateMinMaxSlider(MinMaxSliderField slider, SliderAttribute sliderAttribute, SerializedProperty property, float defaultMinimum, float defaultMaximum)
		{
			void setMin(float value) => slider.MinimumLimit = value;
			void setMax(float value) => slider.MaximumLimit = value;

			if (!ReflectionHelper.SetupValueSourceCallback(sliderAttribute.MinimumSource, fieldInfo.DeclaringType, property, slider, defaultMinimum, sliderAttribute.AutoUpdate, setMin))
				Debug.LogWarningFormat(_invalidMinimumSourceError, property.propertyPath, "float", sliderAttribute.MinimumSource);

			if (!ReflectionHelper.SetupValueSourceCallback(sliderAttribute.MaximumSource, fieldInfo.DeclaringType, property, slider, defaultMaximum, sliderAttribute.AutoUpdate, setMax))
				Debug.LogWarningFormat(_invalidMaximumSourceError, property.propertyPath, "float", sliderAttribute.MaximumSource);

			return slider.ConfigureProperty(property);
		}
	}
}