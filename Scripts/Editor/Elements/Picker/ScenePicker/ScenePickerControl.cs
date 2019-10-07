using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Editor
{
	public class ScenePickerControl : BasePickerControl<AssetReference>, IDragReceiver
	{
		private class SceneProvider : PickerProvider<SceneAsset> { }

		public new const string Stylesheet = "Picker/ScenePicker/ScenePickerStyle.uss";
		public new const string UssClassName = "pirho-scene-picker";
		public const string LoadUssClassName = UssClassName + "__load";
		public const string CreateUssClassName = UssClassName + "__create";

		private IconButton _load;
		private IconButton _create;

		private MessageBox _buildWarning;

		public ScenePickerControl(AssetReference value, Action onCreate)
		{
			_load = new IconButton(Icon.Load.Texture, "Load this scene", Load) { tintColor = Color.black };
			_load.AddToClassList(LoadUssClassName);

			_create = new IconButton(Icon.Add.Texture, "Create a new scene", () => Create(onCreate));
			_create.AddToClassList(CreateUssClassName);

			_buildWarning = new MessageBox(MessageBoxType.Info, "This scene is not in the build settings. Add it now?");
			_buildWarning.Add(new Button(AddToBuild) { text = "Add" });
	
			var assets = AssetHelper.GetAssetList(typeof(SceneAsset));
			var provider = ScriptableObject.CreateInstance<SceneProvider>();
			provider.Setup(assets.Type.Name, assets.Paths.Prepend("None").ToList(), assets.Assets.Prepend(null).Cast<SceneAsset>().ToList(), GetIcon, OnSelected);

			Setup(provider, value);

			Add(_load);
			Add(_create);
			Add(_buildWarning);

			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
			AddToClassList(UssClassName);
		}

		private Texture GetIcon(SceneAsset scene)
		{
			return scene ? AssetPreview.GetMiniTypeThumbnail(typeof(SceneAsset)) : null;
		}

		private void OnSelected(SceneAsset selected)
		{
			var previous = Value;
			var path = AssetDatabase.GetAssetPath(selected);
			var guid = AssetDatabase.AssetPathToGUID(path);

			SetValueWithoutNotify(new AssetReference(guid));
			this.SendChangeEvent(previous, Value);
		}

		private void Load()
		{
			var path = GetPath();
			var scene = SceneManager.GetSceneByPath(path);

			if (scene.isLoaded)
				EditorSceneManager.CloseScene(scene, true);
			else
				EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);

			Refresh();
		}

		private void Create(Action onCreate)
		{
			var scene = SceneHelper.CreateScene(onCreate);
			if (scene.IsValid())
				OnSelected(SceneHelper.GetSceneFromPath(scene.path));
		}

		private void AddToBuild()
		{
			var scene = new EditorBuildSettingsScene(GetPath(), true);
			EditorBuildSettings.scenes = EditorBuildSettings.scenes.Append(scene).ToArray();

			Refresh();
		}

		protected override void Refresh()
		{
			var path = GetPath();
			var asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
			var text = asset ? asset.name : "None (Scene)";
			var icon = GetIcon(asset);

			SetLabel(icon, text);

			if (asset)
			{
				var scene = SceneManager.GetSceneByPath(path);
				var buildIndex = SceneUtility.GetBuildIndexByScenePath(path);

				_load.SetEnabled(!scene.isLoaded || SceneManager.sceneCount > 1);
				_load.image = scene.isLoaded ? Icon.Unload.Texture : Icon.Load.Texture;

				_buildWarning.SetDisplayed(buildIndex < 0);
			}
			else
			{
				_load.image = Icon.Load.Texture;
				_load.SetEnabled(false);

				_buildWarning.SetDisplayed(false);
			}
		}

		private string GetPath() => AssetDatabase.GUIDToAssetPath(Value.AssetGUID);

		#region IDragReceiver Implementation

		public bool IsDragValid(Object[] objects, object data)
		{
			return objects.Length > 0 && objects[0] is SceneAsset;
		}

		public void AcceptDrag(Object[] objects, object data)
		{
			OnSelected(objects[0] as SceneAsset);
		}

		#endregion
	}
}
