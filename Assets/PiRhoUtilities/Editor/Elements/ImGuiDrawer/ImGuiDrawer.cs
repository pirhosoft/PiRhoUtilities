using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class ImGuiDrawer : IMGUIContainer
	{
		private readonly SerializedProperty _property;
		private readonly PropertyDrawer _drawer;
		private readonly GUIContent _label;

		public string Label
		{
			get => _label.text;
			set => _label.text = value;
		}

		public ImGuiDrawer(SerializedProperty property, PropertyDrawer drawer)
		{
			_property = property;
			_drawer = drawer;
			_label = new GUIContent(_property.displayName);

			onGUIHandler = OnGuiHandler;
		}

		private void OnGuiHandler()
		{
			EditorGUI.BeginChangeCheck();
			_property.serializedObject.Update();

			var height = _drawer.GetPropertyHeight(_property, _label);
			var rect = EditorGUILayout.GetControlRect(true, height);

			style.height = height;

			_drawer.OnGUI(rect, _property, _label);

			_property.serializedObject.ApplyModifiedProperties();
			EditorGUI.EndChangeCheck();
		}
	}
}