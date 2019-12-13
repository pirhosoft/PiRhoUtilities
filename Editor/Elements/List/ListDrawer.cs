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
		private const string _missingAllowReorderMethodWarning = "(PULDMAROM) invalid method for AllowReorder on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _invalidAllowAddMethodWarning = "(PULDIAAM) invalid method for AllowAdd on field '{0}': the method '{1}' should take no parameters";
		private const string _invalidAllowRemoveMethodWarning = "(PULDIARM) invalid method for AllowRemove on field '{0}': the method '{1}' should take an 0 or 1 int parameters";
		private const string _invalidAllowReorderMethodWarning = "(PULDIAROM) invalid method for AllowReorder on field '{0}': the method '{1}' should take 0, 1, or 2 int parameters";

		private const string _missingAddMethodWarning = "(PULDMAM) invalid method for AddCallback on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _missingRemoveMethodWarning = "(PULDMRM) invalid method for RemoveCallback on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _missingReorderMethodWarning = "(PULDMROM) invalid method for ReorderCallback on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _missingChangeMethodWarning = "(PULDMCM) invalid method for ChangeCallback on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _invalidAddMethodWarning = "(PULDIAM) invalid method for AddCallback on field '{0}': the method '{1}' should take no parameters";
		private const string _invalidRemoveMethodWarning = "(PULDIRM) invalid method for RemoveCallback on field '{0}': the method '{1}' should take an 0 or 1 int parameters";
		private const string _invalidReorderMethodWarning = "(PULDIROM) invalid method for ReorderCallback on field '{0}': the method '{1}' should take 0, 1, or 2 int parameters";
		private const string _invalidChangeMethodWarning = "(PULDICM) invalid method for ChangeCallback on field '{0}': the method '{1}' should take 0, 1, or 2 int parameters";

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

				var field = new ListField();
				field.SetItemType(referenceType, true);
				field.Proxy = proxy;
				field.bindingPath = property.propertyPath;
				// TODO: other stuff from ConfigureField

				if (!string.IsNullOrEmpty(listAttribute.EmptyLabel))
					field.EmptyLabel = listAttribute.EmptyLabel;

				field.AllowReorder = listAttribute.AllowReorder;

				if (listAttribute.AllowAdd == ListAttribute.Always)
				{
					field.AllowAdd = true;
				}
				else if (listAttribute.AllowAdd == ListAttribute.Never)
				{
					field.AllowAdd = false;
				}
				else if (TryGetMethod(listAttribute.AllowAdd, _missingAllowAddMethodWarning, property.propertyPath, out var allowAddMethod))
				{
					var owner = allowAddMethod.IsStatic ? null : parent;
					var count = CheckSignature(allowAddMethod, typeof(bool), false, false, _invalidAllowAddMethodWarning, property.propertyPath);
					if (count == 0)
						proxy.CanAddCallback += () => NoneConditional(allowAddMethod, owner);
				}

				if (listAttribute.AllowRemove == ListAttribute.Always)
				{
					field.AllowRemove = true;
				}
				else if (listAttribute.AllowRemove == ListAttribute.Never)
				{
					field.AllowRemove = false;
				}
				else if (TryGetMethod(listAttribute.AllowRemove, _missingAllowRemoveMethodWarning, property.propertyPath, out var allowRemoveMethod))
				{
					var owner = allowRemoveMethod.IsStatic ? null : parent;
					var count = CheckSignature(allowRemoveMethod, typeof(bool), true, false, _invalidAllowRemoveMethodWarning, property.propertyPath);
					if (count == 0)
						proxy.CanRemoveCallback += index => NoneConditional(allowRemoveMethod, owner);
					else if (count == 1)
						proxy.CanRemoveCallback += index => OneConditional(index, allowRemoveMethod, owner);
				}

				if (TryGetMethod(listAttribute.AddCallback, _missingAddMethodWarning, property.propertyPath, out var addMethod))
				{
					var owner = addMethod.IsStatic ? null : parent;
					var count = CheckSignature(addMethod, null, false, false, _invalidAddMethodWarning, property.propertyPath);

					if (count == 0)
						field.RegisterCallback<ListField.ItemAddedEvent>(e => NoneCallback(addMethod, owner));
				}

				if (TryGetMethod(listAttribute.RemoveCallback, _missingRemoveMethodWarning, property.propertyPath, out var removeMethod))
				{
					var owner = removeMethod.IsStatic ? null : parent;
					var count = CheckSignature(removeMethod, null, true, false, _invalidRemoveMethodWarning, property.propertyPath);
					if (count == 0)
						field.RegisterCallback<ListField.ItemRemovedEvent>(e => NoneCallback(removeMethod, owner));
					else if (count == 1)
						field.RegisterCallback<ListField.ItemRemovedEvent>(e => OneCallback(e.Index, removeMethod, owner));
				}

				if (TryGetMethod(listAttribute.ReorderCallback, _missingReorderMethodWarning, property.propertyPath, out var reorderMethod))
				{
					var owner = reorderMethod.IsStatic ? null : parent;
					var count = CheckSignature(reorderMethod, null, true, true, _invalidReorderMethodWarning, property.propertyPath);
					if (count == 0)
						field.RegisterCallback<ListField.ItemReorderedEvent>(e => NoneCallback(reorderMethod, owner));
					else if (count == 1)
						field.RegisterCallback<ListField.ItemReorderedEvent>(e => OneCallback(e.ToIndex, reorderMethod, owner));
					else if (count == 2)
						field.RegisterCallback<ListField.ItemReorderedEvent>(e => TwoCallback(e.FromIndex, e.ToIndex, reorderMethod, owner));
				}

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

		private int CheckSignature(MethodInfo method, Type returnType, bool one, bool two, string warning, string propertyPath)
		{
			if (method.HasSignature(returnType))
				return 0;
			else if (one && method.HasSignature(returnType, typeof(int)))
				return 1;
			else if (two && method.HasSignature(returnType, typeof(int), typeof(int)))
				return 2;

			Debug.LogWarningFormat(warning, propertyPath, method.Name);
			return -1;
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
