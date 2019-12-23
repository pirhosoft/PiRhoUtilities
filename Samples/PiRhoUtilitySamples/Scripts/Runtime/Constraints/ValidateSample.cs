using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Validate")]
	public class ValidateSample : MonoBehaviour
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

		[MessageBox("The [Validate] attribute sets the maxmimum length of a string field. It can be hard coded or retrieved from another field, method, or property.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Validate(nameof(IntChanged), "Int must be greater than 3")] public int Integer;
		[Validate(nameof(BoolChanged), "Bool must be true")] public bool Boolean;
		[Validate(nameof(FloatChanged), "Float must be greater than 1")] public float Float;
		[Validate(nameof(StringChanged), "String must not be emtpy")] public string String;
		[Validate(nameof(ColorChanged), "Color must have some red")] public Color Color;
		[Validate(nameof(ObjectChanged), "Object must be set")] public Object ObjectReference;
		[Validate(nameof(LayerChanged), "Layer must be set")] public LayerMask LayerMask;
		[Validate(nameof(EnumChanged), "EnumValue must be Dog")] public Animal EnumValue;
		[Validate(nameof(Vector2Changed), "X must be greater than 1")] public Vector2 Vector2;
		[Validate(nameof(Vector2IntChanged), "X must be greater than 1")] public Vector2Int Vector2Int;
		[Validate(nameof(Vector3Changed), "X must be greater than 1")] public Vector3 Vector3;
		[Validate(nameof(Vector3IntChanged), "X must be greater than 1")] public Vector3Int Vector3Int;
		[Validate(nameof(Vector4Changed), "X must be greater than 1")] public Vector4 Vector4;
		[Validate(nameof(RectChanged), "Width must be greater than 1")] public Rect Rect;
		[Validate(nameof(RectIntChanged), "Height must be greater than 1")] public RectInt RectInt;
		[Validate(nameof(BoundsChanged), "Bounds position must be a the origin")] public Bounds Bounds;
		[Validate(nameof(BoundsIntChanged), "Width must be less than or equal to 1")] public BoundsInt BoundsInt;
		[Validate(nameof(CharChanged), "Character cant be 'A'")] public char Character = 'A';
		[Validate(nameof(AnimationChanged), "Animation must have more than 0 keys")] public AnimationCurve AnimationCurve;
		[Validate(nameof(GradientChanged), "Gradient must be in Blend mode")] public Gradient Gradient;
		[Validate(nameof(QuaternionChanged), "Quaternion can only have z rotation")] [Euler] public Quaternion Quaternion;
		[Validate(nameof(ReferenceChanged), "Object cannot be null")] [SerializeReference] [Reference] public IAnimal ManagedReference = new Dog();

		private bool IntChanged(int value) => value > 3;
		private bool BoolChanged(bool value) => value;
		private bool FloatChanged(float value) => value > 1;
		private bool StringChanged(string value) => !string.IsNullOrEmpty(value);
		private bool ColorChanged(Color value) => value.r > 0;
		private bool ObjectChanged(Object value) => value;
		private bool LayerChanged(int value) => value > 0;
		private bool EnumChanged(Enum value) => (Animal)Enum.Parse(typeof(Animal), value.ToString()) == Animal.Dog;
		private bool Vector2Changed(Vector2 value) => value.x > 1;
		private bool Vector2IntChanged(Vector2Int value) => value.x > 1;
		private bool Vector3Changed(Vector3 value) => value.x > 1;
		private bool Vector3IntChanged(Vector3Int value) => value.x > 1;
		private bool Vector4Changed(Vector4 value) => value.x > 1;
		private bool RectChanged(Rect value) => value.width > 1;
		private bool RectIntChanged(RectInt value) => value.height > 1;
		private bool BoundsChanged(Bounds value) => value.center != Vector3.zero;
		private bool BoundsIntChanged(BoundsInt value) => value.size.x <= 1;
		private bool CharChanged(char value) => value != 'A';
		private bool AnimationChanged(AnimationCurve value) => value.keys.Length > 0;
		private bool GradientChanged(Gradient value) => value.mode != GradientMode.Blend;
		private bool QuaternionChanged(Quaternion value) => value.eulerAngles.x == 0 && value.eulerAngles.y == 0;
		private bool ReferenceChanged(object value) => value != null;
	}
}
