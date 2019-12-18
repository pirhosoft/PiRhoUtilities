using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(ComboBoxAttribute))]
	class ComboBoxDrawer : PropertyDrawer
	{
		private const string _invalidTypeError = "(PUCBDIT) invalid type for ComboBoxAttribute on field '{0}': ComboBox can only be applied to string fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			if (property.propertyType == SerializedPropertyType.String)
			{
				var comboBoxAttribute = attribute as ComboBoxAttribute;
				var comboBox = new ComboBoxField();

				ReflectionHelper.SetupValueSourceCallback<IEnumerable<string>>(comboBoxAttribute.OptionsSource, property, comboBox, fieldInfo.DeclaringType, comboBoxAttribute.Options, comboBoxAttribute.AutoUpdate, nameof(ComboBoxAttribute), nameof(ComboBoxAttribute.OptionsSource), options => comboBox.Options = options.ToList());

				return comboBox.ConfigureProperty(property);
			}
			else
			{
				Debug.LogErrorFormat(_invalidTypeError, property.propertyPath);
				return new FieldContainer(property.displayName);
			}
		}
	}
}