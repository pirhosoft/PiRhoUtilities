namespace PiRhoSoft.Utilities
{
	public static class ApplicationHelper
	{
		// This is different from the built in Application.isPlaying. That property will be false when the editor is
		// transitioning to play mode but not yet playing. The distinction is relevant in OnEnable for ScriptableObject
		// derived classes.
		public static bool IsPlaying
		{
#if UNITY_EDITOR
			get => UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode;
#else
			get => true;
#endif
		}
	}
}
