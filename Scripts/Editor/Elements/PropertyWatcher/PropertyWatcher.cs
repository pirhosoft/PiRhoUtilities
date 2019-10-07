using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public abstract class PropertyWatcher : BindableElement
	{
		public PropertyWatcher(SerializedProperty property)
		{
			style.display = DisplayStyle.None;
			Watch(property);
		}

		public virtual void Watch(SerializedProperty property)
		{
			if (this.IsBound())
				binding.Release();

			if (property != null)
				this.BindProperty(property);
		}
	}

	public abstract class PropertyWatcher<T> : PropertyWatcher, INotifyValueChanged<T>
	{
		private T _value;

		public T value
		{
			get => _value;
			set
			{
				var previous = _value;
				SetValueWithoutNotify(value);
				OnChanged(previous, value);
			}
		}

		public PropertyWatcher(SerializedProperty property) : base(property)
		{
		}

		public void SetValueWithoutNotify(T newValue)
		{
			_value = newValue;
		}

		public override void Watch(SerializedProperty property)
		{
			if (property != null)
				property.TryGetValue(out _value);

			base.Watch(property);
		}

		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);

			if (typeof(T) == typeof(Enum) && this.TryGetPropertyBindEvent(evt, out var property))
			{
				BindingExtensions.DefaultEnumBind(this as INotifyValueChanged<Enum>, property);
				evt.StopPropagation();
			}
		}

		protected abstract void OnChanged(T previousValue, T newValue);
	}
}
