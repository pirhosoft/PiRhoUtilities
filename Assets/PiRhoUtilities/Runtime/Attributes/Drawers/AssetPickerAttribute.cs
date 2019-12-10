using System;
using UnityEngine.AddressableAssets;

namespace PiRhoSoft.Utilities
{
	public class AssetPickerAttribute : PropertyTraitAttribute
	{
		public Type Type { get; private set; }
		public string Tag { get; private set; }
		public string[] Tags { get; private set; }
		public Addressables.MergeMode Mode { get; private set; }

		public AssetPickerAttribute() : base(FieldPhase, 0)
		{
		}

		public AssetPickerAttribute(Type type) : this()
		{
			Type = type;
		}

		public AssetPickerAttribute(string tag, Type type = null) : this(type)
		{
			Tag = tag;
		}

		public AssetPickerAttribute(string[] tags, Type type = null, Addressables.MergeMode mode = Addressables.MergeMode.Intersection) : this(type)
		{
			Tags = tags;
			Mode = mode;
		}
	}
}
