using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(InspectTriggerAttribute))]
	class InspectTriggerDrawer : PropertyDrawer
	{
		private const string _invalidMethodWarning = "(PUITDIM) invalid method for InspectTriggerAttribute on field '{0}': a parameterless method named '{1}' colud not be found on type '{2}'";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var element = this.CreateNextElement(property);
			var inspectAttribute = attribute as InspectTriggerAttribute;

			var method = ReflectionHelper.CreateActionCallback(property, fieldInfo.DeclaringType, inspectAttribute.Method, nameof(InspectTriggerAttribute), nameof(InspectTriggerAttribute.Method));

			if (method == null)
			{
				Debug.LogWarningFormat(_invalidMethodWarning, property.propertyPath, inspectAttribute.Method, fieldInfo.DeclaringType.Name);
			}
			else
			{
				if (!EditorApplication.isPlaying)
					method.Invoke();
			}

			return element;
		}
	}
}