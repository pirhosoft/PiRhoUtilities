using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PiRhoSoft.Utilities.Editor
{
	public static class FieldInfoExtensions
	{
		#region Attributes

		public static bool HasAttribute<AttributeType>(this FieldInfo field) where AttributeType : Attribute
		{
			return field.GetCustomAttribute<AttributeType>() != null;
		}

		public static AttributeType GetAttribute<AttributeType>(this FieldInfo field) where AttributeType : Attribute
		{
			return field.TryGetAttribute<AttributeType>(out var attribute) ? attribute : null;
		}

		public static bool TryGetAttribute<AttributeType>(this FieldInfo field, out AttributeType attribute) where AttributeType : Attribute
		{
			var attributes = field.GetCustomAttributes(typeof(AttributeType), false);
			attribute = attributes != null && attributes.Length > 0 ? attributes[0] as AttributeType : null;

			return attribute != null;
		}

		public static bool HasAttribute(this FieldInfo field, Type attribute)
		{
			return field.GetCustomAttribute(attribute) != null;
		}

		public static Attribute GetAttribute(FieldInfo field, Type attributeType)
		{
			return field.TryGetAttribute(attributeType, out var attribute) ? attribute : null;
		}

		public static bool TryGetAttribute(this FieldInfo field, Type attributeType, out Attribute attribute)
		{
			var attributes = field.GetCustomAttributes(attributeType, false);
			attribute = attributes != null && attributes.Length > 0 ? attributes[0] as Attribute : null;

			return attribute != null;
		}

		#endregion

		public static bool IsSerializable(this FieldInfo field)
		{
			var included = field.IsPublic || GetAttribute<SerializeField>(field) != null;
			var excluded = GetAttribute<NonSerializedAttribute>(field) != null;
			var compatible = !field.IsStatic && !field.IsLiteral && !field.IsInitOnly && field.FieldType.IsSerializable();

			return included && !excluded && compatible;
		}

		public static Type GetFieldType(this FieldInfo fieldInfo)
		{
			var interfaces = fieldInfo.FieldType.GetInterfaces();

			var ilist = interfaces.FirstOrDefault(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>));

			if (ilist != null)
				return ilist.GetGenericArguments()[0];

			var iDict = interfaces.FirstOrDefault(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>));

			if (iDict != null)
				return iDict.GetGenericArguments()[1];

			return fieldInfo.FieldType;
		}
	}
}