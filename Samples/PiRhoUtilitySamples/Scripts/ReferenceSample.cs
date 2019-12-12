using System;
using UnityEngine;

namespace PiRhoSoft.Utilities
{
	public interface ITestReference
	{
	}

	[Serializable]
	public class TestReferenceBool : ITestReference
	{
		public bool Value;
	}

	[Serializable]
	public class TestReferenceInt : ITestReference
	{
		public int Value;
	}

	[AddComponentMenu("PiRho Utilities/Reference")]
	public class ReferenceSample : MonoBehaviour
	{
		[Reference] [SerializeReference] public ITestReference Reference1;
		[Reference] [SerializeReference] public ITestReference Reference2;
	}
}
