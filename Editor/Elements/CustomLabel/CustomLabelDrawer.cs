using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(CustomLabelAttribute))]
	class CustomLabelDrawer : PropertyDrawer
	{
		private static readonly string[] _labelClasses = new string[] { PropertyField.labelUssClassName, BaseFieldExtensions.LabelUssClassName, Frame.LabelUssClassName };

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var element = this.CreateNextElement(property);
			var labelAttribute = attribute as CustomLabelAttribute;

			foreach (var className in _labelClasses)
			{
				var label = element.Q<Label>(className: className);

				if (label != null)
				{
					ReflectionHelper.SetupValueSourceCallback(labelAttribute.LabelSource, property, label, fieldInfo.DeclaringType, labelAttribute.Label, labelAttribute.AutoUpdate, nameof(CustomLabelAttribute), nameof(CustomLabelAttribute.LabelSource), value => label.text = value);
					break;
				}
			}

			return element;
		}
	}
}