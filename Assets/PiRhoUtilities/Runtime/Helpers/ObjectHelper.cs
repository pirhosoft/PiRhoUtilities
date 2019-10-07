using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities
{
	public class ObjectList
	{
		public List<Object> Objects;
		public List<string> Paths;
	}

	public static class ObjectHelper
	{
		public static GameObject FindObject(string name, int sceneIndex = -1)
		{
			// This is slower than the built in GameObject.Find however it will find both active and inactive objects.

			var objects = Resources.FindObjectsOfTypeAll<GameObject>();

			foreach (var o in objects)
			{
				if (o.name == name && (sceneIndex < 0 || o.scene.buildIndex == sceneIndex))
					return o;
			}

			return null;
		}

		public static T GetComponentInScene<T>(int sceneIndex, bool includeDisabled) where T : Component
		{
			var components = includeDisabled ? Resources.FindObjectsOfTypeAll<T>() : Object.FindObjectsOfType<T>();

			foreach (var component in components)
			{
				if (component.gameObject.scene.buildIndex == sceneIndex)
					return component;
			}

			return null;
		}

		public static Component GetComponentInScene(Type type, int sceneIndex, bool includeDisabled)
		{
			var objects = includeDisabled ? Resources.FindObjectsOfTypeAll(type) : Object.FindObjectsOfType(type);

			foreach (var obj in objects)
			{
				var component = obj.GetAsComponent(type);
				if (component && component.gameObject.scene.buildIndex == sceneIndex)
					return component;
			}

			return null;
		}

		public static List<T> GetComponentsInScene<T>(int sceneIndex, bool includeDisabled) where T : Component
		{
			var components = new List<T>();
			var objects = includeDisabled ? Resources.FindObjectsOfTypeAll<T>() : Object.FindObjectsOfType<T>();

			foreach (var obj in objects)
			{
				if (obj.gameObject.scene.buildIndex == sceneIndex)
					components.Add(obj);
			}

			return components;
		}

		public static List<Component> GetComponentsInScene(Type type, int sceneIndex, bool includeDisabled)
		{
			var components = new List<Component>();
			var objects = includeDisabled ? Resources.FindObjectsOfTypeAll(type) : Object.FindObjectsOfType(type);

			foreach (var obj in objects)
			{
				var component = obj.GetAsComponent(type);
				if (component && component.gameObject.scene.buildIndex == sceneIndex)
					components.Add(component);
			}

			return components;
		}

		public static ObjectList GetObjectList<T>(bool includeDisabled)
		{
			return GetObjectList(typeof(T), includeDisabled);
		}

		public static ObjectList GetObjectList(Type type, bool includeDisabled)
		{
			var objectList = new ObjectList();

			var objects = includeDisabled ? Resources.FindObjectsOfTypeAll(type) : Object.FindObjectsOfType(type);
			var paths = objects.Select(obj => GetPath(obj));
			var prefix = FindCommonPath(paths);

			objectList.Objects = objects.ToList();
			objectList.Paths = objects.Select(obj =>
			{
				var path = GetPath(obj).Substring(prefix.Length);
				return path.Length > 0 ? path + obj.name : obj.name;
			}).ToList();

			return objectList;
		}

		public static string GetPath(Object obj)
		{
			var path = string.Empty;
			var gameObject = obj.GetAsGameObject();

			if (gameObject)
			{
				while (gameObject.transform.parent)
				{
					gameObject = gameObject.transform.parent.gameObject;
					path += gameObject.name + "/";
				}
			}

			return path;
		}

		public static string FindCommonPath(IEnumerable<string> paths)
		{
			var prefix = paths.FirstOrDefault() ?? string.Empty;

			foreach (var path in paths)
			{
				var index = 0;
				var count = Math.Min(prefix.Length, path.Length);

				for (; index < count; index++)
				{
					if (prefix[index] != path[index])
						break;
				}

				prefix = prefix.Substring(0, index);

				var slash = prefix.LastIndexOf('/');
				if (slash != prefix.Length - 1)
					prefix = slash >= 0 ? prefix.Substring(0, slash + 1) : string.Empty;
			}

			return prefix;
		}
	}
}
