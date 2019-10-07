using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities
{
	public static class ObjectExtensions
	{
		public static bool HasType(this Object unityObject, Type type)
		{
			if (type == typeof(GameObject))
				return unityObject.GetAsGameObject() != null;
			else if (typeof(Component).IsAssignableFrom(type))
				return unityObject.GetAsGameObject()?.GetComponent(type) != null;
			else
				return type.IsAssignableFrom(unityObject.GetType());
		}

		public static Object GetAsBaseObject(this Object unityObject)
		{
			// The 'base' object is the GameObject for Component types and the actual object for all other types.
			return unityObject is Component component ? component.gameObject : unityObject;
		}

		public static T GetAsObject<T>(this Object unityObject) where T : Object
		{
			if (unityObject is T t)
				return t;

			if (typeof(T) == typeof(GameObject))
				return unityObject.GetAsGameObject() as T;

			if (typeof(Component).IsAssignableFrom(typeof(T)))
				return unityObject.GetAsComponent<T>();

			return null;
		}

		public static Object GetAsObject(this Object unityObject, Type type)
		{
			if (type.IsAssignableFrom(unityObject.GetType()))
				return unityObject;

			if (type == typeof(GameObject))
				return unityObject.GetAsGameObject();

			if (typeof(Component).IsAssignableFrom(type))
				return unityObject.GetAsComponent(type);

			return null;
		}

		public static GameObject GetAsGameObject(this Object unityObject)
		{
			if (unityObject is GameObject gameObject)
				return gameObject;

			if (unityObject is Component component)
				return component.gameObject;

			return null;
		}

		public static T GetAsComponent<T>(this Object unityObject) where T : Object
		{
			if (unityObject is T t)
				return t;

			if (unityObject is GameObject gameObject)
				return gameObject.GetComponent<T>();

			if (unityObject is Component component)
				return component.GetComponent<T>();

			return null;
		}

		public static Component GetAsComponent(this Object unityObject, Type componentType)
		{
			if (componentType.IsAssignableFrom(unityObject.GetType()))
				return unityObject as Component;

			if (unityObject is GameObject gameObject)
				return gameObject.GetComponent(componentType);

			if (unityObject is Component component)
				return component.GetComponent(componentType);

			return null;
		}

		public static Component GetAsComponent(this Object unityObject, string componentName)
		{
			if (unityObject is GameObject gameObject)
				return gameObject.GetComponent(componentName);

			if (unityObject is Component component)
				return component.GetComponent(componentName);

			return null;
		}
	}
}
