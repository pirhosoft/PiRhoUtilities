using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class DictionaryControl : RolloutControl
	{
		public new const string Stylesheet = "Dictionary/DictionaryStyle.uss";

		#region Class Names

		public new static readonly string UssClassName = "pirho-dictionary";
		public static readonly string EmptyUssClassName = UssClassName + "--empty";
		public static readonly string AddDisabledUssClassName = UssClassName + "--add-disabled";
		public static readonly string RemoveDisabledUssClassName = UssClassName + "--remove-disabled";
		public static readonly string MoveDisabledUssClassName = UssClassName + "--move-disabled";
		public static readonly string AddKeyInvalidUssClassName = UssClassName + "--add-key-invalid";
		public static readonly string EmptyLabelUssClassName = UssClassName + "__empty-label";
		public static readonly string ItemsUssClassName = UssClassName + "__items";
		public static readonly string HeaderKeyTextUssClassName = UssClassName + "__key-text";
		public static readonly string AddButtonUssClassName = UssClassName + "__add-button";
		public static readonly string RemoveButtonUssClassName = UssClassName + "__remove-button";
		public static readonly string DragHandleUssClassName = UssClassName + "__drag-handle";
		public static readonly string DragPlaceholderUssClassName = UssClassName + "__drag-placeholder";
		public static readonly string ItemUssClassName = UssClassName + "__item";
		public static readonly string ItemDraggingUssClassName = ItemUssClassName + "--dragging";
		public static readonly string ItemEvenUssClassName = ItemUssClassName + "--even";
		public static readonly string ItemOddUssClassName = ItemUssClassName + "--odd";
		public static readonly string ItemContentUssClassName = ItemUssClassName + "__content";

		#endregion

		#region Icons

		private static readonly Icon _addIcon = Icon.Add;
		private static readonly Icon _removeIcon = Icon.Remove;
		private static readonly Icon _dragIcon = Icon.BuiltIn("animationnocurve");

		#endregion

		public IDictionaryProxy Proxy { get; private set; }

		private VisualElement _itemsContainer;
		private TextField _keyField;
		private IconButton _addButton;
		private UQueryState<IconButton> _removeButtons;

		private int _dragFromIndex = -1;
		private int _dragToIndex = -1;
		private VisualElement _dragElement;
		private VisualElement _dragPlaceholder;

		public DictionaryControl(IDictionaryProxy proxy) : base(true)
		{
			Proxy = proxy;

			CreateFrame();
			SetupDragging();
			Refresh();

			AddToClassList(UssClassName);
			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
		}

		public void Refresh()
		{
			while (_itemsContainer.childCount > Proxy.KeyCount)
				_itemsContainer.RemoveAt(_itemsContainer.childCount - 1);

			for (var i = 0; i < _itemsContainer.childCount; i++)
				UpdateItem(i);

			for (var i = _itemsContainer.childCount; i < Proxy.KeyCount; i++)
				CreateItem(i);

			EnableInClassList(EmptyUssClassName, Proxy.KeyCount == 0);
			EnableInClassList(AddDisabledUssClassName, !Proxy.AllowAdd);
			EnableInClassList(RemoveDisabledUssClassName, !Proxy.AllowRemove);
			EnableInClassList(MoveDisabledUssClassName, !Proxy.AllowReorder);

			_removeButtons.ForEach(button =>
			{
				var index = GetItemIndex(button.parent);
				var removable = Proxy.CanRemove(index);

				button.SetEnabled(removable);
			});
		}

		#region Element Creation

		private void CreateFrame()
		{
			SetLabel(Proxy.Label, Proxy.Tooltip);

			_addButton = AddHeaderButton(_addIcon.Texture, Proxy.AddTooltip, AddButtonUssClassName, AddItem);
			_addButton.SetEnabled(false);
			_removeButtons = Content.Query<IconButton>(className: RemoveButtonUssClassName).Build();

			_keyField = new TextField();
			_keyField.AddToClassList(HeaderKeyTextUssClassName);
			_keyField.RegisterValueChangedCallback(evt => AddKeyChanged(evt.newValue));
			_keyField.Q(TextField.textInputUssName).RegisterCallback<KeyDownEvent>(evt => KeyPressed(evt));

			var keyPlaceholder = new PlaceholderControl(Proxy.AddPlaceholder);
			keyPlaceholder.AddToField(_keyField);

			Header.Add(_keyField);
			_keyField.PlaceInFront(Label);

			var empty = new TextElement { text = Proxy.EmptyLabel, tooltip = Proxy.EmptyTooltip };
			empty.AddToClassList(EmptyLabelUssClassName);

			Content.Add(empty);

			_itemsContainer = new VisualElement();
			_itemsContainer.AddToClassList(ItemsUssClassName);

			Content.Add(_itemsContainer);
		}

		private void CreateItem(int index)
		{
			var item = new VisualElement();
			item.AddToClassList(ItemUssClassName);
			item.AddToClassList(index % 2 == 0 ? ItemEvenUssClassName : ItemOddUssClassName);
			_itemsContainer.Add(item);

			var dragHandle = new Image { image = _dragIcon.Texture, tooltip = Proxy.ReorderTooltip };
			dragHandle.AddToClassList(DragHandleUssClassName);
			dragHandle.RegisterCallback((MouseDownEvent e) => StartDrag(e, GetItemIndex(item)));
			item.Add(dragHandle);

			var content = Proxy.CreateField(index);
			content.AddToClassList(ItemContentUssClassName);
			item.Add(content);

			var remove = new IconButton(_removeIcon.Texture, Proxy.RemoveTooltip, () => RemoveItem(index));
			remove.AddToClassList(RemoveButtonUssClassName);
			item.Add(remove);
		}

		private void UpdateItem(int index)
		{
			var item = _itemsContainer.ElementAt(index);
			item.EnableInClassList(ItemEvenUssClassName, index % 2 == 0);
			item.EnableInClassList(ItemOddUssClassName, index % 2 != 0);

			if (Proxy.NeedsUpdate(item, index))
			{
				var content = Proxy.CreateField(index);
				content.AddToClassList(ItemContentUssClassName);
				item.RemoveAt(1);
				item.Insert(1, content);
			}
		}

		private void KeyPressed(KeyDownEvent evt)
		{
			if (evt.keyCode == KeyCode.KeypadEnter || evt.keyCode == KeyCode.Return)
			{
				AddItem();
				evt.StopPropagation();
				evt.PreventDefault();
			}
		}

		private void AddKeyChanged(string newValue)
		{
			// Separately check for empty because we don't want empty to be addable but we also don't want it be show as invalid
			var valid = IsKeyValid(newValue) || string.IsNullOrEmpty(newValue);
			EnableInClassList(AddKeyInvalidUssClassName, !valid);
			_addButton.SetEnabled(valid && !string.IsNullOrEmpty(newValue));
		}

		private bool IsKeyValid(string key)
		{
			return !string.IsNullOrEmpty(key) && Proxy.CanAdd(key);
		}

		#endregion

		#region Item Management

		private void AddItem()
		{
			var key = _keyField.text;

			if (Proxy.AllowAdd && IsKeyValid(key))
			{
				Proxy.AddItem(key);
				_keyField.value = string.Empty;
				Refresh();
			}
		}

		private void RemoveItem(int index)
		{
			if (Proxy.AllowRemove && Proxy.CanRemove(index))
			{
				Proxy.RemoveItem(index);
				Refresh();
			}
		}

		private void ReorderItem(int from, int to)
		{
			if (Proxy.AllowReorder && Proxy.CanReorder(from, to))
			{
				var item = _itemsContainer.ElementAt(from);
				_itemsContainer.RemoveAt(from);
				_itemsContainer.Insert(to, item);

				Proxy.ReorderItem(from, to);
				Refresh();
			}
		}
		#endregion

		#region Dragging

		private int GetItemIndex(VisualElement item)
		{
			return item.parent.IndexOf(item);
		}

		private void SetupDragging()
		{
			RegisterCallback<MouseMoveEvent>(UpdateDrag);
			RegisterCallback<MouseUpEvent>(StopDrag);

			_dragPlaceholder = new VisualElement();
			_dragPlaceholder.AddToClassList(DragPlaceholderUssClassName);
		}

		private void StartDrag(MouseDownEvent e, int index)
		{
			if (e.button == (int)MouseButton.LeftMouse)
			{
				var mousePosition = _itemsContainer.WorldToLocal(e.mousePosition);

				_dragFromIndex = index;
				_dragToIndex = index;

				_dragElement = _itemsContainer.ElementAt(index);
				_dragElement.AddToClassList(ItemDraggingUssClassName);
				_dragElement.BringToFront();
				_dragElement.style.left = mousePosition.x;
				_dragElement.style.top = mousePosition.y;

				_itemsContainer.Insert(index, _dragPlaceholder);

				this.CaptureMouse();
			}
		}

		private void UpdateDrag(MouseMoveEvent e)
		{
			if (e.button == (int)MouseButton.LeftMouse)
			{
				if (_dragElement != null)
				{
					var mousePosition = _itemsContainer.WorldToLocal(e.mousePosition);

					_dragElement.style.left = mousePosition.x;
					_dragElement.style.top = mousePosition.y;

					var nextIndex = -1;
					VisualElement nextElement = null;

					for (var i = 0; i < _itemsContainer.childCount - 1; i++)
					{
						if (mousePosition.y < _itemsContainer.ElementAt(i).localBound.center.y)
						{
							nextIndex = i;
							nextElement = _itemsContainer.ElementAt(i);
							break;
						}
					}

					if (nextElement != null)
					{
						_dragToIndex = nextIndex > _dragToIndex ? nextIndex - 1 : nextIndex;
						_dragPlaceholder.PlaceBehind(nextElement);
					}
					else
					{
						_dragToIndex = _itemsContainer.childCount - 2; // Subtract 2 because _dragPlaceholder counts as a child
						_dragPlaceholder.PlaceBehind(_dragElement);
					}
				}
			}
		}

		private void StopDrag(MouseUpEvent e)
		{
			if (e.button == (int)MouseButton.LeftMouse)
			{
				this.ReleaseMouse();

				if (_dragElement != null)
				{
					_dragElement.style.left = 0;
					_dragElement.style.top = 0;
					_dragElement.PlaceBehind(_dragPlaceholder);
					_dragElement.RemoveFromClassList(ItemDraggingUssClassName);
				}

				_dragPlaceholder.RemoveFromHierarchy();

				if (_dragFromIndex != _dragToIndex)
					ReorderItem(_dragFromIndex, _dragToIndex);

				_dragElement = null;
				_dragFromIndex = -1;
				_dragToIndex = -1;
			}
		}

		#endregion
	}
}