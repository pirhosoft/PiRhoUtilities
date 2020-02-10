using System;
using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Reference")]
	public class ReferenceSample : MonoBehaviour
	{
		public interface IAnimal
		{
		}

		[Serializable]
		public class Dog : IAnimal
		{
			public int Teeth;
		}

		[Serializable]
		public class Cat : IAnimal
		{
			public int Claws;
		}

		[Serializable]
		public class Hippo : IAnimal
		{
			public int Blubber;
		}

		[MessageBox("The [Reference] attribute is applied in addition to Unity's [SerializeReference] attribute to enable creating and editing of any valid subtype for the field.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[SerializeReference]
		[Reference]
		public IAnimal AnimalReference;
	}
}
