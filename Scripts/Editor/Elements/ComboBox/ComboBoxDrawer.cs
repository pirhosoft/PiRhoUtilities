using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(ComboBoxAttribute))]
	class ComboBoxDrawer : PropertyDrawer
	{
		private const string _invalidTypeError = "(PUCBDIT) invalid type for ComboBoxAttribute on field '{0}': ComboBox can only be applied to string fields";
		private const string _invalidOptionsError = "(PUCBDIOE) invalid values for ComboBoxAttribute on field '{0}': Popup on string fields must specify Options";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			if (attribute is ComboBoxAttribute popup && property.propertyType == SerializedPropertyType.String)
			{
				if (popup.Options != null && popup.Options.Count > 0)
				{
					var field = new ComboBoxField(property.displayName, property.stringValue, popup.Options);
					return field.ConfigureProperty(property, this.GetTooltip());
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
	}
}