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
	public class DictionaryCodeSample : CodeSample
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
			public IDictionaryProxy Proxy { get; private set; }
			public Type ItemType { get; private set; }
			public bool AllowDerived { get; private set; }
			public IDictionary Dictionary { get; private set; }

			public ProxySelect(string name, IDictionaryProxy proxy, Type itemType, bool allowDerived, IDictionary dictionary)
			{
				Name = name;
				Proxy = proxy;
				ItemType = itemType;
				AllowDerived = allowDerived;
				Dictionary = dictionary;
			}
		}

		[Serializable] public class IntDictionary : SerializedDictionary<string, int> { }
		[Serializable] public class SampleItemBoolDictionary : SerializedDictionary<string, SampleItemBool> { }
		[Serializable] public class SampleItemDictionary : ReferenceDictionary<string, ISampleItem> { }

		public class SampleAsset : ScriptableObject
		{
			public IntDictionary Primitives = new IntDictionary();
			public SampleItemBoolDictionary Classes = new SampleItemBoolDictionary();
			public SampleItemDictionary References = new SampleItemDictionary();
		}

		private IDictionaryProxy CreatePropertyProxy(SerializedObject obj, string propertyName)
		{
			var property = obj.FindProperty(propertyName);
			var keys = property.FindPropertyRelative(SerializedDictionary<string, int>.KeyProperty);
			var values = property.FindPropertyRelative(SerializedDictionary<string, int>.ValueProperty);

			return new PropertyDictionaryProxy(property, keys, values, null);
		}

		private VisualElement CreateInt(IDictionary dictionary, string key)
		{
			var field = new IntegerField(key);
			field.value = (int)dictionary[key];
			field.RegisterValueChangedCallback(e => dictionary[key] = e.newValue);
			return field;
		}

		private VisualElement CreateSampleItemBool(IDictionary dictionary, string key)
		{
			return CreateSampleItemBool(dictionary[key] as SampleItemBool, key);
		}

		private VisualElement CreateSampleItem(IDictionary dictionary, string key)
		{
			var item = dictionary[key] as ISampleItem;

			switch (item)
			{
				case SampleItemBool b: return CreateSampleItemBool(b, key);
				case SampleItemInt i: return CreateSampleItemInt(i, key);
				case SampleItemString s: return CreateSampleItemString(s, key);
			}

			return null;
		}

		private VisualElement CreateInt(IDictionary<string, int> dictionary, string key)
		{
			var field = new IntegerField(key);
			field.value = dictionary[key];
			field.RegisterValueChangedCallback(e => dictionary[key] = e.newValue);
			return field;
		}

		private VisualElement CreateSampleItemBool(IDictionary<string, SampleItemBool> dictionary, string key)
		{
			return CreateSampleItemBool(dictionary[key] as SampleItemBool, key);
		}

		private VisualElement CreateSampleItem(IDictionary<string, ISampleItem> dictionary, string key)
		{
			var item = dictionary[key];

			switch (item)
			{
				case SampleItemBool b: return CreateSampleItemBool(b, key);
				case SampleItemInt i: return CreateSampleItemInt(i, key);
				case SampleItemString s: return CreateSampleItemString(s, key);
			}

			return null;
		}

		private VisualElement CreateSampleItemBool(SampleItemBool item, string key)
		{
			var foldout = new Foldout();
			foldout.text = key;

			var field = new Toggle(nameof(SampleItemBool.BoolValue));
			field.value = item.BoolValue;
			field.RegisterValueChangedCallback(e => item.BoolValue = e.newValue);
			foldout.Add(field);

			return foldout;
		}

		private VisualElement CreateSampleItemInt(SampleItemInt item, string key)
		{
			var foldout = new Foldout();
			foldout.text = key;

			var field = new IntegerField(nameof(SampleItemInt.IntValue));
			field.value = item.IntValue;
			field.RegisterValueChangedCallback(e => item.IntValue = e.newValue);
			foldout.Add(field);

			return foldout;
		}


		private VisualElement CreateSampleItemString(SampleItemString item, string key)
		{
			var foldout = new Foldout();
			foldout.text = key;

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
			var typedPrimitives = new Dictionary<string, int>();
			var typedClasses = new Dictionary<string, SampleItemBool>();
			var typedReferences = new Dictionary<string, ISampleItem>();
			var genericPrimitives = new Dictionary<string, int>();
			var genericClasses = new Dictionary<string, SampleItemBool>();
			var genericReferences = new Dictionary<string, ISampleItem>();

			var proxies = new List<ProxySelect>
			{
				new ProxySelect("Typed Primitive", new DictionaryProxy<int>(typedPrimitives, CreateInt), null, false, typedPrimitives),
				new ProxySelect("Typed Class", new DictionaryProxy<SampleItemBool>(typedClasses, CreateSampleItemBool), typeof(SampleItemBool), false, typedClasses),
				new ProxySelect("Typed Reference", new DictionaryProxy<ISampleItem>(typedReferences, CreateSampleItem), typeof(ISampleItem), true, typedReferences),
				new ProxySelect("Generic Primitive", new DictionaryProxy(genericPrimitives, CreateInt), typeof(int), false, genericPrimitives),
				new ProxySelect("Generic Class", new DictionaryProxy(genericClasses, CreateSampleItemBool), typeof(SampleItemBool), false, genericClasses),
				new ProxySelect("Generic Reference", new DictionaryProxy(genericReferences, CreateSampleItem), typeof(ISampleItem), true, genericReferences),
				new ProxySelect("Property Primitive", CreatePropertyProxy(obj, nameof(SampleAsset.Primitives)), null, false, asset.Primitives),
				new ProxySelect("Property Class", CreatePropertyProxy(obj, nameof(SampleAsset.Classes)), null, false, asset.Classes),
				new ProxySelect("Property Reference", CreatePropertyProxy(obj, nameof(SampleAsset.References)), typeof(ISampleItem), true, asset.References),
				new ProxySelect("None", null, null, false, null)
			};

			var startingProxy = 0;
			var dictionary = new DictionaryField();
			dictionary.Label = "Sample Dictionary";
			dictionary.SetProxy(proxies[startingProxy].Proxy, proxies[startingProxy].ItemType, proxies[startingProxy].AllowDerived);
			root.Add(dictionary);

			var proxyPopup = new UnityEditor.UIElements.PopupField<ProxySelect>("Item Type", proxies, startingProxy, proxy => proxy.Name, proxy => proxy.Name);
			proxyPopup.RegisterValueChangedCallback(e => dictionary.SetProxy(e.newValue.Proxy, e.newValue.ItemType, e.newValue.AllowDerived));
			root.Add(proxyPopup);

			var allowAddToggle = new Toggle("Allow Add");
			allowAddToggle.value = dictionary.AllowAdd;
			allowAddToggle.RegisterValueChangedCallback(e => dictionary.AllowAdd = e.newValue);
			root.Add(allowAddToggle);

			var allowRemoveToggle = new Toggle("Allow Remove");
			allowRemoveToggle.value = dictionary.AllowRemove;
			allowRemoveToggle.RegisterValueChangedCallback(e => dictionary.AllowRemove = e.newValue);
			root.Add(allowRemoveToggle);

			var allowReorderToggle = new Toggle("Allow Reorder");
			allowReorderToggle.tooltip = "For properties only - regular dictionaries cannot be reorderable";
			allowReorderToggle.value = dictionary.AllowReorder;
			allowReorderToggle.RegisterValueChangedCallback(e => dictionary.AllowReorder = e.newValue);
			root.Add(allowReorderToggle);

			var emptyLabelText = new TextField("Empty Label");
			emptyLabelText.value = dictionary.EmptyLabel;
			emptyLabelText.RegisterValueChangedCallback(e => dictionary.EmptyLabel = e.newValue);
			root.Add(emptyLabelText);

			var emptyTooltipText = new TextField("Empty Tooltip");
			emptyTooltipText.value = dictionary.EmptyTooltip;
			emptyTooltipText.RegisterValueChangedCallback(e => dictionary.EmptyTooltip = e.newValue);
			root.Add(emptyTooltipText);

			var addTooltipText = new TextField("Add Tooltip");
			addTooltipText.value = dictionary.AddTooltip;
			addTooltipText.RegisterValueChangedCallback(e => dictionary.AddTooltip = e.newValue);
			root.Add(addTooltipText);

			var addPlaceholderText = new TextField("Add Placeholder");
			addPlaceholderText.value = dictionary.AddPlaceholder;
			addPlaceholderText.RegisterValueChangedCallback(e => dictionary.AddPlaceholder = e.newValue);
			root.Add(addPlaceholderText);

			var removeTooltipText = new TextField("Remove Tooltip");
			removeTooltipText.value = dictionary.RemoveTooltip;
			removeTooltipText.RegisterValueChangedCallback(e => dictionary.RemoveTooltip = e.newValue);
			root.Add(removeTooltipText);

			var reorderTooltipText = new TextField("Reorder Tooltip");
			reorderTooltipText.value = dictionary.ReorderTooltip;
			reorderTooltipText.RegisterValueChangedCallback(e => dictionary.ReorderTooltip = e.newValue);
			root.Add(reorderTooltipText);

			var printButton = new Button(() => Print(proxies[proxyPopup.index].Dictionary));
			printButton.text = "Log Dictionary Contents";
			root.Add(printButton);
		}

		private void Print(IDictionary dictionary)
		{
			if (dictionary == null)
			{
				Debug.Log("The proxy has not been set");
			}
			else if (dictionary.Count == 0)
			{
				Debug.Log("The dictionary is empty");
			}
			else
			{
				var builder = new StringBuilder();
				var first = true;

				foreach (var key in dictionary.Keys)
				{
					if (!first)
						builder.Append(", ");

					builder.Append($"{key}: {dictionary[key]}");
					first = false;
				}

				Debug.Log(builder.ToString());
			}
		}
	}
}
