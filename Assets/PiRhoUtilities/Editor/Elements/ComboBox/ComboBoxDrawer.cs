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
		private const string _invalidOptionsError = "(PUCBDIO) invalid options source for ComboBoxAttribute on field '{0}': a List<string> field, method, or property nameed '{1}' could not be found";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			if (property.propertyType == SerializedPropertyType.String)
			{
				var comboBoxAttribute = attribute as ComboBoxAttribute;
				var comboBox = new ComboBoxField();

				void options(IEnumerable<string> value) => comboBox.Options = value.ToList();

				if (!ReflectionHelper.SetupValueSourceCallback(comboBoxAttribute.OptionsSource, fieldInfo.DeclaringType, property, comboBox, comboBoxAttribute.Options, comboBoxAttribute.AutoUpdate, options))
					Debug.LogWarningFormat(_invalidOptionsError, property.propertyPath, comboBoxAttribute.OptionsSource);

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