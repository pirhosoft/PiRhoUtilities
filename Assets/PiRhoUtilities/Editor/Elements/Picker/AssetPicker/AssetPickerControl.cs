using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Editor
{
	public class AssetPickerControl : BasePickerControl<AssetReference>, IDragReceiver
	{
		private class AssetProvider : PickerProvider<AssetReference> { }

		private const string _invalidTypeWarning = "(PUAPCIT) Invalid type for AssetPickerControl: the type '{0}' must be derived from UnityEngine.Object";

		public new const string Stylesheet = "Picker/AssetPicker/AssetPickerStyle.uss";
		public new const string UssClassName = "pirho-asset-picker";

		public Type Type { get; private set; }

		public AssetPickerControl(AssetReference value, Type type, string tag) : this(value, type, tag, null, Addressables.MergeMode.None)
		{
		}

		public AssetPickerControl(AssetReference value, Type type, string[] tags, Addressables.MergeMode mode) : this(value, type, null, tags, mode)
		{
		}

		private AssetPickerControl(AssetReference value, Type type, string tag, string[] tags, Addressables.MergeMode mode)
		{
			if (type != null && !(typeof(Object).IsAssignableFrom(type)))
			{
				Debug.LogWarningFormat(_invalidTypeWarning, type);
				return;
			}

			Type = type;

			var provider = ScriptableObject.CreateInstance<AssetProvider>();
			var paths = new List<string> { "None" };
			var assets = new List<AssetReference> { null };

			var loader = tags == null ? Addressables.LoadResourceLocationsAsync(tag, type) : Addressables.LoadResourceLocationsAsync(tags, mode, type);
			loader.Completed += handle =>
			{
				if (handle.Status == AsyncOperationStatus.Succeeded)
				{
					foreach (var result in handle.Result)
					{
						paths.Add(result.PrimaryKey);
						assets.Add(new AssetReference(result.ProviderId));
					}

					provider.Setup(type?.Name ?? "Addressable Asset", paths, assets, GetIcon, OnSelected);
				}
			};

			Setup(provider, value);

			this.MakeDragReceiver();
			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
			AddToClassList(UssClassName);
		}

		private Texture GetIcon(AssetReference asset)
		{
			return asset != null && asset.editorAsset ? AssetPreview.GetMiniTypeThumbnail(asset.editorAsset.GetType()) : null;
		}

		private void OnSelected(AssetReference selected)
		{
			var previous = Value;

			SetValueWithoutNotify(selected);
			this.SendChangeEvent(previous, Value);
		}

		protected override void Refresh()
		{
			var text = Value == null ? Value.editorAsset.name : $"None ({Type?.Name ?? "Asset"})";
			var icon = GetIcon(Value);

			SetLabel(icon, text);
		}

		#region IDragReceiver Implementation

		public bool IsDragValid(Object[] objects, object data)
		{
			//if (objects.Length > 0)
			//{
			//	var obj = objects[0];
			//	if (obj != null)
			//	{
			//		var drag = obj.GetType();
			//		return Type.IsAssignableFrom(drag);
			//	}
			//}

			return false;
		}

		public void AcceptDrag(Object[] objects, object data)
		{
			//OnSelected(new AssetReference(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(objects[0]))));
		}

		#endregion
	}
}
