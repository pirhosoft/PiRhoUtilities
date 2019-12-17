using PiRhoSoft.Utilities.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Samples
{
	public class ChangeTriggerCodeSample : CodeSample
	{
		// ChangeTrigger is only relevant when using bindings so this class is used to provide a SerializedObject
		// to bind to for this example. For a real use case an existing asset would be used.

		public interface IDummyReference
		{
		}

		[Serializable]
		public class IntDummyReference : IDummyReference
		{
			public int Int;
		}

		[Serializable]
		public class BoolDummyReference : IDummyReference
		{
			public bool Bool;
		}

		public class DummyAsset : ScriptableObject
		{
			public int Int;
			public bool Bool;
			public float Float;
			public string String;
			public Color Color;
			public Texture2D Object;
			public TextureImporterType Enum;
			public Vector2 Vector2;
			public Vector3 Vector3;
			public Vector4 Vector4;
			public Rect Rect;
			public List<int> ArraySize;
			public char Character = 'a';
			public AnimationCurve AnimationCurve;
			public Bounds Bounds;
			public Gradient Gradient;
			public Quaternion Quaternion;
			public Vector2Int Vector2Int;
			public Vector3Int Vector3Int;
			public RectInt RectInt;
			public BoundsInt BoundsInt;
			[Reference] [SerializeReference] public IDummyReference Reference; // Changing this will log undo related errors which will presumably go away when Unity adds proper ManagedReference support
		}

		public override void Create(VisualElement root)
		{
			var asset = ScriptableObject.CreateInstance<DummyAsset>();
			var obj = new SerializedObject(asset);

			CreateTrigger<int>(root, obj, nameof(DummyAsset.Int));
			CreateTrigger<bool>(root, obj, nameof(DummyAsset.Bool));
			CreateTrigger<float>(root, obj, nameof(DummyAsset.Float));
			CreateTrigger<string>(root, obj, nameof(DummyAsset.String));
			CreateTrigger<Color>(root, obj, nameof(DummyAsset.Color));
			CreateTrigger<Object>(root, obj, nameof(DummyAsset.Object));
			CreateTrigger<Enum>(root, obj, nameof(DummyAsset.Enum));
			CreateTrigger<Vector2>(root, obj, nameof(DummyAsset.Vector2));
			CreateTrigger<Vector3>(root, obj, nameof(DummyAsset.Vector3));
			CreateTrigger<Vector4>(root, obj, nameof(DummyAsset.Vector4));
			CreateTrigger<Rect>(root, obj, nameof(DummyAsset.Rect));
			CreateArraySizeTrigger(root, obj, nameof(DummyAsset.ArraySize));
			CreateTrigger<char>(root, obj, nameof(DummyAsset.Character));
			CreateTrigger<AnimationCurve>(root, obj, nameof(DummyAsset.AnimationCurve));
			CreateTrigger<Bounds>(root, obj, nameof(DummyAsset.Bounds));
			CreateTrigger<Gradient>(root, obj, nameof(DummyAsset.Gradient));
			CreateTrigger<Quaternion>(root, obj, nameof(DummyAsset.Quaternion));
			CreateTrigger<Vector2Int>(root, obj, nameof(DummyAsset.Vector2Int));
			CreateTrigger<Vector3Int>(root, obj, nameof(DummyAsset.Vector3Int));
			CreateTrigger<RectInt>(root, obj, nameof(DummyAsset.RectInt));
			CreateTrigger<BoundsInt>(root, obj, nameof(DummyAsset.BoundsInt));
			CreateTrigger<IDummyReference>(root, obj, nameof(DummyAsset.Reference));

			root.Bind(obj);
		}

		private void CreateTrigger<T>(VisualElement root, SerializedObject obj, string propertyName)
		{
			var property = obj.FindProperty(propertyName);
			var field = new PropertyField(property);
			var trigger = new ChangeTrigger<T>(property, PropertyChanged);

			root.Add(field);
			root.Add(trigger);
		}

		private void CreateArraySizeTrigger(VisualElement root, SerializedObject obj, string propertyName)
		{
			var list = obj.FindProperty(propertyName);
			var field = new PropertyField(list);
			var property = list.FindPropertyRelative("Array.size");
			var trigger = new ChangeTrigger<int>(property, PropertyChanged);

			root.Add(field);
			root.Add(trigger);
		}

		private void PropertyChanged<T>(SerializedProperty property, T oldValue, T newValue)
		{
			Debug.Log($"{property.propertyPath} changed from {oldValue} to {newValue}");
		}
	}
}
