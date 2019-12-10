using System;

namespace PiRhoSoft.Utilities
{
	public class ObjectPickerAttribute : PropertyTraitAttribute
	{
		public Type BaseType { get; private set; }

		public ObjectPickerAttribute() : this(null)
		{
		}

		public ObjectPickerAttribute(Type baseType) : base(FieldPhase, 0)
		{
			BaseType = baseType; 
		}
	}
}
