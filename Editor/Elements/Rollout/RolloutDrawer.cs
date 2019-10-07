using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(RolloutAttribute))]
	class RolloutDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PURDIT) invalid type for RolloutAttribute on field '{0}': Rollout can only be applied to fields that have child fields (classes or structs)";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			if (property.HasVisibleChildFields())
			{
				return new RolloutField()
				{
					bindingPath = property.propertyPath,
					Label = property.displayName,
					Tooltip = this.GetTooltip()
				};
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
				return new FieldContainer(property.displayName);
			}
		}
	}
}