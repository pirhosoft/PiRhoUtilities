using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(PopupAttribute))]
	class PopupDrawer : PropertyDrawer
	{
		private const string _invalidTypeError = "(PUPDIT) invalid type for PopupAttribute on field '{0}': Popup can only be applied to int, float, or string fields";
		private const string _invalidValuesSourceError = "(PUPDIVS) invalid value source for PopupAttribute on field '{0}': a field, method, or property of type '{1}' named '{2}' could not be found";
		private const string _invalidOptionsSourceError = "(PUPDIOS) invalid value source for PopupAttribute on field '{0}': a string field, method, or property named '{1}' could not be found";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var popupAttribute = attribute as PopupAttribute;

			if (property.propertyType == SerializedPropertyType.Integer)
				return CreatePopup(property, popupAttribute, popupAttribute.IntValues);
			else if (property.propertyType == SerializedPropertyType.Float)
				return CreatePopup(property, popupAttribute, popupAttribute.FloatValues);
			else if (property.propertyType == SerializedPropertyType.String)
				return CreatePopup(property, popupAttribute, popupAttribute.StringValues);
			else
				Debug.LogErrorFormat(_invalidTypeError, property.propertyPath);

			return new FieldContainer(property.displayName);
		}

		private VisualElement CreatePopup<T>(SerializedProperty property, PopupAttribute popupAttribute, List<T> defaultValues)
		{
			var popup = new PopupField<T>();

			void setValues(List<T> values) => popup.Values = values;
			void setOptions(List<string> options) => popup.Options = options;

			if (!ReflectionHelper.SetupValueSourceCallback(popupAttribute.ValuesSource, fieldInfo.DeclaringType, property, popup, defaultValues, popupAttribute.AutoUpdate, setValues))
				Debug.LogWarningFormat(_invalidValuesSourceError, property.propertyPath, nameof(T), popupAttribute.ValuesSource);

			if (!ReflectionHelper.SetupValueSourceCallback(popupAttribute.OptionsSource, fieldInfo.DeclaringType, property, popup, popupAttribute.Options, popupAttribute.AutoUpdate, setOptions))
				Debug.LogWarningFormat(_invalidOptionsSourceError, property.propertyPath, popupAttribute.OptionsSource);

			return popup.ConfigureProperty(property);
		}
	}
}