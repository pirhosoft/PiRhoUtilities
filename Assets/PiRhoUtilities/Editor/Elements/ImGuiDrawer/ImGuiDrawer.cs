using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class ImGuiDrawer : IMGUIContainer
	{
		private SerializedProperty _property;
		private PropertyDrawer _drawer;

		public ImGuiDrawer(SerializedProperty property, PropertyDrawer drawer)
		{
			_property = property;
			_drawer = drawer;

			onGUIHandler = OnGuiHandler;
		}

		private void OnGuiHandler()
		{
			EditorGUI.BeginChangeCheck();
			_property.serializedObject.Update();

			var content = new GUIContent(_property.displayName);
			var height = _drawer.GetPropertyHeight(_property, content);
			var rect = EditorGUILayout.GetControlRect(true, height);

			_drawer.OnGUI(rect, _property, content);

			_property.serializedObject.ApplyModifiedProperties();
			EditorGUI.EndChangeCheck();
		}
	}
}