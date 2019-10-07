using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public interface IDictionaryProxy
	{
		string Label { get; }
		string Tooltip { get; }
		string EmptyLabel { get; }
		string EmptyTooltip { get; }
		string AddPlaceholder { get; }
		string AddTooltip { get; }
		string RemoveTooltip { get; }
		string ReorderTooltip { get; }

		int KeyCount { get; }

		bool AllowAdd { get; }
		bool AllowRemove { get; }
		bool AllowReorder { get; }

		VisualElement CreateField(int index);

		bool NeedsUpdate(VisualElement item, int index);

		bool CanAdd(string key);
		bool CanRemove(int index);
		bool CanReorder(int from, int to);

		void AddItem(string key);
		void RemoveItem(int index);
		void ReorderItem(int from, int to);
	}

	public abstract class DictionaryProxy : IDictionaryProxy
	{
		public const string DefaultEmptyLabel = "The dictionary is empty";
		public const string DefaultEmptyTooltip = "There are no items in this dictionary";
		public const string DefaultAddPlaceholder = "New key";
		public const string DefaultAddTooltip = "Add an item to this dictionary";
		public const string DefaultRemoveTooltip = "Remove this item from the dictionary";
		public const string DefaultReorderTooltip = "Move this item within the dictionary";

		public string Label { get; set; }
		public string Tooltip { get; set; }
		public string EmptyLabel { get; set; } = DefaultEmptyLabel;
		public string EmptyTooltip { get; set; }
		public string AddPlaceholder { get; set; } = DefaultAddPlaceholder;
		public string AddTooltip { get; set; } = DefaultAddTooltip;
		public string RemoveTooltip { get; set; } = DefaultRemoveTooltip;
		public string ReorderTooltip { get; set; } = DefaultReorderTooltip;

		public abstract int KeyCount { get; }

		public virtual bool AllowAdd { get; set; } = true;
		public virtual bool AllowRemove { get; set; } = true;
		public virtual bool AllowReorder { get; set; } = false;

		public abstract VisualElement CreateField(int index);
		public abstract bool NeedsUpdate(VisualElement item, int index);

		public abstract void AddItem(string key);
		public abstract void RemoveItem(int index);
		public virtual void ReorderItem(int from, int to) { }

		public virtual bool CanAdd(string key) => AllowAdd;
		public virtual bool CanRemove(int index) => AllowRemove;
		public virtual bool CanReorder(int from, int to) => AllowReorder;
	}

	public class DictionaryProxy<T> : DictionaryProxy
	{
		public Dictionary<string, T> Items { get => _items; set => SetItems(value); }

		public override int KeyCount => Items.Count;

		private readonly Func<string, VisualElement> _createElement;

		private Dictionary<string, T> _items = new Dictionary<string, T>();
		private List<string> _indexMap = new List<string>();

		public DictionaryProxy(Dictionary<string, T> items, Func<string, VisualElement> createElement)
		{
			Items = items;
			_createElement = createElement;
		}

		public override VisualElement CreateField(int index)
		{
			var key = GetKey(index);
			var element = _createElement?.Invoke(key) ?? new VisualElement();
			element.userData = key;
			return element;
		}

		public override bool NeedsUpdate(VisualElement item, int index)
		{
			var key = GetKey(index);
			return !(item.userData is string k) || k != key;
		}

		public override bool CanAdd(string key)
		{
			return AllowAdd && !Items.ContainsKey(key);
		}

		public override void AddItem(string key)
		{
			Items.Add(key, default);
		}

		public override void RemoveItem(int index)
		{
			var key = GetKey(index);
			Items.Remove(key);
		}

		private string GetKey(int index)
		{
			return _indexMap[index];
		}

		private void SetItems(Dictionary<string, T> items)
		{
			_items = items;
			_indexMap.Clear();

			foreach (var item in Items)
				_indexMap.Add(item.Key);
		}
	}
}