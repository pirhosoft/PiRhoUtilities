using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public abstract class PropertyWatcher : BindableElement
	{
		public SerializedProperty Property { get; private set; }

		public virtual void Watch(SerializedProperty property)
		{
			if (this.IsBound())
				binding.Release();

			Property = property;

			if (property != null)
				this.BindProperty(property);
		}

		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);

			if (this.TryGetPropertyBindEvent(evt, out var property))
			{
				if (!SerializedProperty.EqualContents(Property, property))
				{
					Watch(property);
					evt.StopPropagation();
				}
			}
		}
	}

	public abstract class PropertyWatcher<T> : PropertyWatcher, INotifyValueChanged<T>
	{
		#region Log Messages

		private const string _invalidWatcherError = "(PUEPWIW) invalid type '{0}' for PropertyWatcher: PropertyWatcher can only be used with types that have a corresponding SerializedPropertyType";
		private const string _invalidPropertyError = "(PUEPWIP) invalid property '{0}' for PropertyWatcher: the property is type '{1}' but should be type '{2}'";

		#endregion

		private T _value;

		public T value
		{
			get => _value;
			set
			{
				var previous = _value;
				SetValueWithoutNotify(value);
				OnChanged(Property, previous, value);
			}
		}

		public void SetValueWithoutNotify(T newValue)
		{
			_value = newValue;
		}

		public override void Watch(SerializedProperty property)
		{
			if (property != null)
			{
				if (!property.TryGetValue(out _value))
				{
					var requiredType = SerializedPropertyExtensions.GetPropertyType<T>();

					if (requiredType == SerializedPropertyType.Generic)
						Debug.LogWarningFormat(_invalidWatcherError, typeof(T).Name); // TODO: this will also trigger when T is intended to be a ManagedReference type
					else
						Debug.LogWarningFormat(_invalidPropertyError, property.propertyPath, property.propertyType, requiredType);

					base.Watch(null);
				}
			}

			base.Watch(property);
		}

		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);

			if (!evt.isPropagationStopped && this.TryGetPropertyBindEvent(evt, out var property))
			{
				if (this is INotifyValueChanged<Enum> enumThis)
				{
					BindingExtensions.DefaultEnumBind(enumThis, property);
					evt.StopPropagation();
				}
				else if (property.propertyType == SerializedPropertyType.ManagedReference)
				{
					var type = property.GetManagedReferenceFieldType();
					BindingExtensions.BindManagedReference(this, property, null);
					evt.StopPropagation();
				}
			}
		}

		protected abstract void OnChanged(SerializedProperty property, T previousValue, T newValue);
	}
}
