using UnityEngine;

namespace PiRhoSoft.Utilities
{
	public class SnapAttribute : PropertyTraitAttribute
	{
		public const int Order = 0;

		public float Number { get; private set; }
		public Vector4 Vector { get; private set; }
		public Bounds Bounds { get; private set; }

		public SnapAttribute(float snap) : base(ValidatePhase, Order)
		{
			Number = snap;
		}

		public SnapAttribute(int snap) : base(ValidatePhase, Order)
		{
			Number = snap;
		}

		public SnapAttribute(float x, float y) : base(ValidatePhase, Order)
		{
			Vector = new Vector2(x, y);
		}

		public SnapAttribute(int x, int y) : base(ValidatePhase, Order)
		{
			Vector = new Vector2(x, y);
		}

		public SnapAttribute(float x, float y, float z) : base(ValidatePhase, Order)
		{
			Vector = new Vector3(x, y, z);
		}

		public SnapAttribute(int x, int y, int z) : base(ValidatePhase, Order)
		{
			Vector = new Vector3(x, y, z);
		}

		public SnapAttribute(float x, float y, float z, float w) : base(ValidatePhase, Order)
		{
			Vector = new Vector4(x, y, z, w);
		}

		public SnapAttribute(int x, int y, int width, int height) : base(ValidatePhase, Order)
		{
			Vector = new Vector4(x, y, width, height);
		}

		public SnapAttribute(float x, float y, float z, float width, float height, float depth) : base(ValidatePhase, Order)
		{
			Bounds = new Bounds(new Vector3(x, y, z), new Vector3(width, height, depth));
		}

		public SnapAttribute(int x, int y, int z, int width, int height, int depth) : base(ValidatePhase, Order)
		{
			Bounds = new Bounds(new Vector3(x, y, z), new Vector3(width, height, depth));
		}
	}
}