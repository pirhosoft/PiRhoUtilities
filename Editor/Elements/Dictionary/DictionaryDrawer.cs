using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(DictionaryAttribute))]
	class DictionaryDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUDDIT) invalid type for DictionaryAttribute on field '{0}': Dictionary can only be applied to SerializedDictionary fields";
		private const string _invalidAddCallbackWarning = "(PUDDIAC) invalid add callback for DictionaryAttribute on field '{0}': The method must accept a string or have no parameters";
		private const string _invalidAddReferenceCallbackWarning = "(PUDDIAC) invalid add callback for DictionaryAttribute on field '{0}': The method must accept a string and/or an object or have no parameters";
		private const string _invalidRemoveCallbackWarning = "(PUDDIRMC) invalid remove callback for DictionaryAttribute on field '{0}': The method must accept an string or have no parameters";
		private const string _invalidReorderCallbackWarning = "(PUDDIROC) invalid reorder callback for DictionaryAttribute on field '{0}': The method must accept two ints or have no parameters";
		private const string _invalidChangeCallbackWarning = "(PUDDICC) invalid change callback for DictionaryAttribute on field '{0}': The method must have no parameters";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var keys = property.FindPropertyRelative(SerializedDictionary<string, int>.KeyProperty);
			var values = property.FindPropertyRelative(SerializedDictionary<string, int>.ValueProperty);

			if (keys != null && keys.isArray && values != null && values.isArray && keys.arrayElementType == "string")
			{
				var isReference = fieldInfo.FieldType.BaseType.GetGenericTypeDefinition() == typeof(ReferenceDictionary<,>);
				var referenceType = isReference ? fieldInfo.GetFieldType() : null;
				var declaringType = fieldInfo.DeclaringType;
				var dictionaryAttribute = attribute as DictionaryAttribute;
				var drawer = this.GetNextDrawer();
				var proxy = new PropertyDictionaryProxy(property, keys, values, drawer);

				var field = new DictionaryField();
				field.SetItemType(referenceType, true);
				field.Proxy = proxy;
				field.bindingPath = property.propertyPath;
				// TODO: other stuff from ConfigureField

				if (!string.IsNullOrEmpty(dictionaryAttribute.AddPlaceholder))
					field.AddPlaceholder = dictionaryAttribute.AddPlaceholder;

				if (!string.IsNullOrEmpty(dictionaryAttribute.EmptyLabel))
					field.EmptyLabel = dictionaryAttribute.EmptyLabel;

				field.AllowAdd = dictionaryAttribute.AllowAdd != DictionaryAttribute.Never;
				field.AllowRemove = dictionaryAttribute.AllowRemove != DictionaryAttribute.Never;
				field.AllowReorder = dictionaryAttribute.AllowReorder;

				SetupAdd(dictionaryAttribute, proxy, field, property, declaringType, isReference);
				SetupRemove(dictionaryAttribute, proxy, field, property, declaringType);
				SetupReorder(dictionaryAttribute, field, property, declaringType);
				SetupChange(dictionaryAttribute, field, property, declaringType);

				return field;
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
				return new FieldContainer(property.displayName);
			}
		}

		private void SetupAdd(DictionaryAttribute dictionaryAttribute, PropertyDictionaryProxy proxy, DictionaryField field, SerializedProperty property, Type declaringType, bool isReference)
		{
			if (field.AllowAdd)
			{
				if (!string.IsNullOrEmpty(dictionaryAttribute.AllowAdd))
				{
					proxy.CanAddCallback = ReflectionHelper.CreateFunctionCallback<string, bool>(property, declaringType, dictionaryAttribute.AllowAdd, nameof(DictionaryAttribute), nameof(DictionaryAttribute.AllowRemove));
					if (proxy.CanAddCallback == null)
					{
						var canRemove = ReflectionHelper.CreateValueSourceFunction(property, field, declaringType, dictionaryAttribute.AllowAdd, true, nameof(DictionaryAttribute), nameof(DictionaryAttribute.AllowRemove));
						proxy.CanAddCallback = index => canRemove();
					}
				}

				if (!string.IsNullOrEmpty(dictionaryAttribute.AddCallback))
				{
					if (!isReference)
					{
						var addCallback = ReflectionHelper.CreateActionCallback(property, declaringType, dictionaryAttribute.AddCallback, nameof(DictionaryAttribute), nameof(DictionaryAttribute.AddCallback));
						if (addCallback != null)
						{
							field.RegisterCallback<DictionaryField.ItemAddedEvent>(evt => addCallback.Invoke());
						}
						else
						{
							var addCallbackKey = ReflectionHelper.CreateActionCallback<string>(property, declaringType, dictionaryAttribute.AddCallback, nameof(DictionaryAttribute), nameof(DictionaryAttribute.AddCallback));
							if (addCallbackKey != null)
								field.RegisterCallback<DictionaryField.ItemAddedEvent>(evt => addCallbackKey.Invoke(evt.Key));
							else
								Debug.LogWarningFormat(_invalidAddCallbackWarning, property.propertyPath);
						}
					}
					else
					{
						var addCallback = ReflectionHelper.CreateActionCallback(property, declaringType, dictionaryAttribute.AddCallback, nameof(DictionaryAttribute), nameof(DictionaryAttribute.AddCallback));
						if (addCallback != null)
						{
							field.RegisterCallback<DictionaryField.ItemAddedEvent>(evt => addCallback.Invoke());
						}
						else
						{
							var addCallbackKey = ReflectionHelper.CreateActionCallback<string>(property, declaringType, dictionaryAttribute.AddCallback, nameof(DictionaryAttribute), nameof(DictionaryAttribute.AddCallback));
							if (addCallbackKey != null)
							{
								field.RegisterCallback<DictionaryField.ItemAddedEvent>(evt => addCallbackKey.Invoke(evt.Key));
							}
							else
							{
								var addCallbackObject = ReflectionHelper.CreateActionCallback<object>(property, declaringType, dictionaryAttribute.AddCallback, nameof(DictionaryAttribute), nameof(DictionaryAttribute.AddCallback));
								if (addCallbackObject != null)
								{
									field.RegisterCallback<DictionaryField.ItemAddedEvent>(evt => addCallbackObject.Invoke(evt.Item));
								}
								else
								{
									var addCallbackKeyObject = ReflectionHelper.CreateActionCallback<string, object>(property, declaringType, dictionaryAttribute.AddCallback, nameof(DictionaryAttribute), nameof(DictionaryAttribute.AddCallback));
									if (addCallbackKeyObject != null)
										field.RegisterCallback<DictionaryField.ItemAddedEvent>(evt => addCallbackKeyObject.Invoke(evt.Key, evt.Item));
									else
										Debug.LogWarningFormat(_invalidAddReferenceCallbackWarning, property.propertyPath);
								}
							}
						}
					}
				}
			}
		}

		private void SetupRemove(DictionaryAttribute dictionaryAttribute, PropertyDictionaryProxy proxy, DictionaryField field, SerializedProperty property, Type declaringType)
		{
			if (field.AllowRemove)
			{
				if (!string.IsNullOrEmpty(dictionaryAttribute.AllowRemove))
				{
					proxy.CanRemoveCallback = ReflectionHelper.CreateFunctionCallback<string, bool>(property, declaringType, dictionaryAttribute.AllowRemove, nameof(DictionaryAttribute), nameof(DictionaryAttribute.AllowRemove));
					if (proxy.CanRemoveCallback == null)
					{
						var canRemove = ReflectionHelper.CreateValueSourceFunction(property, field, declaringType, dictionaryAttribute.AllowRemove, true, nameof(DictionaryAttribute), nameof(DictionaryAttribute.AllowRemove));
						proxy.CanRemoveCallback = index => canRemove();
					}
				}

				if (!string.IsNullOrEmpty(dictionaryAttribute.RemoveCallback))
				{
					var removeCallback = ReflectionHelper.CreateActionCallback(property, declaringType, dictionaryAttribute.RemoveCallback, nameof(DictionaryAttribute), nameof(DictionaryAttribute.RemoveCallback));
					if (removeCallback != null)
					{
						field.RegisterCallback<DictionaryField.ItemRemovedEvent>(evt => removeCallback.Invoke());
					}
					else
					{
						var removeCallbackKey = ReflectionHelper.CreateActionCallback<string>(property, declaringType, dictionaryAttribute.RemoveCallback, nameof(DictionaryAttribute), nameof(DictionaryAttribute.RemoveCallback));
						if (removeCallbackKey != null)
							field.RegisterCallback<DictionaryField.ItemRemovedEvent>(evt => removeCallbackKey.Invoke(evt.Key));
						else
							Debug.LogWarningFormat(_invalidRemoveCallbackWarning, property.propertyPath);
					}
				}
			}
		}

		private void SetupReorder(DictionaryAttribute dictionaryAttribute, DictionaryField field, SerializedProperty property, Type declaringType)
		{
			if (field.AllowReorder)
			{
				if (!string.IsNullOrEmpty(dictionaryAttribute.ReorderCallback))
				{
					var reorderCallback = ReflectionHelper.CreateActionCallback(property, declaringType, dictionaryAttribute.ReorderCallback, nameof(DictionaryAttribute), nameof(DictionaryAttribute.ReorderCallback));
					if (reorderCallback != null)
					{
						field.RegisterCallback<DictionaryField.ItemReorderedEvent>(evt => reorderCallback.Invoke());
					}
					else
					{
						var reorderCallbackFromTo = ReflectionHelper.CreateActionCallback<int, int>(property, declaringType, dictionaryAttribute.ReorderCallback, nameof(DictionaryAttribute), nameof(DictionaryAttribute.ReorderCallback));
						if (reorderCallbackFromTo != null)
							field.RegisterCallback<DictionaryField.ItemReorderedEvent>(evt => reorderCallbackFromTo.Invoke(evt.FromIndex, evt.ToIndex));
						else
							Debug.LogWarningFormat(_invalidReorderCallbackWarning, property.propertyPath);
					}
				}
			}
		}

		private void SetupChange(DictionaryAttribute dictionaryAttribute, DictionaryField field, SerializedProperty property, Type declaringType)
		{
			if (!string.IsNullOrEmpty(dictionaryAttribute.ChangeCallback))
			{
				var changeCallback = ReflectionHelper.CreateActionCallback(property, declaringType, dictionaryAttribute.ChangeCallback, nameof(DictionaryAttribute), nameof(DictionaryAttribute.AllowRemove));
				if (changeCallback != null)
					field.RegisterCallback<DictionaryField.ItemsChangedEvent>(evt => changeCallback.Invoke());
				else
					Debug.LogWarningFormat(_invalidChangeCallbackWarning, property.propertyPath);
			}
		}
	}
}
