using UnityEngine;

namespace PiRhoSoft.Utilities
{
	public enum TraitLocation
	{
		Before,
		After,
		Right
	}

	public enum TraitMessageType
	{
		Info,
		Warning,
		Error
	}

	public abstract class PropertyTraitAttribute : PropertyAttribute
	{
		public const int TestPhase = 1;
		public const int PerContainerPhase = 2;
		public const int ContainerPhase = 3;
		public const int FieldPhase = 4;
		public const int ValidatePhase = 5;
		public const int ControlPhase = 6;

		protected PropertyTraitAttribute(int drawPhase, int drawOrder)
		{
			order = int.MaxValue - (drawPhase * 1000 + drawOrder);
		}
	}
}