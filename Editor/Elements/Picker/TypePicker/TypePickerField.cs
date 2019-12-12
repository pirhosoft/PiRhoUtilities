using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Editor
{
	public class TypePickerField : PickerField<string>
	{
		public new const string Stylesheet = "Picker/TypePicker/TypePickerStyle.uss";
		public new const string UssClassName = "pirho-scene-picker-field";

		private const string _invalidTypeWarning = "(PUTPFIT) Invalid type for TypePickerField: the type '{0}' could not be found";
		private const string _invalidValueWarning = "(PUTPFIT) Failed to set TypePickerField value: '{0}' is not derivable from type '{1}'";

		private TypePickerControl _picker => _control as TypePickerControl;

		public Type Type
		{
			get => _picker.Type;
			set => _picker.SetType(value, ShowAbstract);
		}

		public bool ShowAbstract
		{
			get => _picker.ShowAbstract;
			set => _picker.SetType(Type, value);
		}

		public TypePickerField(string label) : base(label, new TypePickerControl())
		{
			AddToClassList(UssClassName);
		}

		public TypePickerField(string label, Type type, bool showAbstract = false) : this(label)
		{
			_picker.SetType(type, showAbstract);
		}

		public TypePickerField(Type type, bool showAbstract = false) : this(null, type, showAbstract)
		{
		}

		private class TypeProvider : PickerProvider<string> { }
		private class TypePickerControl : PickerControl, IDragReceiver
		{
			public Type Type { get; private set; }
			public bool ShowAbstract { get; private set; }

			private Type _value;
			private string _valueName => _value?.AssemblyQualifiedName ?? string.Empty;

			public TypePickerControl()
			{
				SetLabel(null, GetLabel());

				this.MakeDragReceiver();
			}

			public override void SetValueWithoutNotify(string newValue)
			{
				if (_valueName != newValue)
				{
					var type = GetType(newValue);

					if (!string.IsNullOrEmpty(newValue) && (Type == null || !Type.IsAssignableFrom(type)))
					{
						Debug.LogWarningFormat(_invalidValueWarning, newValue, Type);
					}
					else
					{
						_value = type;

						var icon = GetIcon(type);
						var text = GetLabel();

						SetLabel(icon, text);
					}
				}
			}

			public void SetType(Type type, bool showAbstract)
			{
				if (type != Type || showAbstract != ShowAbstract)
				{
					if (_provider)
						Object.DestroyImmediate(_provider);

					_provider = null;

					Type = type;
					ShowAbstract = showAbstract;

					if (Type == null)
					{
						Debug.LogWarningFormat(_invalidTypeWarning);
					}
					else
					{
						var types = TypeHelper.GetTypeList(Type, showAbstract);

						_provider = ScriptableObject.CreateInstance<TypeProvider>();
						_provider.Setup(types.BaseType.Name, types.Paths.Prepend("None").ToList(), types.Types.Select(t => t.AssemblyQualifiedName).Prepend(string.Empty).ToList(), GetIcon, OnSelected);
					}

					if (Type == null || (_value != null && !Type.IsAssignableFrom(_value)))
						OnSelected(null);
					else
						SetLabel(GetIcon(Type), GetLabel());
				}
			}

			private Type GetType(string typeName)
			{
				return string.IsNullOrEmpty(typeName) ? null : Type.GetType(typeName, false);
			}

			private Texture GetIcon(string typeName)
			{
				var type = GetType(typeName);
				return GetIcon(type);
			}

			private Texture GetIcon(Type type)
			{
				return type == null ? null : AssetPreview.GetMiniTypeThumbnail(type);
			}

			private string GetLabel()
			{
				return _value == null ? $"None ({Type?.Name ?? "No Base Type"})" : _value.Name;
			}

			private void OnSelected(string selected)
			{
				if (_valueName != selected)
					this.SendChangeEvent(_valueName, selected);
			}

			#region IDragReceiver Implementation

			public bool IsDragValid(Object[] objects, object data)
			{
				if (objects.Length > 0)
				{
					var obj = objects[0];
					if (obj != null)
					{
						var drag = obj.GetType();
						return Type != null && Type.IsAssignableFrom(drag);
					}
				}

				return false;
			}

			public void AcceptDrag(Object[] objects, object data)
			{
				OnSelected(objects[0].GetType().AssemblyQualifiedName);
			}

			#endregion
		}

		#region UXML Support

		public TypePickerField() : base(null, null) { }

		public new class UxmlFactory : UxmlFactory<TypePickerField, UxmlTraits> { }
		public new class UxmlTraits : BaseFieldTraits<string, UxmlStringAttributeDescription>
		{
			private readonly UxmlStringAttributeDescription _type = new UxmlStringAttributeDescription { name = "type" };
			private readonly UxmlBoolAttributeDescription _showAbstract = new UxmlBoolAttributeDescription { name = "show-abstract" };

			public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
			{
				var field = element as TypePickerField;
				var typeName = _type.GetValueFromBag(bag, cc);

				if (!string.IsNullOrEmpty(typeName))
					field.Type = Type.GetType(typeName, false);

				field.ShowAbstract = _showAbstract.GetValueFromBag(bag, cc);

				base.Init(element, bag, cc);
			}
		}

		#endregion
	}
}
