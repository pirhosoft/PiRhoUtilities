using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(MaximumLengthAttribute))]
	class MaximumLengthDrawer : PropertyDrawer
	{
		private const string _invalidDrawerWarning = "(PUMLDID) invalid drawer for MaximumLengthAttribute on field {0}: MaximumLength can only be applied to fields with a TextField";
		private const string _invalidSourceError = "(PUMLDIS) invalid source for MaximumLengthAttribute on field '{0}': an int field, method, or property named '{1}' could not be found";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var element = this.CreateNextElement(property);
			var maxLengthAttribute = attribute as MaximumLengthAttribute;
			var input = element.Q<TextField>();

			if (input != null)
			{
				void setMaxLength(int value) => input.maxLength = value;

				if (!ReflectionHelper.SetupValueSourceCallback(maxLengthAttribute.MaximumLengthSource, fieldInfo.DeclaringType, property, input, maxLengthAttribute.MaximumLength, maxLengthAttribute.AutoUpdate, setMaxLength))
					Debug.LogWarningFormat(_invalidSourceError, property.propertyPath, maxLengthAttribute.MaximumLengthSource);
			}
			else
			{
				Debug.LogWarningFormat(_invalidDrawerWarning, property.propertyPath);
			}

			return element;
		}
	}
}