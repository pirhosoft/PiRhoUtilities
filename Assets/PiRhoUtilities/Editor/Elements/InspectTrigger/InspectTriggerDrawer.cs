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

			var method = ReflectionHelper.CreateActionCallback(inspectAttribute.Method, fieldInfo.DeclaringType, property);

			if (method != null)
			{
				if (!EditorApplication.isPlaying)
					method.Invoke();
			}
			else
			{
				Debug.LogWarningFormat(_invalidMethodWarning, property.propertyPath, inspectAttribute.Method, fieldInfo.DeclaringType.Name);
			}

			return element;
		}
	}
}