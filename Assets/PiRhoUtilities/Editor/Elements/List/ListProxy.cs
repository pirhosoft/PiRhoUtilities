using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public interface IListProxy
	{
		string Label { get; }
		string Tooltip { get; }
		string EmptyLabel { get; }
		string EmptyTooltip { get; }
		string AddTooltip { get; }
		string RemoveTooltip { get; }
		string ReorderTooltip { get; }

		int ItemCount { get; }
		bool AllowAdd { get; }
		bool AllowRemove { get; }
		bool AllowReorder { get; }

		VisualElement CreateElement(int index);
		bool NeedsUpdate(VisualElement item, int index);

		bool CanAdd();
		bool CanRemove(int index);
		bool CanReorder(int from, int to);

		void AddItem();
		void RemoveItem(int index);
		void ReorderItem(int from, int to);
	}

	public abstract class ListProxy : IListProxy
	{
		public const string DefaultEmptyLabel = "The list is empty";
		public const string DefaultEmptyTooltip = "There are no items in this list";
		public const string DefaultAddTooltip = "Add an item to this list";
		public const string DefaultRemoveTooltip = "Remove this item from the list";
		public const string DefaultReorderTooltip = "Move this item within the list";

		public string Label { get; set; }
		public string Tooltip { get; set; }
		public string EmptyLabel { get; set; } = DefaultEmptyLabel;
		public string EmptyTooltip { get; set; } = DefaultEmptyTooltip;
		public string AddTooltip { get; set; } = DefaultAddTooltip;
		public string RemoveTooltip { get; set; } = DefaultRemoveTooltip;
		public string ReorderTooltip { get; set; } = DefaultReorderTooltip;

		public abstract int ItemCount { get; }
		public virtual bool AllowAdd { get; } = true;
		public virtual bool AllowRemove { get; } = true;
		public virtual bool AllowReorder { get; } = true;

		public abstract VisualElement CreateElement(int index);
		public abstract bool NeedsUpdate(VisualElement item, int index);
		public abstract void AddItem();
		public abstract void RemoveItem(int index);
		public abstract void ReorderItem(int from, int to);

		public virtual bool CanAdd() => AllowAdd;
		public virtual bool CanRemove(int index) => AllowRemove;
		public virtual bool CanReorder(int from, int to) => AllowReorder;
	}

	public class ListProxy<T> : ListProxy
	{
		public List<T> Items;

		public override int ItemCount => Items.Count;

		private readonly Func<int, VisualElement> _createElement;

		public ListProxy(List<T> items, Func<int, VisualElement> createElement)
		{
			Items = items;

			_createElement = createElement;
		}

		public override VisualElement CreateElement(int index)
		{
			var element = _createElement?.Invoke(index) ?? new VisualElement();
			element.userData = index;
			return element;
		}

		public override bool NeedsUpdate(VisualElement item, int index)
		{
			return !(item.userData is int i) || i != index;
		}

		public override void AddItem()
		{
			Items.Add(default);
		}

		public override void RemoveItem(int index)
		{
			Items.RemoveAt(index);
		}

		public override void ReorderItem(int from, int to)
		{
			var previous = Items[to];
			Items[to] = Items[from];
			Items[from] = previous;
		}
	}
}