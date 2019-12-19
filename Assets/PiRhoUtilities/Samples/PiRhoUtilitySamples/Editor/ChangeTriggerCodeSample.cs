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

		public interface ISampleReference
		{
		}

		[Serializable]
		public class IntSampleReference : ISampleReference
		{
			public int Int;
		}

		[Serializable]
		public class BoolSampleReference : ISampleReference
		{
			public bool Bool;
		}

		public class SampleAsset : ScriptableObject
		{
			public int Int;
			public bool Bool;
			public float Float;
			public string String;
			public Color Color;
			public Texture2D Object;
			public LayerMask Layer;
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
			[Reference] [SerializeReference] public ISampleReference Reference; // Changing this will log undo related errors which will presumably go away when Unity adds proper ManagedReference support
		}

		public override void Create(VisualElement root)
		{
			var asset = ScriptableObject.CreateInstance<SampleAsset>();
			var obj = new SerializedObject(asset);

			CreateTrigger<int>(root, obj, nameof(SampleAsset.Int));
			CreateTrigger<bool>(root, obj, nameof(SampleAsset.Bool));
			CreateTrigger<float>(root, obj, nameof(SampleAsset.Float));
			CreateTrigger<string>(root, obj, nameof(SampleAsset.String));
			CreateTrigger<Color>(root, obj, nameof(SampleAsset.Color));
			CreateTrigger<Object>(root, obj, nameof(SampleAsset.Object));
			CreateTrigger<int>(root, obj, nameof(SampleAsset.Layer));
			CreateTrigger<Enum>(root, obj, nameof(SampleAsset.Enum));
			CreateTrigger<Vector2>(root, obj, nameof(SampleAsset.Vector2));
			CreateTrigger<Vector3>(root, obj, nameof(SampleAsset.Vector3));
			CreateTrigger<Vector4>(root, obj, nameof(SampleAsset.Vector4));
			CreateTrigger<Rect>(root, obj, nameof(SampleAsset.Rect));
			CreateArraySizeTrigger(root, obj, nameof(SampleAsset.ArraySize));
			CreateTrigger<char>(root, obj, nameof(SampleAsset.Character));
			CreateTrigger<AnimationCurve>(root, obj, nameof(SampleAsset.AnimationCurve));
			CreateTrigger<Bounds>(root, obj, nameof(SampleAsset.Bounds));
			CreateTrigger<Gradient>(root, obj, nameof(SampleAsset.Gradient));
			CreateTrigger<Quaternion>(root, obj, nameof(SampleAsset.Quaternion));
			CreateTrigger<Vector2Int>(root, obj, nameof(SampleAsset.Vector2Int));
			CreateTrigger<Vector3Int>(root, obj, nameof(SampleAsset.Vector3Int));
			CreateTrigger<RectInt>(root, obj, nameof(SampleAsset.RectInt));
			CreateTrigger<BoundsInt>(root, obj, nameof(SampleAsset.BoundsInt));
			CreateTrigger<ISampleReference>(root, obj, nameof(SampleAsset.Reference));

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
