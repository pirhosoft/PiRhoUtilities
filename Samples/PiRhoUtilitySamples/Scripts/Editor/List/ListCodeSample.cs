using PiRhoSoft.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Samples
{
	public class ListCodeSample : CodeSample
	{
		public interface ISampleItem
		{
		}

		[Serializable]
		public class SampleItemInt : ISampleItem
		{
			public int IntValue;

			public override string ToString() => $"SampleItemInt: {IntValue}";
		}

		[Serializable]
		public class SampleItemBool : ISampleItem
		{
			public bool BoolValue;

			public override string ToString() => $"SampleItemBool: {BoolValue}";
		}

		[Serializable]
		public class SampleItemString : ISampleItem
		{
			public string StringValue;

			public override string ToString() => $"SampleItemString: {StringValue}";
		}

		private class ProxySelect
		{
			public string Name { get; private set; }
			public IListProxy Proxy { get; private set; }
			public Type ItemType { get; private set; }
			public bool AllowDerived { get; private set; }
			public IList List { get; private set; }

			public ProxySelect(string name, IListProxy proxy, Type itemType, bool allowDerived, IList list)
			{
				Name = name;
				Proxy = proxy;
				ItemType = itemType;
				AllowDerived = allowDerived;
				List = list;
			}
		}

		[Serializable] public class IntList : SerializedList<int> { }
		[Serializable] public class SampleItemBoolList : SerializedList<SampleItemBool> { }
		[Serializable] public class SampleItemList : ReferenceList<ISampleItem> { }

		public class SampleAsset : ScriptableObject
		{
			public IntList Primitives = new IntList();
			public SampleItemBoolList Classes = new SampleItemBoolList();
			public SampleItemList References = new SampleItemList();
		}

		private IListProxy CreatePropertyProxy(SerializedObject obj, string propertyName)
		{
			var property = obj.FindProperty(propertyName);
			return new PropertyListProxy(property, null);
		}

		private VisualElement CreateInt(IList list, int index)
		{
			var field = new IntegerField();
			field.value = (int)list[index];
			field.RegisterValueChangedCallback(e => list[index] = e.newValue);
			return field;
		}

		private VisualElement CreateSampleItemBool(IList list, int index)
		{
			return CreateSampleItemBool(list[index] as SampleItemBool);
		}

		private VisualElement CreateSampleItem(IList list, int index)
		{
			var item = list[index] as ISampleItem;

			switch (item)
			{
				case SampleItemBool b: return CreateSampleItemBool(b);
				case SampleItemInt i: return CreateSampleItemInt(i);
				case SampleItemString s: return CreateSampleItemString(s);
			}

			return null;
		}

		private VisualElement CreateInt(IList<int> list, int index)
		{
			var field = new IntegerField();
			field.value = list[index];
			field.RegisterValueChangedCallback(e => list[index] = e.newValue);
			return field;
		}

		private VisualElement CreateSampleItemBool(IList<SampleItemBool> list, int index)
		{
			return CreateSampleItemBool(list[index]);
		}

		private VisualElement CreateSampleItem(IList<ISampleItem> list, int index)
		{
			var item = list[index];

			switch (item)
			{
				case SampleItemBool b: return CreateSampleItemBool(b);
				case SampleItemInt i: return CreateSampleItemInt(i);
				case SampleItemString s: return CreateSampleItemString(s);
			}

			return null;
		}

		private VisualElement CreateSampleItemBool(SampleItemBool item)
		{
			var foldout = new Foldout();
			foldout.text = nameof(SampleItemBool);

			var field = new Toggle(nameof(SampleItemBool.BoolValue));
			field.value = item.BoolValue;
			field.RegisterValueChangedCallback(e => item.BoolValue = e.newValue);
			foldout.Add(field);

			return foldout;
		}

		private VisualElement CreateSampleItemInt(SampleItemInt item)
		{
			var foldout = new Foldout();
			foldout.text = nameof(SampleItemInt);

			var field = new IntegerField(nameof(SampleItemInt.IntValue));
			field.value = item.IntValue;
			field.RegisterValueChangedCallback(e => item.IntValue = e.newValue);
			foldout.Add(field);

			return foldout;
		}

		private VisualElement CreateSampleItemString(SampleItemString item)
		{
			var foldout = new Foldout();
			foldout.text = nameof(SampleItemString);

			var field = new TextField(nameof(SampleItemString.StringValue));
			field.value = item.StringValue;
			field.RegisterValueChangedCallback(e => item.StringValue = e.newValue);
			foldout.Add(field);

			return foldout;
		}

		public override void Create(VisualElement root)
		{
			var asset = ScriptableObject.CreateInstance<SampleAsset>();
			var obj = new SerializedObject(asset);
			var typedPrimitives = new List<int>();
			var typedClasses = new List<SampleItemBool>();
			var typedReferences = new List<ISampleItem>();
			var genericPrimitives = new List<int>();
			var genericClasses = new List<SampleItemBool>();
			var genericReferences = new List<ISampleItem>();

			var proxies = new List<ProxySelect>
			{
				new ProxySelect("Typed Primitive", new ListProxy<int>(typedPrimitives, CreateInt), null, false, typedPrimitives),
				new ProxySelect("Typed Class", new ListProxy<SampleItemBool>(typedClasses, CreateSampleItemBool), typeof(SampleItemBool), false, typedClasses),
				new ProxySelect("Typed Reference", new ListProxy<ISampleItem>(typedReferences, CreateSampleItem), typeof(ISampleItem), true, typedReferences),
				new ProxySelect("Generic Primitive", new ListProxy(genericPrimitives, CreateInt), typeof(int), false, genericPrimitives),
				new ProxySelect("Generic Class", new ListProxy(genericClasses, CreateSampleItemBool), typeof(SampleItemBool), false, genericClasses),
				new ProxySelect("Generic Reference", new ListProxy(genericReferences, CreateSampleItem), typeof(ISampleItem), true, genericReferences),
				new ProxySelect("Property Primitive", CreatePropertyProxy(obj, nameof(SampleAsset.Primitives)), null, false, asset.Primitives),
				new ProxySelect("Property Class", CreatePropertyProxy(obj, nameof(SampleAsset.Classes)), null, false, asset.Classes),
				new ProxySelect("Property Reference", CreatePropertyProxy(obj, nameof(SampleAsset.References)), typeof(ISampleItem), true, asset.References),
				new ProxySelect("None", null, null, false, null)
			};

			var startingProxy = 0;
			var list = new ListField();
			list.Label = "Sample List";
			list.SetProxy(proxies[startingProxy].Proxy, proxies[startingProxy].ItemType, proxies[startingProxy].AllowDerived);
			root.Add(list);

			var proxyPopup = new UnityEditor.UIElements.PopupField<ProxySelect>("Item Type", proxies, startingProxy, proxy => proxy.Name, proxy => proxy.Name);
			proxyPopup.RegisterValueChangedCallback(e => list.SetProxy(e.newValue.Proxy, e.newValue.ItemType, e.newValue.AllowDerived));
			root.Add(proxyPopup);

			var allowAddToggle = new Toggle("Allow Add");
			allowAddToggle.value = list.AllowAdd;
			allowAddToggle.RegisterValueChangedCallback(e => list.AllowAdd = e.newValue);
			root.Add(allowAddToggle);

			var allowRemoveToggle = new Toggle("Allow Remove");
			allowRemoveToggle.value = list.AllowRemove;
			allowRemoveToggle.RegisterValueChangedCallback(e => list.AllowRemove = e.newValue);
			root.Add(allowRemoveToggle);

			var allowReorderToggle = new Toggle("Allow Reorder");
			allowReorderToggle.value = list.AllowReorder;
			allowReorderToggle.RegisterValueChangedCallback(e => list.AllowReorder = e.newValue);
			root.Add(allowReorderToggle);

			var emptyLabelText = new TextField("Empty Label");
			emptyLabelText.value = list.EmptyLabel;
			emptyLabelText.RegisterValueChangedCallback(e => list.EmptyLabel = e.newValue);
			root.Add(emptyLabelText);

			var emptyTooltipText = new TextField("Empty Tooltip");
			emptyTooltipText.value = list.EmptyTooltip;
			emptyTooltipText.RegisterValueChangedCallback(e => list.EmptyTooltip = e.newValue);
			root.Add(emptyTooltipText);

			var addTooltipText = new TextField("Add Tooltip");
			addTooltipText.value = list.AddTooltip;
			addTooltipText.RegisterValueChangedCallback(e => list.AddTooltip = e.newValue);
			root.Add(addTooltipText);

			var removeTooltipText = new TextField("Remove Tooltip");
			removeTooltipText.value = list.RemoveTooltip;
			removeTooltipText.RegisterValueChangedCallback(e => list.RemoveTooltip = e.newValue);
			root.Add(removeTooltipText);

			var reorderTooltipText = new TextField("Reorder Tooltip");
			reorderTooltipText.value = list.ReorderTooltip;
			reorderTooltipText.RegisterValueChangedCallback(e => list.ReorderTooltip = e.newValue);
			root.Add(reorderTooltipText);

			var printButton = new Button(() => Print(proxies[proxyPopup.index].List));
			printButton.text = "Log List Contents";
			root.Add(printButton);
		}

		private void Print(IList list)
		{
			if (list == null)
			{
				Debug.Log("The proxy has not been set");
			}
			else if (list.Count == 0)
			{
				Debug.Log("The list is empty");
			}
			else
			{
				var builder = new StringBuilder();
				var first = true;

				foreach (var item in list)
				{
					if (!first)
						builder.Append(", ");

					builder.Append(item.ToString());
					first = false;
				}

				Debug.Log(builder.ToString());
			}
		}
	}
}
