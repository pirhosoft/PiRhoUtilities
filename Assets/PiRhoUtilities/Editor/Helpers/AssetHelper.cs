using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Editor
{
	public class AssetList
	{
		public AssetList(Type type)
		{
			Type = type;
		}

		public Type Type { get; private set; }

		public List<Object> Assets;
		public List<string> Paths;
	}

	public class AssetHelper : AssetPostprocessor
	{
		private static Dictionary<string, AssetList> _assetLists = new Dictionary<string, AssetList>();
		private const string _invalidPathError = "(UAHIP) failed to create asset at path {0}: the path must be inside the 'Assets' folder for this project";
		private const string _missingEditorPathError = "(UAHMEP) failed to determine editor path for type {0} and path {1}";

		static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			_assetLists.Clear(); // no reason to figure out what actually changed - just clear all the lists so they are rebuilt on next access
		}

		#region Creation

		public static ObjectType Create<ObjectType>() where ObjectType : ScriptableObject
		{
			return (ObjectType)Create(typeof(ObjectType));
		}
		
		public static Object Create(Type createType)
		{
			var title = $"Create a new {createType.Name}";
			var path = EditorUtility.SaveFilePanel(title, "Assets", $"{createType.Name}.asset", "asset");
		
			if (!string.IsNullOrEmpty(path))
			{
				var asset = CreateAssetAtPath(path, createType);
		
				if (asset == null)
					Debug.LogErrorFormat(_invalidPathError, path);
		
				return asset;
			}
		
			return null;
		}
		
		public static AssetType CreateAsset<AssetType>(string name) where AssetType : ScriptableObject
		{
			return CreateAsset(name, typeof(AssetType)) as AssetType;
		}
		
		public static AssetType GetOrCreateAsset<AssetType>(string name) where AssetType : ScriptableObject
		{
			var asset = GetAsset<AssetType>();
		
			if (asset == null)
				asset = CreateAsset<AssetType>(name);
		
			return asset;
		}
		
		public static ScriptableObject CreateAsset(string name, Type type)
		{
			var asset = ScriptableObject.CreateInstance(type);
			var path = AssetDatabase.GenerateUniqueAssetPath("Assets/" + name + ".asset");
		
			AssetDatabase.CreateAsset(asset, path);
			AssetDatabase.SaveAssets();
		
			return asset;
		}
		
		public static ScriptableObject GetOrCreateAsset(string name, Type type)
		{
			var asset = GetAsset(type) as ScriptableObject;
		
			if (asset == null)
				asset = CreateAsset(name, type);
		
			return asset;
		}
		
		public static ScriptableObject CreateAssetAtPath(string path, Type type)
		{
			if (!path.StartsWith(Application.dataPath))
				return null;
		
			path = path.Substring(Application.dataPath.Length - 6); // keep 'Assets' as the root folder
		
			var asset = ScriptableObject.CreateInstance(type);
		
			AssetDatabase.CreateAsset(asset, path);
			AssetDatabase.SaveAssets();
		
			return asset;
		}

		#endregion

		#region Lookup

		public static AssetType GetAsset<AssetType>() where AssetType : Object
		{
			return GetAssets<AssetType>().FirstOrDefault();
		}

		public static AssetType GetAssetWithId<AssetType>(string id) where AssetType : Object
		{
			var path = AssetDatabase.GUIDToAssetPath(id);
			return GetAssetAtPath<AssetType>(path);
		}

		public static AssetType GetAssetAtPath<AssetType>(string path) where AssetType : Object
		{
			return AssetDatabase.LoadAssetAtPath<AssetType>(path);
		}

		public static Object GetAsset(Type assetType)
		{
			return GetAssets(assetType).FirstOrDefault();
		}

		public static Object GetAssetWithId(string id, Type type)
		{
			var path = AssetDatabase.GUIDToAssetPath(id);
			return GetAssetAtPath(path, type);
		}

		public static Object GetAssetAtPath(string path, Type type)
		{
			return AssetDatabase.LoadAssetAtPath(path, type) as Object;
		}

		#endregion

		#region Listing

		public static IEnumerable<AssetType> GetAssets<AssetType>() where AssetType : Object
		{
			return GetAssets(typeof(AssetType)).Select(asset => asset as AssetType);
		}

		public static IEnumerable<Object> GetAssets(Type assetType)
		{
			return AssetDatabase.FindAssets($"t:{assetType.Name}").Select(id => GetAssetWithId(id, assetType)).Where(asset => asset);
		}

		public static AssetList GetAssetList<AssetType>() where AssetType : Object
		{
			return GetAssetList(typeof(AssetType));
		}

		public static AssetList GetAssetList(Type assetType)
		{
			var listName = assetType.AssemblyQualifiedName;

			if (!_assetLists.TryGetValue(listName, out var list))
			{
				list = new AssetList(assetType);

				var assets = GetAssets(assetType);
				var paths = assets.Select(asset => GetPath(asset));
				var prefix = FindCommonPath(paths);

				list.Assets = assets.ToList();
				list.Paths = assets.Select(asset =>
				{
					var path = GetPath(asset).Substring(prefix.Length);
					return path.Length > 0 ? path + asset.name : asset.name;
				}).ToList();

				_assetLists.Add(listName, list);
			}

			return list;
		}

		#endregion

		#region Helpers

		public static string GetPath(Object asset)
		{
			var path = AssetDatabase.GetAssetPath(asset);
			var slash = path.LastIndexOf('/');

			return path.Substring(0, slash + 1);
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

		public static string FindEditorPath(string typename, string editorFolder)
		{
			// Packages might be added as a subfolder of a different project so this determines the
			// actual path to the editor scripts by finding the asset representing the desired script file

			var ids = AssetDatabase.FindAssets(typename);

			foreach (var id in ids)
			{
				var path = AssetDatabase.GUIDToAssetPath(id);
				var index = path.IndexOf(editorFolder);

				if (index >= 0)
					return path.Substring(0, index) + editorFolder;
			}

			Debug.LogErrorFormat(_missingEditorPathError, typename, editorFolder);
			return "Assets/" + editorFolder;
		}

		#endregion
	}
}
