using UnityEngine;

namespace PiRhoSoft.Utilities
{
	public static class MathHelper
	{
		public static int IntExponent(int value, int exponent)
		{
			var result = 1;

			for (var i = 0; i < exponent; i++)
				result *= value;

			return result;
		}

		public static int Snap(int value, int snap)
		{
			return snap > 0 ? Mathf.RoundToInt(value / (float)snap) * snap : value;
		}

		public static float Snap(float value, float snap)
		{
			return snap > 0.0f ? Mathf.Round(value / snap) * snap : value;
		}

		public static int Wrap(int value, int size)
		{
			return (value % size + size) % size;
		}

		public static float Wrap(float value, float length)
		{
			return Mathf.Repeat(value, length);
		}
	}
}
