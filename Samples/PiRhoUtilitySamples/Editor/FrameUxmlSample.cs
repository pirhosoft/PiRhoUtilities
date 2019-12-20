using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Samples
{
	public class FrameUxmlSample : UxmlSample
	{
		[Serializable]
		public class Painting
		{
			public string Title;
			public string Artist;
			public string Period;
		}

		public class SampleObject : ScriptableObject
		{
			public Painting BoundClass = new Painting();
			public Painting BoundFields = new Painting();
		}

		public override void Setup(VisualElement root)
		{
			var asset = ScriptableObject.CreateInstance<SampleObject>();
			asset.BoundClass.Title = "Mona Lisa";
			asset.BoundClass.Artist = "Leonardo da Vinci";
			asset.BoundClass.Period = "Renaissance";

			var obj = new SerializedObject(asset);
			root.Bind(obj);
		}
	}
}
