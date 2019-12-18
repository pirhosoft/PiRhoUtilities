using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Editor
{
	public class ObjectPickerField : PickerField<Object>
	{
		#region Class Names

		public new const string Stylesheet = "Picker/ObjectPicker/ObjectPickerStyle.uss";
		public new const string UssClassName = "pirho-object-picker-field";
		public const string InspectUssClassName = UssClassName + "__inspect";

		#endregion

		#region Log Messages

		private const string _invalidTypeWarning = "(PUOPFIT) Invalid type for ObjectPickerField: the type '{0}' must be derived from UnityEngine.Object";
		private const string _invalidValueWarning = "(PUOPFIT) Failed to set ObjectPickerField value: '{0}' is not a object of type '{1}'";

		#endregion

		#region Members

		private ObjectPickerControl Picker => _control as ObjectPickerControl;

		#endregion

		#region Public Interface

		public Type Type
		{
			get => Picker.Type;
			set => Picker.Type = value;
		}

		public ObjectPickerField() : this(null, null)
		{
		}

		public ObjectPickerField(string label) : base(label, new ObjectPickerControl())
		{
			AddToClassList(UssClassName);
		}

		public ObjectPickerField(string label, Type type) : this(label)
		{
			Type = type;
		}

		public ObjectPickerField(Type type) : this(null, type)
		{
		}

		#endregion

		#region Visual Input

		private class ObjectProvider : PickerProvider<Object> { }
		private class ObjectPickerControl : PickerControl, IDragReceiver
		{
			private Type _type;
			public Type Type
			{
				get => _type;
				set => SetType(value);
			}

			private readonly IconButton _inspect;
			private Object _value;

			public ObjectPickerControl()
			{
				_inspect = new IconButton(Icon.Inspect.Texture, "View this object in the inspector", Inspect);
				_inspect.AddToClassList(InspectUssClassName);
				_inspect.SetEnabled(false);

				Add(_inspect);
				SetLabel(null, GetLabel());

				this.MakeDragReceiver();
			}

			public override void SetValueWithoutNotify(Object newValue)
			{
				if (_value != newValue)
				{
					if (newValue && (Type == null || !Type.IsAssignableFrom(newValue.GetType())))
					{
						Debug.LogWarningFormat(_invalidValueWarning, newValue.name, Type);
					}
					else
					{
						_value = newValue;
						_inspect.SetEnabled(_value);

						var icon = GetIcon(_value);
						var label = GetLabel();

						SetLabel(icon, label);
					}
				}
			}

			private void SetType(Type type)
			{
				if (type != _type)
				{
					if (_provider)
						Object.DestroyImmediate(_provider);

					_provider = null;
					_type = type;

					if (_type == null || !(typeof(Object).IsAssignableFrom(_type)))
					{
						Debug.LogWarningFormat(_invalidTypeWarning, _type);
					}
					else
					{
						_provider = ScriptableObject.CreateInstance<ObjectProvider>();

						if (typeof(Component).IsAssignableFrom(_type) || typeof(GameObject) == _type)
						{
							var objects = ObjectHelper.GetObjectList(_type, true);
							_provider.Setup(type.Name, objects.Paths.Prepend("None").ToList(), objects.Objects.Prepend(null).ToList(), GetIcon, OnSelected);
						}
						else
						{
							var databaseAssets = AssetHelper.GetAssetList(_type);

							var paths = databaseAssets.Paths.Prepend("None");
							var assets = databaseAssets.Assets.Prepend(null);

							_provider.Setup(_type.Name, paths.ToList(), assets.ToList(), GetIcon, OnSelected);
						}
					}

					if (_type == null || (_value && !_type.IsAssignableFrom(_value.GetType())))
						OnSelected(null);
					else
						SetLabel(GetIcon(_value), GetLabel());
				}
			}

			private Texture GetIcon(Object obj)
			{
				if (obj)
					return AssetPreview.GetMiniThumbnail(obj);

				return Type != null ? AssetPreview.GetMiniTypeThumbnail(Type) : null;
			}

			private string GetLabel()
			{
				return _value ? _value.name : $"None ({Type?.Name ?? "Typeless"})";
			}

			private void OnSelected(Object selected)
			{
				if (_value != selected)
					this.SendChangeEvent(_value, selected);
			}

			private void Inspect()
			{
				if (_value)
					Selection.activeObject = _value;
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
				OnSelected(objects[0]);
			}

			#endregion
		}

		#endregion

		#region UXML Support

		public new class UxmlFactory : UxmlFactory<ObjectPickerField, UxmlTraits> { }
		public new class UxmlTraits : BaseField<Object>.UxmlTraits
		{
			private readonly UxmlStringAttributeDescription _type = new UxmlStringAttributeDescription { name = "type" };
			private readonly UxmlStringAttributeDescription _value = new UxmlStringAttributeDescription { name = "value" };

			public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(element, bag, cc);

				var field = element as ObjectPickerField;
				var typeName = _type.GetValueFromBag(bag, cc);
				var valueName = _value.GetValueFromBag(bag, cc);

				if (!string.IsNullOrEmpty(typeName))
					field.Type = Type.GetType(typeName, false);

				if (!string.IsNullOrEmpty(valueName))
					field.SetValueWithoutNotify(AssetDatabase.LoadAssetAtPath(valueName, field.Type));
			}
		}

		#endregion
	}
}
