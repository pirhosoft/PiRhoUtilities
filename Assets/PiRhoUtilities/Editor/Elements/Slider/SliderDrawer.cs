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
				var element = new SliderIntField(property.displayName, property.intValue, (int)sliderAttribute.Minimum, (int)sliderAttribute.Maximum);
				return element.ConfigureProperty(property, this.GetTooltip());
			}
			else if (property.propertyType == SerializedPropertyType.Float)
			{
				var element = new SliderFloatField(property.displayName, property.floatValue, sliderAttribute.Minimum, sliderAttribute.Maximum);
				return element.ConfigureProperty(property, this.GetTooltip());
			}
			else if (property.propertyType == SerializedPropertyType.Vector2)
			{
				var element = new MinMaxSliderField(property.displayName, property.vector2Value, sliderAttribute.Minimum, sliderAttribute.Maximum);
				return element.ConfigureProperty(property, this.GetTooltip());
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
				return new FieldContainer(property.displayName);
			}
		}
	}
}