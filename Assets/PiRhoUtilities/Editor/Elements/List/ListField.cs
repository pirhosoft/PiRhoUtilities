using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class ListField : BindableElement
	{
		private const string _invalidBindingError = "(PUEBEIB) invalid binding '{0}' for ListField: property '{1}' is type '{2}' but should be an array";

		public static readonly string UssClassName = "pirho-list-field";

		private ListControl _control;
		private ChangeTriggerControl<int> _sizeBinding;

		public ListField()
		{
		}

		public void Setup(SerializedProperty property, IListProxy proxy)
		{
			bindingPath = property.propertyPath;
			Setup(proxy);
		}

		public void Setup(IListProxy proxy)
		{
			Clear();

			_control = new ListControl(proxy);

			Add(_control);
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
					if (_sizeBinding == null)
						_sizeBinding = new ChangeTriggerControl<int>(null, (oldSize, size) => _control.Refresh());

					Add(_sizeBinding); // setup calls Clear so this always needs to be added
					_sizeBinding.Watch(property.FindPropertyRelative("Array.size"));
				}
				else
				{
					Debug.LogErrorFormat(_invalidBindingError, bindingPath, property.propertyPath, property.propertyType);
				}
			}
		}

		#endregion

		#region UXML Support

		public new class UxmlFactory : UxmlFactory<ListField, UxmlTraits> { }

		public new class UxmlTraits : BindableElement.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);

				var list = ve as ListField;

				// TODO: attributes for proxy properties
				// TODO: if !bindingPath, call Setup with a proxy that owns a list holding objects of type specified as an attribute
			}
		}

		#endregion
	}
}