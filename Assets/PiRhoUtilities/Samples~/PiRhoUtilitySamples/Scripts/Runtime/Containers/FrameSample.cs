using System;
using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Frame")]
	public class FrameSample : MonoBehaviour
	{
		[Serializable]
		public class Subclass
		{
			public bool Bool;
			public int Int;
			public float Float;
			public string String;
		}

		[MessageBox("The [Frame] attribute displays a serializable class or stuct a frame like view.", MessageBoxType.Info, Location = TraitLocation.Above)]

		[Frame]
		public Subclass Frame;
	}
}