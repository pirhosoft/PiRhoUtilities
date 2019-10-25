using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class DictionaryField : BindableElement
	{
		private const string _invalidBindingError = "(PUEBEIB) invalid binding '{0}' for DictionaryField: property '{1}' is type '{2}' but should be an array";

		public const string UssClassName = "pirho-dictionary-field";

		public DictionaryControl Control { get; private set; }

		public void Setup(SerializedProperty keysProperty, IDictionaryProxy proxy)
		{
			bindingPath = keysProperty.propertyPath;
			Setup(proxy);
		}

		private void Setup(IDictionaryProxy proxy)
		{
			Clear();

			Control = new DictionaryControl(proxy);

			Add(Control);
			AddToClassList(UssClassName);
		}

		#region Binding

		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);

			if (this.TryGetPropertyBindEvent(evt, out var property))
			{
				if (property.isArray)
				{
					var sizeBinding = new ChangeTriggerControl<int>(null, (oldSize, size) => Control.Refresh());
					sizeBinding.Watch(property.FindPropertyRelative("Array.size"));

					Add(sizeBinding);
				}
				else
				{
					Debug.LogErrorFormat(_invalidBindingError, bindingPath, property.propertyPath, property.propertyType);
				}
			}
		}

		#endregion

		#region UXML Support

		public new class UxmlFactory : UxmlFactory<DictionaryField, UxmlTraits> { }

		public new class UxmlTraits : BindableElement.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);

				var list = ve as DictionaryField;

				// TODO: attributes for proxy properties
				// TODO: if !bindingPath, call Setup with a proxy that owns a list holding objects of type specified as an attribute
			}
		}

		#endregion
	}
}