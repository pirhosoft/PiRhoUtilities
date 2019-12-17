using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(PopupAttribute))]
	class PopupDrawer : PropertyDrawer
	{
		private const string _invalidTypeError = "(PUPDIT) invalid type for PopupAttribute on field '{0}': Popup can only be applied to int, float, or string fields";

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

			ReflectionHelper.SetupValueSourceCallback<IList<T>>(popupAttribute.ValuesSource, ReflectionSource.All, property, popup, fieldInfo.DeclaringType, defaultValues, popupAttribute.AutoUpdate, nameof(PopupAttribute), nameof(PopupAttribute.ValuesSource), values => popup.Values = values.ToList());
			ReflectionHelper.SetupValueSourceCallback<IList<string>>(popupAttribute.OptionsSource, ReflectionSource.All, property, popup, fieldInfo.DeclaringType, popupAttribute.Options, popupAttribute.AutoUpdate, nameof(PopupAttribute), nameof(PopupAttribute.OptionsSource), options => popup.Options = options.ToList());

			return popup.ConfigureProperty(property);
		}
	}
}