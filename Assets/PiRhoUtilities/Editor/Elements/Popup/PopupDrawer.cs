using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(PopupAttribute))]
	class PopupDrawer : PropertyDrawer
	{
		private const string _invalidTypeError = "(PUPDIT) invalid type for PopupAttribute on field '{0}': Popup can only be applied to int, float, or string fields";
		private const string _invalidIntValuesError = "(PUPDIIV) invalid values for PopupAttribute on field '{0}': Popup on int fields must specify IntValues";
		private const string _invalidFloatValuesError = "(PUPDIFV) invalid values for PopupAttribute on field '{0}': Popup on float fields must specify FloatValues";
		private const string _invalidOptionsError = "(PUPDIOE) invalid values for PopupAttribute on field '{0}': Popup on string fields must specify Options";
		private const string _invalidOptionsWarning = "(PUPDIOW) invalid options for PopupAttribute on field '{0}': the number of Options does not match the number of Values";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var popup = attribute as PopupAttribute;
			var tooltip = this.GetTooltip();

			if (property.propertyType == SerializedPropertyType.Integer)
			{
				if (popup.IntValues != null && popup.IntValues.Count > 0)
				{
					if (popup.Options != null && popup.Options.Count != popup.IntValues.Count)
						Debug.LogWarningFormat(_invalidOptionsWarning, property.propertyPath);

					var field = new PopupField<int>(property.displayName, popup.IntValues, defaultIndex: 0, FormatInt, FormatInt);
					return field.ConfigureProperty(property, tooltip);
				}
				else
				{
					Debug.LogErrorFormat(_invalidIntValuesError, property.propertyPath);
				}
			}
			else if (property.propertyType == SerializedPropertyType.Float)
			{
				if (popup.FloatValues != null && popup.FloatValues.Count > 0)
				{
					if (popup.Options != null && popup.Options.Count != popup.FloatValues.Count)
						Debug.LogWarningFormat(_invalidOptionsWarning, property.propertyPath);

					var field = new PopupField<float>(property.displayName, popup.FloatValues, defaultIndex: 0, FormatFloat, FormatFloat);
					return field.ConfigureProperty(property, tooltip);
				}
				else
				{
					Debug.LogErrorFormat(_invalidFloatValuesError, property.propertyPath);
				}
			}
			else if (property.propertyType == SerializedPropertyType.String)
			{
				if (popup.Options != null && popup.Options.Count > 0)
				{
					var field = new PopupField<string>(property.displayName, popup.Options, 0);
					return field.ConfigureProperty(property, tooltip);
				}
				else
				{
					Debug.LogErrorFormat(_invalidOptionsError, property.propertyPath);
				}
			}
			else
			{
				Debug.LogErrorFormat(_invalidTypeError, property.propertyPath);
			}

			return new FieldContainer(property.displayName);
		}

		private string FormatInt(int value)
		{
			var popup = attribute as PopupAttribute;
			var index = popup.IntValues.IndexOf(value);

			if (popup.Options == null || index < 0 || index >= popup.Options.Count)
				return value.ToString();

			return popup.Options[index];
		}

		private string FormatFloat(float value)
		{
			var popup = attribute as PopupAttribute;
			var index = popup.FloatValues.IndexOf(value);

			if (popup.Options == null || index < 0 || index >= popup.Options.Count)
				return value.ToString();

			return popup.Options[index];
		}
	}
}