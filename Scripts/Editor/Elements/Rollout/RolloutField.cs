using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class RolloutField : BindableElement, INotifyValueChanged<bool>
	{
		public string Label { get; set; }
		public string Tooltip { get; set; }

		public RolloutControl Control { get; private set; }

		public RolloutField()
		{
			Control = new RolloutControl(true);
			Add(Control);
		}

		private void Setup(SerializedProperty property)
		{
			Control.Content.Clear();
			Control.IsExpanded = property.isExpanded;
			Control.SetLabel(Label, Tooltip);

			var end = property.GetEndProperty();
			property.NextVisible(true);

			while (!SerializedProperty.EqualContents(property, end))
			{
				var field = new PropertyField(property);
				Control.Content.Add(field);
				property.NextVisible(false);
			}
		}

		#region Binding

		public bool value
		{
			get
			{
				return Control.IsExpanded;
			}
			set
			{
				var previous = Control.IsExpanded;

				if (previous != value)
				{
					SetValueWithoutNotify(value);
					this.SendChangeEvent(Control.IsExpanded, value);
				}
			}
		}

		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);

			if (this.TryGetPropertyBindEvent(evt, out var property))
			{
				// bind to expanded property

				if (property.HasVisibleChildFields())
					Setup(property.Copy());
				//else
				//	Debug.LogErrorFormat(_invalidBindingError, bindingPath, property.propertyPath, property.propertyType);
			}
		}

		public void SetValueWithoutNotify(bool newValue)
		{
			Control.IsExpanded = newValue;
		}

		#endregion

		#region UXML Support

		public new class UxmlFactory : UxmlFactory<RolloutField, UxmlTraits> { }

		public new class UxmlTraits : BindableElement.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);

				var rollout = ve as RolloutField;

				// label, tooltip
			}
		}

		#endregion
	}
}