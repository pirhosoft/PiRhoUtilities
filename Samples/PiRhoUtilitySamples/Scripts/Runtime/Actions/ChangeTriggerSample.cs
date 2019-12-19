using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/ChangeTrigger")]
	public class ChangeTriggerSample : MonoBehaviour
	{
		[ChangeTrigger(nameof(IntChanged))] public int Integer;
		[ChangeTrigger(nameof(IntChanged))] public bool Boolean;
		[ChangeTrigger(nameof(IntChanged))] public float Float;
		[ChangeTrigger(nameof(IntChanged))] public string String;
		[ChangeTrigger(nameof(IntChanged))] public Color Color;
		[ChangeTrigger(nameof(IntChanged))] public Object ObjectReference;
		[ChangeTrigger(nameof(IntChanged))] public LayerMask LayerMask;
		[ChangeTrigger(nameof(IntChanged))] public int Enum;
		[ChangeTrigger(nameof(IntChanged))] public Vector2 Vector2;
		[ChangeTrigger(nameof(IntChanged))] public Vector2Int Vector2Int;
		[ChangeTrigger(nameof(IntChanged))] public Vector3 Vector3;
		[ChangeTrigger(nameof(IntChanged))] public Vector3Int Vector3Int;
		[ChangeTrigger(nameof(IntChanged))] public Vector4 Vector4;
		[ChangeTrigger(nameof(IntChanged))] public Rect Rect;
		[ChangeTrigger(nameof(IntChanged))] public RectInt RectInt;
		[ChangeTrigger(nameof(IntChanged))] public Bounds Bounds;
		[ChangeTrigger(nameof(IntChanged))] public BoundsInt BoundsInt;
		[ChangeTrigger(nameof(IntChanged))] public char Character;
		[ChangeTrigger(nameof(IntChanged))] public AnimationCurve AnimationCurve;
		[ChangeTrigger(nameof(IntChanged))] public Gradient Gradient;
		[ChangeTrigger(nameof(IntChanged))] public Quaternion Quaternion;
		[ChangeTrigger(nameof(IntChanged))] public int ManagedReference;

		[ChangeTrigger(nameof(PublicStaticParameterless))] public int PublicStaticParameterless;
		[ChangeTrigger(nameof(PublicStaticOneParameter))] public int PublicStaticOneParameter;
		[ChangeTrigger(nameof(PublicStaticTwoParameters))] public int PublicStaticTwoParameters;
		[ChangeTrigger(nameof(PrivateStaticParameterless))] public int PrivateStaticParameterless;
		[ChangeTrigger(nameof(PrivateStaticOneParameter))] public int PrivateStaticOneParameter;
		[ChangeTrigger(nameof(PrivateStaticTwoParameters))] public int PrivateStaticTwoParameters;
		[ChangeTrigger(nameof(PublicInstanceParameterless))] public int PublicInstanceParameterless;
		[ChangeTrigger(nameof(PublicInstanceOneParameter))] public int PublicInstanceOneParameter;
		[ChangeTrigger(nameof(PublicInstanceTwoParameters))] public int PublicInstanceTwoParameters;
		[ChangeTrigger(nameof(PrivateInstanceParameterless))] public int PrivateInstanceParameterless;
		[ChangeTrigger(nameof(PrivateInstanceOneParameter))] public int PrivateInstanceOneParameter;
		[ChangeTrigger(nameof(PrivateInstanceTwoParameters))] public int PrivateInstanceTwoParameters;

		public static void PublicInstanceParameterlessChanged()
		{
		}

		public static void PublicInstanceOneParameterChanged(int newValue)
		{
		}

		public static void PublicInstanceTwoParametersChanged(int oldValue, int newValue)
		{
		}

		private void IntChanged(int from, int to)
		{
		}

		public static void PublicStaticParameterlessChanged()
		{
		}

		public static void PublicStaticOneParameterChanged(int newValue)
		{
		}

		public static void PublicStaticTwoParametersChanged(int oldValue, int newValue)
		{
		}

		private static void PrivateStaticParameterlessChanged()
		{
		}

		private static void PrivateStaticOneParameterChanged(int newValue)
		{
		}

		private static void PrivateStaticTwoParametersChanged(int oldValue, int newValue)
		{
		}

		private static void PrivateInstanceParameterlessChanged()
		{
		}

		private static void PrivateInstanceOneParameterChanged(int newValue)
		{
		}

		private static void PrivateInstanceTwoParametersChanged(int oldValue, int newValue)
		{
		}
	}
}