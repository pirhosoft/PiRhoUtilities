using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PiRhoSoft.Utilities.Editor
{
	public class SceneList
	{
		public List<string> Names;
		public List<string> Paths;
	}

	[Serializable]
	public class SceneState
	{
		[SerializeField] public SceneData[] Scenes;

		[Serializable]
		public struct SceneData
		{
			public bool IsActive;
			public bool IsLoaded;
			public string Path;
		}
	}

	public static class SceneHelper
	{
		public static SceneAsset GetSceneFromPath(string path)
		{
			return AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
		}

		public static SceneAsset GetSceneFromBuildIndex(int index)
		{
			var path = SceneUtility.GetScenePathByBuildIndex(index);
			return GetSceneFromPath(path);
		}

		#region Creation

		public static Scene CreateScene(Action create)
		{
			var title = string.Format("Create a new Scene");
			var path = EditorUtility.SaveFilePanel(title, "Assets", "NewScene.unity", "unity");

			if (path.StartsWith(Application.dataPath))
				return CreateScene(path.Substring(Application.dataPath.Length - 6), create);
		
			return new Scene();
		}

		private static Scene CreateScene(string path, Action create)
		{
			var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
			SceneManager.SetActiveScene(scene);

			create();

			EditorSceneManager.SaveScene(scene, path);
			AddSceneToBuild(scene);

			return scene;
		}

		public static void AddSceneToBuild(Scene scene)
		{
			var original = EditorBuildSettings.scenes;
			var newSettings = new EditorBuildSettingsScene[original.Length + 1];
			var sceneToAdd = new EditorBuildSettingsScene(scene.path, true);

			Array.Copy(original, newSettings, original.Length);

			newSettings[newSettings.Length - 1] = sceneToAdd;

			EditorBuildSettings.scenes = newSettings;
		}

		#endregion

		#region Play State

		// SceneSetup is not serializable so this little dance is necessary to persist the editor state through play
		// mode changes

		public static SceneState CaptureState()
		{
			var state = new SceneState();
			var setup = EditorSceneManager.GetSceneManagerSetup();

			state.Scenes = new SceneState.SceneData[setup.Length];

			for (var i = 0; i < setup.Length; i++)
				state.Scenes[i] = new SceneState.SceneData { IsActive = setup[i].isActive, IsLoaded = setup[i].isLoaded, Path = setup[i].path };

			return state;
		}

		public static void RestoreState(SceneState state)
		{
			var scenes = new SceneSetup[state.Scenes.Length];

			for (var i = 0; i < state.Scenes.Length; i++)
				scenes[i] = new SceneSetup { isActive = state.Scenes[i].IsActive, isLoaded = state.Scenes[i].IsLoaded, path = state.Scenes[i].Path };

			EditorSceneManager.RestoreSceneManagerSetup(scenes);
		}

		#endregion
	}
}
