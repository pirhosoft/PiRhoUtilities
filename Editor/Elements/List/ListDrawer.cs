using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(ListAttribute))]
	class ListDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PULDIT) invalid type for ListAttribute on field '{0}': List can only be applied to SerializedList or SerializedArray fields";

		private const string _missingAllowAddMethodWarning = "(PULDMAAM) invalid method for AllowAdd on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _missingAllowRemoveMethodWarning = "(PULDMARM) invalid method for AllowRemove on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _invalidAllowAddMethodWarning = "(PULDIAAM) invalid method for AllowAdd on field '{0}': the method '{1}' should take no parameters";
		private const string _invalidAllowRemoveMethodWarning = "(PULDIARM) invalid method for AllowRemove on field '{0}': the method '{1}' should take an 0 or 1 int parameters";

		private const string _missingAddMethodWarning = "(PULDMAM) invalid method for AddCallback on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _missingRemoveMethodWarning = "(PULDMRM) invalid method for RemoveCallback on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _missingReorderMethodWarning = "(PULDMROM) invalid method for ReorderCallback on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _missingChangeMethodWarning = "(PULDMCM) invalid method for ChangeCallback on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _invalidAddMethodWarning = "(PULDIAM) invalid method for AddCallback on field '{0}': the method '{1}' should take no parameters";
		private const string _invalidRemoveMethodWarning = "(PULDIRM) invalid method for RemoveCallback on field '{0}': the method '{1}' should take an 0 or 1 int parameters";
		private const string _invalidReorderMethodWarning = "(PULDIROM) invalid method for ReorderCallback on field '{0}': the method '{1}' should take 0, 1, or 2 int parameters";
		private const string _invalidChangeMethodWarning = "(PULDICM) invalid method for ChangeCallback on field '{0}': the method '{1}' should take no parameters";

		private static readonly object[] _oneParameter = new object[1];
		private static readonly object[] _twoParameters = new object[2];

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var items = property.FindPropertyRelative(SerializedList<string>.ItemsProperty);

			if (items != null && items.isArray)
			{
				var isReference = fieldInfo.FieldType.BaseType.GetGenericTypeDefinition() == typeof(ReferenceList<>);
				var referenceType = isReference ? fieldInfo.GetFieldType() : null;
				var listAttribute = attribute as ListAttribute;
				var parent = property.GetOwner<object>();
				var drawer = this.GetNextDrawer();
				var proxy = new PropertyListProxy(items, drawer);
				var path = property.propertyPath;

				var field = new ListField();
				field.SetItemType(referenceType, true);
				field.Proxy = proxy;
				field.bindingPath = property.propertyPath;
				// TODO: other stuff from ConfigureField

				if (!string.IsNullOrEmpty(listAttribute.EmptyLabel))
					field.EmptyLabel = listAttribute.EmptyLabel;

				field.AllowAdd = listAttribute.AllowAdd != ListAttribute.Never;
				field.AllowRemove = listAttribute.AllowRemove != ListAttribute.Never;
				field.AllowReorder = listAttribute.AllowReorder;

				if (TryGetMethod(listAttribute.AllowAdd, _missingAllowAddMethodWarning, path, out var allowAddMethod))
					AddConditional(proxy.CanAddCallback, parent, allowAddMethod, _invalidAllowAddMethodWarning, path);

				if (TryGetMethod(listAttribute.AllowRemove, _missingAllowRemoveMethodWarning, path, out var allowRemoveMethod))
					AddConditional(proxy.CanRemoveCallback, parent, allowRemoveMethod, _invalidAllowRemoveMethodWarning, path);

				if (TryGetMethod(listAttribute.AddCallback, _missingAddMethodWarning, path, out var addMethod))
					SetupAddCallback(field, parent, addMethod, _invalidAddMethodWarning, path);

				if (TryGetMethod(listAttribute.RemoveCallback, _missingRemoveMethodWarning, path, out var removeMethod))
					SetupRemoveCallback(field, parent, removeMethod, _invalidRemoveMethodWarning, path);

				if (TryGetMethod(listAttribute.ReorderCallback, _missingReorderMethodWarning, path, out var reorderMethod))
					SetupReorderCallback(field, parent, reorderMethod, _invalidReorderMethodWarning, path);

				if (TryGetMethod(listAttribute.ChangeCallback, _missingChangeMethodWarning, path, out var changeMethod))
					SetupChangeCallback(field, parent, changeMethod, _invalidChangeMethodWarning, path);

				return field;
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
				return new FieldContainer(property.displayName, string.Empty);
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

		private void SetupAddCallback(ListField field, object parent, MethodInfo method, string warning, string propertyPath)
		{
			var owner = method.IsStatic ? null : parent;

			if (method.HasSignature(null))
				field.RegisterCallback<ListField.ItemAddedEvent>(e => NoneCallback(method, owner));
			else if (method.HasSignature(null, typeof(int)))
				field.RegisterCallback<ListField.ItemAddedEvent>(e => OneCallback(e.Index, method, owner));
			else
				Debug.LogWarningFormat(warning, propertyPath, method.Name);
		}

		private void SetupRemoveCallback(ListField field, object parent, MethodInfo method, string warning, string propertyPath)
		{
			var owner = method.IsStatic ? null : parent;

			if (method.HasSignature(null))
				field.RegisterCallback<ListField.ItemRemovedEvent>(e => NoneCallback(method, owner));
			else if (method.HasSignature(null, typeof(int)))
				field.RegisterCallback<ListField.ItemRemovedEvent>(e => OneCallback(e.Index, method, owner));
			else
				Debug.LogWarningFormat(warning, propertyPath, method.Name);
		}

		private void SetupReorderCallback(ListField field, object parent, MethodInfo method, string warning, string propertyPath)
		{
			var owner = method.IsStatic ? null : parent;

			if (method.HasSignature(null))
				field.RegisterCallback<ListField.ItemReorderedEvent>(e => NoneCallback(method, owner));
			else if (method.HasSignature(null, typeof(int)))
				field.RegisterCallback<ListField.ItemReorderedEvent>(e => OneCallback(e.ToIndex, method, owner));
			else if (method.HasSignature(null, typeof(int), typeof(int)))
				field.RegisterCallback<ListField.ItemReorderedEvent>(e => TwoCallback(e.FromIndex, e.ToIndex, method, owner));
			else
				Debug.LogWarningFormat(warning, propertyPath, method.Name);
		}

		private void SetupChangeCallback(ListField field, object parent, MethodInfo method, string warning, string propertyPath)
		{
			var owner = method.IsStatic ? null : parent;

			if (method.HasSignature(null))
				field.RegisterCallback<ListField.ItemsChangedEvent>(e => NoneCallback(method, owner));
			else
				Debug.LogWarningFormat(warning, propertyPath, method.Name);
		}

		private void AddConditional(Func<bool> callback, object parent, MethodInfo method, string warning, string propertyPath)
		{
			var owner = method.IsStatic ? null : parent;

			if (method.HasSignature(typeof(bool)))
				callback += () => NoneConditional(method, owner);
			else
				Debug.LogWarningFormat(warning, propertyPath, method.Name);
		}

		private void AddConditional(Func<int, bool> callback, object parent, MethodInfo method, string warning, string propertyPath)
		{
			var owner = method.IsStatic ? null : parent;

			if (method.HasSignature(typeof(bool)))
				callback += index => NoneConditional(method, owner);
			else if (method.HasSignature(typeof(bool), typeof(string)))
				callback += index => OneConditional(index, method, owner);
			else
				Debug.LogWarningFormat(warning, propertyPath, method.Name);
		}

		private void NoneCallback(MethodInfo method, object owner)
		{
			if (!EditorApplication.isPlaying)
				method.Invoke(owner, null);
		}

		private void OneCallback(int index, MethodInfo method, object owner)
		{
			if (!EditorApplication.isPlaying)
			{
				_oneParameter[0] = index;
				method.Invoke(owner, _oneParameter);
			}
		}

		private void TwoCallback(int from, int to, MethodInfo method, object owner)
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

		private bool OneConditional(int index, MethodInfo method, object owner)
		{
			_oneParameter[0] = index;
			return (bool)method.Invoke(owner, _oneParameter);
		}
	}
}
