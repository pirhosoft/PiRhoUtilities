using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(ValidateAttribute))]
	class ValidateDrawer : PropertyDrawer
	{
		public const string Stylesheet = "Validate/ValidateDrawer.uss";
		public const string UssClassName = "pirho-validate";
		public const string MessageBoxUssClassName = UssClassName + "__message-box";

		private const string _missingMethodWarning = "(PUVDMM) invalid method for ValidateAttribute on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _invalidMethodReturnWarning = "(PUVDIMR) invalid method for ValidateAttribute on field '{0}': the method '{1}' should return a bool";
		private const string _invalidMethodParametersWarning = "(PUVDIMP) invalid method for ValidateAttribute on field '{0}': the method '{1}' should take no parameters";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var element = this.CreateNextElement(property);

			var validate = new VisualElement();
			validate.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
			validate.AddToClassList(UssClassName);

			var validateAttribute = attribute as ValidateAttribute;
			var message = new MessageBox((MessageBoxType)(int)validateAttribute.Type, validateAttribute.Message);
			message.AddToClassList(MessageBoxUssClassName);

			validate.Add(element);
			validate.Add(message);

			var method = GetMethod(property, validateAttribute);

			if (method != null)
			{
				var owner = method.IsStatic ? null : property.GetOwner<object>();
				var change = ChangeTriggerControl.Create(property, () => Update(message, method, owner));
				Update(message, method, owner);
				element.Add(change);
			}

			return validate;
		}

		private void Update(VisualElement message, MethodInfo method, object owner)
		{
			var valid = (bool)method.Invoke(owner, null);
			message.SetDisplayed(!valid);
		}

		private MethodInfo GetMethod(SerializedProperty property, ValidateAttribute validateAttribute)
		{
			var method = fieldInfo.DeclaringType.GetMethod(validateAttribute.Method, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			if (method != null)
			{
				if (method.ReturnType != typeof(bool))
					Debug.LogWarningFormat(_invalidMethodReturnWarning, property.propertyPath, validateAttribute.Method);
				else if (!method.HasSignature(null))
					Debug.LogWarningFormat(_invalidMethodParametersWarning, property.propertyPath, validateAttribute.Method);
				else
					return method;
			}
			else
			{
				Debug.LogWarningFormat(_missingMethodWarning, property.propertyPath, validateAttribute.Method, fieldInfo.DeclaringType.Name);
			}

			return null;
		}
	}
}