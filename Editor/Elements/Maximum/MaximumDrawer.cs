using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(MaximumAttribute))]
	class MaximumDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUMXDIT) invalid type for MaximumAttribute on field {0}: Maximum can only be applied to int or float fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var maximum = (attribute as MaximumAttribute).Maximum;
			var element = this.CreateNextElement(property);
			var clone = property.Copy();

			if (property.propertyType == SerializedPropertyType.Integer)
			{
				Clamp(property, Mathf.RoundToInt(maximum));
				element.RegisterCallback<FocusOutEvent>(e => Clamp(clone, Mathf.RoundToInt(maximum)));
			}
			else if (property.propertyType == SerializedPropertyType.Float)
			{
				Clamp(property, maximum);
				element.RegisterCallback<FocusOutEvent>(e => Clamp(clone, maximum));
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
			}

			return element;
		}

		private void Clamp(SerializedProperty property, int maximum)
		{
			property.intValue = Mathf.Min(maximum, property.intValue);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void Clamp(SerializedProperty property, float maximum)
		{
			property.floatValue = Mathf.Min(maximum, property.floatValue);
			property.serializedObject.ApplyModifiedProperties();
		}
	}
}