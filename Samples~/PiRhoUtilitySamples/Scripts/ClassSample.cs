using System;
using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Class")]
	public class ClassSample : MonoBehaviour
	{
		[Serializable]
		public class Subclass
		{
			public bool Bool;
			public int Int;
			public float Float;
			public string String;
		}

		[Serializable]
		public class WrapperClass
		{
			public int Value;
		}

		[Inline]
		public Subclass Inline;

		[Rollout]
		public Subclass Rollout;

		[Inline(false)]
		public WrapperClass Wrapper;
	}
}