using UnityEngine;

namespace PiRhoSoft.Utilities
{
	public interface IResource : ISerializationCallbackReceiver
	{
		string Path { get; }
	}

	public abstract class Resource : ScriptableObject, ISerializationCallbackReceiver
	{
		public const string _invalidPathWarning = "(URIP) Invalid Resource location: the {0} at path {1} should be beneath a folder called 'Resources' so it can be loaded at runtime";

#pragma warning disable CS0414

		private static readonly string ResourcesFolder = "Resources/";
		private static readonly int FolderLength = "Resources/".Length;
		private static readonly int ExtensionLength = ".asset".Length;

#pragma warning restore CS0414

		[SerializeField, HideInInspector]
		private string _path = "";

		public string Path => _path;

		public void OnBeforeSerialize()
		{
			_path = GetResourcePath(this);
		}

		public void OnAfterDeserialize()
		{
		}

		// We use a static helper method so that objects that want to be Resources but are already derived from another object can
		// simply call this method in their own serilazation methods.

		public static string GetResourcePath(Object obj)
		{
#if UNITY_EDITOR
			// During runtime ScriptableObjects are only referenceable outside of scenes (e.g from a game save file) by
			// passing their path to Resources Load (and then only if the object is in a Resources folder).

			var path = UnityEditor.AssetDatabase.GetAssetPath(obj);
			var index = path.IndexOf(ResourcesFolder);

			if (index < 0)
			{
				if (!string.IsNullOrEmpty(path))
					Debug.LogWarningFormat(obj, _invalidPathWarning, obj.GetType().Name, path);

				return path;
			}
			else
			{
				// Unity merges all Resources folders so the path needed to look up the Resource is just the portion
				// beneath the Resources folder.

				return path.Substring(index + FolderLength, path.Length - index - FolderLength - ExtensionLength);
			}
#else
			return string.Empty;
#endif
		}
	}
}
