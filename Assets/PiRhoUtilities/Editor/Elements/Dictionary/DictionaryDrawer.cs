using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(DictionaryAttribute))]
	class DictionaryDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUDDIT) invalid type for DictionaryAttribute on field '{0}': Dictionary can only be applied to SerializedDictionary fields";

		private const string _missingAllowAddMethodWarning = "(PUDDMAAM) invalid method for AllowAdd on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _missingAllowRemoveMethodWarning = "(PUDDMARM) invalid method for AllowRemove on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _invalidAllowAddMethodWarning = "(PUDDIAAM) invalid method for AllowAdd on field '{0}': the method '{1}' should take no parameters";
		private const string _invalidAllowRemoveMethodWarning = "(PUDDIARM) invalid method for AllowRemove on field '{0}': the method '{1}' should take an 0 or 1 int parameters";

		private const string _missingAddMethodWarning = "(PUDDMAM) invalid method for AddCallback on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _missingRemoveMethodWarning = "(PUDDMRM) invalid method for RemoveCallback on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _missingReorderMethodWarning = "(PUDDMROM) invalid method for ReorderCallback on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _missingChangeMethodWarning = "(PUDDMCM) invalid method for ChangeCallback on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _invalidAddMethodWarning = "(PUDDIAM) invalid method for AddCallback on field '{0}': the method '{1}' should take 0 or 1 string parameters";
		private const string _invalidRemoveMethodWarning = "(PUDDIRM) invalid method for RemoveCallback on field '{0}': the method '{1}' should take 0 or 1 string parameters";
		private const string _invalidReorderMethodWarning = "(PUDDIROM) invalid method for ReorderCallback on field '{0}': the method '{1}' should take 0, 1, or 2 int parameters";
		private const string _invalidChangeMethodWarning = "(PUDDICM) invalid method for ChangeCallback on field '{0}': the method '{1}' should take no parameters";

		private static readonly object[] _oneParameter = new object[1];
		private static readonly object[] _twoParameters = new object[2];

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var keys = property.FindPropertyRelative(SerializedDictionary<string, int>.KeyProperty);
			var values = property.FindPropertyRelative(SerializedDictionary<string, int>.ValueProperty);

			if (keys != null && keys.isArray && values != null && values.isArray && keys.arrayElementType == "string")
			{
				var isReference = fieldInfo.FieldType.BaseType.GetGenericTypeDefinition() == typeof(ReferenceDictionary<,>);
				var referenceType = isReference ? fieldInfo.GetFieldType() : null;
				var dictionaryAttribute = attribute as DictionaryAttribute;
				var parent = property.GetOwner<object>();
				var drawer = this.GetNextDrawer();
				var proxy = new PropertyDictionaryProxy(property, keys, values, drawer);
				var path = property.propertyPath;

				var field = new DictionaryField();
				field.SetProxy(proxy, referenceType, true);
				field.bindingPath = property.propertyPath;
				// TODO: other stuff from ConfigureField

				if (!string.IsNullOrEmpty(dictionaryAttribute.EmptyLabel))
					field.EmptyLabel = dictionaryAttribute.EmptyLabel;

				field.AllowAdd = dictionaryAttribute.AllowAdd != DictionaryAttribute.Never;
				field.AllowRemove = dictionaryAttribute.AllowRemove != DictionaryAttribute.Never;
				field.AllowReorder = dictionaryAttribute.AllowReorder;

				if (TryGetMethod(dictionaryAttribute.AllowAdd, _missingAllowAddMethodWarning, path, out var allowAddMethod))
					AddConditional(proxy.CanAddKeyCallback, parent, allowAddMethod, _invalidAllowAddMethodWarning, path);

				if (TryGetMethod(dictionaryAttribute.AllowRemove, _missingAllowRemoveMethodWarning, path, out var allowRemoveMethod))
					AddConditional(proxy.CanRemoveCallback, parent, allowRemoveMethod, _invalidAllowRemoveMethodWarning, path);

				if (TryGetMethod(dictionaryAttribute.AddCallback, _missingAddMethodWarning, path, out var addMethod))
					SetupAddCallback(field, parent, addMethod, _invalidAddMethodWarning, path);

				if (TryGetMethod(dictionaryAttribute.RemoveCallback, _missingRemoveMethodWarning, path, out var removeMethod))
					SetupRemoveCallback(field, parent, removeMethod, _invalidRemoveMethodWarning, path);

				if (TryGetMethod(dictionaryAttribute.ReorderCallback, _missingReorderMethodWarning, path, out var reorderMethod))
					SetupReorderCallback(field, parent, reorderMethod, _invalidReorderMethodWarning, path);

				if (TryGetMethod(dictionaryAttribute.ChangeCallback, _missingChangeMethodWarning, path, out var changeMethod))
					SetupChangeCallback(field, parent, changeMethod, _invalidChangeMethodWarning, path);

				return field;
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
				return new FieldContainer(property.displayName);
			}
		}

		private bool TryGetMethod(string name, string warning, string propertyPath, out MethodInfo method)
		{
			method = null;

			if (!string.IsNullOrEmpty(name))
			{
				method = fieldInfo.DeclaringType.GetMethod(name, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (method == null)
					Debug.LogWarningFormat(warning, propertyPath, name, fieldInfo.DeclaringType.Name);
			}

			return method != null;
		}

		private void SetupAddCallback(DictionaryField field, object parent, MethodInfo method, string warning, string propertyPath)
		{
			var owner = method.IsStatic ? null : parent;

			if (method.HasSignature(null))
				field.RegisterCallback<DictionaryField.ItemAddedEvent>(e => NoneCallback(method, owner));
			else if (method.HasSignature(null, typeof(string)))
				field.RegisterCallback<DictionaryField.ItemAddedEvent>(e => OneCallback(e.Key, method, owner));
			else
				Debug.LogWarningFormat(warning, propertyPath, method.Name);
		}

		private void SetupRemoveCallback(DictionaryField field, object parent, MethodInfo method, string warning, string propertyPath)
		{
			var owner = method.IsStatic ? null : parent;

			if (method.HasSignature(null))
				field.RegisterCallback<DictionaryField.ItemRemovedEvent>(e => NoneCallback(method, owner));
			else if (method.HasSignature(null, typeof(string)))
				field.RegisterCallback<DictionaryField.ItemRemovedEvent>(e => OneCallback(e.Key, method, owner));
			else
				Debug.LogWarningFormat(warning, propertyPath, method.Name);
		}

		private void SetupReorderCallback(DictionaryField field, object parent, MethodInfo method, string warning, string propertyPath)
		{
			var owner = method.IsStatic ? null : parent;

			if (method.HasSignature(null))
				field.RegisterCallback<DictionaryField.ItemReorderedEvent>(e => NoneCallback(method, owner));
			else if (method.HasSignature(null, typeof(int)))
				field.RegisterCallback<DictionaryField.ItemReorderedEvent>(e => OneIntCallback(e.ToIndex, method, owner));
			else if (method.HasSignature(null, typeof(int), typeof(int)))
				field.RegisterCallback<DictionaryField.ItemReorderedEvent>(e => TwoIntCallback(e.FromIndex, e.ToIndex, method, owner));
			else
				Debug.LogWarningFormat(warning, propertyPath, method.Name);
		}

		private void SetupChangeCallback(DictionaryField field, object parent, MethodInfo method, string warning, string propertyPath)
		{
			var owner = method.IsStatic ? null : parent;

			if (method.HasSignature(null))
				field.RegisterCallback<DictionaryField.ItemsChangedEvent>(e => NoneCallback(method, owner));
			else
				Debug.LogWarningFormat(warning, propertyPath, method.Name);
		}

		private void AddConditional(Func<string, bool> callback, object parent, MethodInfo method, string warning, string propertyPath)
		{
			var owner = method.IsStatic ? null : parent;

			if (method.HasSignature(typeof(bool)))
				callback += key => NoneConditional(method, owner);
			else if (method.HasSignature(typeof(bool), typeof(string)))
				callback += key => OneConditional(key, method, owner);
			else
				Debug.LogWarningFormat(warning, propertyPath, method.Name);
		}

		private void NoneCallback(MethodInfo method, object owner)
		{
			if (!EditorApplication.isPlaying)
				method.Invoke(owner, null);
		}

		private void OneCallback(string key, MethodInfo method, object owner)
		{
			if (!EditorApplication.isPlaying)
			{
				_oneParameter[0] = key;
				method.Invoke(owner, _oneParameter);
			}
		}

		private void OneIntCallback(int index, MethodInfo method, object owner)
		{
			if (!EditorApplication.isPlaying)
			{
				_oneParameter[0] = index;
				method.Invoke(owner, _oneParameter);
			}
		}

		private void TwoIntCallback(int from, int to, MethodInfo method, object owner)
		{
			if (!EditorApplication.isPlaying)
			{
				_twoParameters[0] = from;
				_twoParameters[1] = to;
				method.Invoke(owner, _twoParameters);
			}
		}

		private bool NoneConditional(MethodInfo method, object owner)
		{
			return (bool)method.Invoke(owner, null);
		}

		private bool OneConditional(string key, MethodInfo method, object owner)
		{
			_oneParameter[0] = key;
			return (bool)method.Invoke(owner, _oneParameter);
		}
	}
}
