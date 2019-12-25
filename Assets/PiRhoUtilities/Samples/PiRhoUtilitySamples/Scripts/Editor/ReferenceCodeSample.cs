using PiRhoSoft.Utilities.Editor;
using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Samples
{
	public class ReferenceCodeSample : CodeSample
	{
		public interface ISampleItem
		{
		}

		[Serializable]
		public class SampleItemInt : ISampleItem
		{
			public int IntValue;
		}

		[Serializable]
		public class SampleItemBool : ISampleItem
		{
			public bool BoolValue;
		}

		[Serializable]
		public class SampleItemString : ISampleItem
		{
			public string StringValue;
		}

		public override void Create(VisualElement root)
		{
			var field = new ReferenceField("Reference", typeof(ISampleItem), new SampleItemDrawer());
			field.RegisterValueChangedCallback(evt => Debug.Log($"Reference type changed from {evt.previousValue?.GetType().Name ?? "null"} to {evt.newValue?.GetType().Name ?? "null"}"));
			root.Add(field);
		}

		public class SampleItemDrawer : IReferenceDrawer
		{
			public VisualElement CreateElement(object value)
			{
				switch (value)
				{
					case SampleItemBool b: return CreateSampleItemBool(b);
					case SampleItemInt i: return CreateSampleItemInt(i);
					case SampleItemString s: return CreateSampleItemString(s);
				}

				return null;
			}

			private VisualElement CreateSampleItemBool(SampleItemBool item)
			{
				var field = new Toggle(nameof(SampleItemBool.BoolValue));
				field.value = item.BoolValue;
				field.RegisterValueChangedCallback(e => item.BoolValue = e.newValue);
				return field;
			}

			private VisualElement CreateSampleItemInt(SampleItemInt item)
			{
				var field = new IntegerField(nameof(SampleItemInt.IntValue));
				field.value = item.IntValue;
				field.RegisterValueChangedCallback(e => item.IntValue = e.newValue);
				return field;
			}

			private VisualElement CreateSampleItemString(SampleItemString item)
			{
				var field = new TextField(nameof(SampleItemString.StringValue));
				field.value = item.StringValue;
				field.RegisterValueChangedCallback(e => item.StringValue = e.newValue);
				return field;
			}
		}
	}
}
