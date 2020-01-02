using System;
using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Inline")]
	public class InlineSample : MonoBehaviour
	{
		[Serializable]
		public class MembersClass
		{
			public bool Member1;
			public int Member2;
			public float Member3;
			public string Member4;
		}

		[Serializable]
		public class WrapperClass
		{
			public int Value;
		}

		[MessageBox("The [Inline] attribute displays a serializable class directly inline. Member labels can be optionally shown.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Inline]
		public MembersClass Members;

		[Inline(false)]
		public WrapperClass Wrapper;
	}
}