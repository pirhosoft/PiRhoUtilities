using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Required")]
	public class RequiredSample : MonoBehaviour
	{
		public interface IAnimal
		{
		}

		[Serializable]
		public class Dog : IAnimal
		{
			public int Value;
		}

		[Serializable]
		public class Cat : IAnimal
		{
			public int Value;
		}

		[MessageBox("The [Required] attribute is applied to a string, Object, or ManagedReference and will show a message if they are not set", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Required("A string is required")] public string String;
		[Required("An object is required")] public Object Object;
		[Required("A reference is required")] [SerializeReference] [Reference] public IAnimal Reference;
	}
}
