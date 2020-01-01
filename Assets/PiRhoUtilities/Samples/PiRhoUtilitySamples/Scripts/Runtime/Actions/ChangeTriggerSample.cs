using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/ChangeTrigger")]
	public class ChangeTriggerSample : MonoBehaviour
	{
		public enum Animal
		{
			Cat,
			Dog,
			Rabbit
		}

		public interface IAnimal
		{
		}

		[Serializable]
		public class Cat : IAnimal
		{
			public int Value;
		}

		[Serializable]
		public class Dog : IAnimal
		{
			public int Value;
		}

		[MessageBox("The [ChangeTrigger] attribute calls a method on the clsss when the field changes. The method can be public/private, optionally static, and takes zero, one(newValue), or two(oldValue, newValue) parameters.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[ChangeTrigger(nameof(IntChanged))] public int Integer;
		[ChangeTrigger(nameof(BoolChanged))] public bool Boolean;
		[ChangeTrigger(nameof(FloatChanged))] public float Float;
		[ChangeTrigger(nameof(StringChanged))] public string String;
		[ChangeTrigger(nameof(ColorChanged))] public Color Color;
		[ChangeTrigger(nameof(ObjectChanged))] public Object ObjectReference;
		[ChangeTrigger(nameof(LayerChanged))] public LayerMask LayerMask;
		[ChangeTrigger(nameof(EnumChanged))] public Animal Enum;
		[ChangeTrigger(nameof(Vector2Changed))] public Vector2 Vector2;
		[ChangeTrigger(nameof(Vector2IntChanged))] public Vector2Int Vector2Int;
		[ChangeTrigger(nameof(Vector3Changed))] public Vector3 Vector3;
		[ChangeTrigger(nameof(Vector3IntChanged))] public Vector3Int Vector3Int;
		[ChangeTrigger(nameof(Vector4Changed))] public Vector4 Vector4;
		[ChangeTrigger(nameof(RectChanged))] public Rect Rect;
		[ChangeTrigger(nameof(RectIntChanged))] public RectInt RectInt;
		[ChangeTrigger(nameof(BoundsChanged))] public Bounds Bounds;
		[ChangeTrigger(nameof(BoundsIntChanged))] public BoundsInt BoundsInt;
		[ChangeTrigger(nameof(CharChanged))] public char Character;
		[ChangeTrigger(nameof(AnimationChanged))] public AnimationCurve AnimationCurve;
		[ChangeTrigger(nameof(GradientChanged))] public Gradient Gradient;
		[ChangeTrigger(nameof(QuaternionChanged))] public Quaternion Quaternion;
		[ChangeTrigger(nameof(ReferenceChanged))] [SerializeReference] [Reference] public IAnimal ManagedReference1 = new Dog();
		[ChangeTrigger(nameof(ReferenceChanged))] [SerializeReference] [Reference] public IAnimal ManagedReference2 = new Cat(); // At least two references are required for bindings to work due to a Unity bug

		[ChangeTrigger(nameof(PublicStaticParameterlessChanged))] public int PublicStaticParameterless;
		[ChangeTrigger(nameof(PublicStaticOneParameterChanged))] public int PublicStaticOneParameter;
		[ChangeTrigger(nameof(PublicStaticTwoParametersChanged))] public int PublicStaticTwoParameters;
		[ChangeTrigger(nameof(PublicInstanceParameterlessChanged))] public int PublicInstanceParameterless;
		[ChangeTrigger(nameof(PublicInstanceOneParameterChanged))] public int PublicInstanceOneParameter;
		[ChangeTrigger(nameof(PublicInstanceTwoParametersChanged))] public int PublicInstanceTwoParameters;
		[ChangeTrigger(nameof(PrivateStaticParameterlessChanged))] public int PrivateStaticParameterless;
		[ChangeTrigger(nameof(PrivateStaticOneParameterChanged))] public int PrivateStaticOneParameter;
		[ChangeTrigger(nameof(PrivateStaticTwoParametersChanged))] public int PrivateStaticTwoParameters;
		[ChangeTrigger(nameof(PrivateInstanceParameterlessChanged))] public int PrivateInstanceParameterless;
		[ChangeTrigger(nameof(PrivateInstanceOneParameterChanged))] public int PrivateInstanceOneParameter;
		[ChangeTrigger(nameof(PrivateInstanceTwoParametersChanged))] public int PrivateInstanceTwoParameters;

		private void IntChanged(int from, int to)
		{
			Debug.LogFormat("Int changed from '{0}' to '{1}'", from, to);
		}

		private void BoolChanged(bool from, bool to)
		{
			Debug.LogFormat("Bool changed from '{0}' to '{1}'", from, to);
		}

		private void FloatChanged(float from, float to)
		{
			Debug.LogFormat("Float changed from '{0}' to '{1}'", from, to);
		}

		private void StringChanged(string from, string to)
		{
			Debug.LogFormat("String changed from '{0}' to '{1}'", from, to);
		}

		private void ColorChanged(Color from, Color to)
		{
			Debug.LogFormat("Color changed from '{0}' to '{1}'", from, to);
		}

		private void ObjectChanged(Object from, Object to)
		{
			Debug.LogFormat("Object changed from '{0}' to '{1}'", from, to);
		}

		private void LayerChanged(int from, int to)
		{
			Debug.LogFormat("Layer changed from '{0}' to '{1}'", from, to);
		}

		private void EnumChanged(Enum from, Enum to)
		{
			Debug.LogFormat("Enum changed from '{0}' to '{1}'", from, to);
		}

		private void Vector2Changed(Vector2 from, Vector2 to)
		{
			Debug.LogFormat("Vector2 changed from '{0}' to '{1}'", from, to);
		}

		private void Vector2IntChanged(Vector2Int from, Vector2Int to)
		{
			Debug.LogFormat("Vector2Int changed from '{0}' to '{1}'", from, to);
		}

		private void Vector3Changed(Vector3 from, Vector3 to)
		{
			Debug.LogFormat("Vector3 changed from '{0}' to '{1}'", from, to);
		}

		private void Vector3IntChanged(Vector3Int from, Vector3Int to)
		{
			Debug.LogFormat("Vector3Int changed from '{0}' to '{1}'", from, to);
		}

		private void Vector4Changed(Vector4 from, Vector4 to)
		{
			Debug.LogFormat("Vector4 changed from '{0}' to '{1}'", from, to);
		}

		private void RectChanged(Rect from, Rect to)
		{
			Debug.LogFormat("Rect changed from '{0}' to '{1}'", from, to);
		}

		private void RectIntChanged(RectInt from, RectInt to)
		{
			Debug.LogFormat("RectInt changed from '{0}' to '{1}'", from, to);
		}

		private void BoundsChanged(Bounds from, Bounds to)
		{
			Debug.LogFormat("Bounds changed from '{0}' to '{1}'", from, to);
		}

		private void BoundsIntChanged(BoundsInt from, BoundsInt to)
		{
			Debug.LogFormat("BoundsInt changed from '{0}' to '{1}'", from, to);
		}

		private void CharChanged(char from, char to)
		{
			Debug.LogFormat("Char changed from '{0}' to '{1}'", from, to);
		}

		private void AnimationChanged(AnimationCurve from, AnimationCurve to)
		{
			Debug.LogFormat("Animation changed from '{0}' to '{1}'", from, to);
		}

		private void GradientChanged(Gradient from, Gradient to)
		{
			Debug.LogFormat("Gradient changed from '{0}' to '{1}'", from, to);
		}

		private void QuaternionChanged(Quaternion from, Quaternion to)
		{
			Debug.LogFormat("Quaternion changed from '{0}' to '{1}'", from, to);
		}

		private void ReferenceChanged(object from, object to)
		{
			Debug.LogFormat("Reference changed from '{0}' to '{1}'", from, to);
		}

		public static void PublicStaticParameterlessChanged()
		{
			Debug.Log("Public static method - parameterless");
		}

		public static void PublicStaticOneParameterChanged(int newValue)
		{
			Debug.LogFormat("Public static method - '{0}'", newValue);
		}

		public static void PublicStaticTwoParametersChanged(int oldValue, int newValue)
		{
			Debug.LogFormat("Public static method - '{0}' to '{1}'", oldValue, newValue);
		}

		public void PublicInstanceParameterlessChanged()
		{
			Debug.Log("Public instance method - parameterless");
		}

		public void PublicInstanceOneParameterChanged(int newValue)
		{
			Debug.LogFormat("Public instance method - '{0}'", newValue);
		}

		public void PublicInstanceTwoParametersChanged(int oldValue, int newValue)
		{
			Debug.LogFormat("Public instance method - '{0}' to '{1}'", oldValue, newValue);
		}

		private static void PrivateStaticParameterlessChanged()
		{
			Debug.Log("Private static method - parameterless");
		}

		private static void PrivateStaticOneParameterChanged(int newValue)
		{
			Debug.LogFormat("Private static method - '{0}'", newValue);
		}

		private static void PrivateStaticTwoParametersChanged(int oldValue, int newValue)
		{
			Debug.LogFormat("Pirivate static method - '{0}' to '{1}'", oldValue, newValue);
		}

		private static void PrivateInstanceParameterlessChanged()
		{
			Debug.Log("Private instance method - parameterless");
		}

		private static void PrivateInstanceOneParameterChanged(int newValue)
		{
			Debug.LogFormat("Private instance method - '{0}'", newValue);
		}

		private static void PrivateInstanceTwoParametersChanged(int oldValue, int newValue)
		{
			Debug.LogFormat("Pirivate instance method - '{0}' to '{1}'", oldValue, newValue);
		}
	}
}