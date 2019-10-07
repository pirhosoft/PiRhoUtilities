using System;
using System.Reflection;

namespace PiRhoSoft.Utilities
{
	public static class InternalHelper
	{
		// TODO: these are all static only right now - probably need the instance type in the signature for non statics
		// TODO: need to be able to pass in parameter types to disambiguate overloads
		// TODO: error reporting

		public static DelegateType CreateDelegate<DelegateType>(MethodInfo method) where DelegateType : Delegate
		{
			return (DelegateType)Delegate.CreateDelegate(typeof(DelegateType), method, false);
		}

		public static DelegateType CreateDelegate<DelegateType>(Type type, string methodName) where DelegateType : Delegate
		{
			var method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			return method != null ? (DelegateType)Delegate.CreateDelegate(typeof(DelegateType), method, false) : null;
		}

		public static Func<PropertyType> CreateGetDelegate<PropertyType>(Type type, string propertyName)
		{
			var property = type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			var method = property != null ? property.GetGetMethod(true) : null; // nonPublic parameter means also get non public, rather than only get non public

			return method != null ? (Func<PropertyType>)Delegate.CreateDelegate(typeof(Func<PropertyType>), method, false) : null;
		}

		public static Action<PropertyType> CreateSetDelegate<PropertyType>(Type type, string propertyName)
		{
			var property = type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			var method = property != null ? property.GetSetMethod(true) : null; // same as GetGetMethod in CreateGetDelegate

			return method != null ? (Action<PropertyType>)Delegate.CreateDelegate(typeof(Action<PropertyType>), method, false) : null;
		}

		public static Func<FieldType> CreateGetField<FieldType>(Type type, string fieldName)
		{
			var field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

			if (field != null)
				return () => (FieldType)field.GetValue(null);

			return null;
		}

		public static Action<FieldType> CreateSetField<FieldType>(Type type, string fieldName)
		{
			var field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

			if (field != null)
				return value => field.SetValue(null, value);

			return null;
		}

		public static Func<FieldType> CreateGetField<FieldType>(Type type, string fieldName, object obj)
		{
			var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			if (field != null)
				return () => (FieldType)field.GetValue(obj);

			return null;
		}

		public static Action<FieldType> CreateSetField<FieldType>(Type type, string fieldName, object obj)
		{
			var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			if (field != null)
				return value => field.SetValue(obj, value);

			return null;
		}
	}
}
