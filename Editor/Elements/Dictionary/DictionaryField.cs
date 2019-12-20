using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class DictionaryField : Frame
	{
		#region Events

		public class ItemAddedEvent : EventBase<ItemAddedEvent>
		{
			public static ItemAddedEvent GetPooled(string key)
			{
				var e = GetPooled();
				e.Key = key;
				return e;
			}

			public string Key { get; private set; }

			public ItemAddedEvent()
			{
				LocalInit();
			}

			protected override void Init()
			{
				base.Init();
				LocalInit();
			}

			void LocalInit()
			{
				Key = string.Empty;
			}
		}

		public class ItemRemovedEvent : EventBase<ItemRemovedEvent>
		{
			public static ItemRemovedEvent GetPooled(string key)
			{
				var e = GetPooled();
				e.Key = key;
				return e;
			}

			public string Key { get; private set; }

			public ItemRemovedEvent()
			{
				LocalInit();
			}

			protected override void Init()
			{
				base.Init();
				LocalInit();
			}

			void LocalInit()
			{
				Key = string.Empty;
			}
		}

		public class ItemReorderedEvent : EventBase<ItemReorderedEvent>
		{
			public static ItemReorderedEvent GetPooled(int from, int to)
			{
				var e = GetPooled();
				e.FromIndex = from;
				e.ToIndex = to;
				return e;
			}

			public int FromIndex { get; private set; }
			public int ToIndex { get; private set; }

			public ItemReorderedEvent()
			{
				LocalInit();
			}

			protected override void Init()
			{
				base.Init();
				LocalInit();
			}

			void LocalInit()
			{
				FromIndex = 0;
				ToIndex = 0;
			}
		}

		public class ItemsChangedEvent : EventBase<ItemsChangedEvent>
		{
		}

		#endregion

		#region Log Messages

		private const string _invalidBindingError = "(PUEDFIB) invalid binding '{0}' for DictionaryField: property '{1}' is type '{2}' but should be an array";
		private const string _invalidTypeError = "(PUEDFIT) invalid item type '{0}' for DictionaryField: the item type must be a default constructable class when used with allowDerived = false";
		private const string _failedAddError = "(PUEDFFA) failed to add item '{0}' of type '{1}' to the DictionaryField: the item type must be a value type or default constructable class that is compatible with the dictionary";
		private const string _unspecifiedType = "(unspecified)";

		#endregion

		#region Class Names

		public new const string Stylesheet = "Dictionary/DictionaryStyle.uss";
		public new const string UssClassName = "pirho-dictionary-field";
		public const string EmptyUssClassName = UssClassName + "--empty";
		public const string AddDisabledUssClassName = UssClassName + "--add-disabled";
		public const string RemoveDisabledUssClassName = UssClassName + "--remove-disabled";
		public const string ReorderDisabledUssClassName = UssClassName + "--reorder-disabled";
		public const string AddKeyValidUssClassName = UssClassName + "--add-key-valid";
		public const string AddKeyInvalidUssClassName = UssClassName + "--add-key-invalid";
		public const string EmptyLabelUssClassName = UssClassName + "__empty-label";
		public const string ItemsUssClassName = UssClassName + "__items";
		public const string HeaderKeyTextUssClassName = UssClassName + "__key-text";
		public const string AddButtonUssClassName = UssClassName + "__add-button";
		public const string RemoveButtonUssClassName = UssClassName + "__remove-button";
		public const string DragHandleUssClassName = UssClassName + "__drag-handle";
		public const string DragPlaceholderUssClassName = UssClassName + "__drag-placeholder";
		public const string ItemUssClassName = UssClassName + "__item";
		public const string ItemDraggingUssClassName = ItemUssClassName + "--dragging";
		public const string ItemEvenUssClassName = ItemUssClassName + "--even";
		public const string ItemOddUssClassName = ItemUssClassName + "--odd";
		public const string ItemContentUssClassName = ItemUssClassName + "__content";

		#endregion

		#region Defaults

		public const string DefaultEmptyLabel = "The dictionary is empty";
		public const string DefaultEmptyTooltip = "There are no items in this dictionary";
		public const string DefaultAddPlaceholder = "New key";
		public const string DefaultAddTooltip = "Add an item to this dictionary";
		public const string DefaultRemoveTooltip = "Remove this item from the dictionary";
		public const string DefaultReorderTooltip = "Move this item within the dictionary";

		public const bool DefaultAllowAdd = true;
		public const bool DefaultAllowRemove = true;
		public const bool DefaultAllowReorder = true;

		#endregion

		#region Icons

		private static readonly Icon _addIcon = Icon.Add;
		private static readonly Icon _removeIcon = Icon.Remove;
		private static readonly Icon _dragIcon = Icon.BuiltIn("animationnocurve");

		#endregion

		#region Members

		private string _emptyLabel = DefaultEmptyLabel;
		private string _emptyTooltip = DefaultEmptyTooltip;
		private string _addTooltip = DefaultAddTooltip;
		private string _addPlaceholder = DefaultAddPlaceholder;
		private string _removeTooltip = DefaultRemoveTooltip;
		private string _reorderTooltip = DefaultReorderTooltip;

		private bool _allowAdd = DefaultAllowAdd;
		private bool _allowRemove = DefaultAllowRemove;
		private bool _allowReorder = DefaultAllowReorder;

		private IDictionaryProxy _proxy;

		private class TypeProvider : PickerProvider<Type> { }
		private TypeProvider _typeProvider;
		private Type _itemType;
		private bool _allowDerived = false;

		private TextField _addField;
		private Placeholder _addPlaceholderText;
		private IconButton _addButton;
		private UQueryState<IconButton> _removeButtons;
		private UQueryState<Image> _reorderHandles;
		private TextElement _emptyText;
		private VisualElement _itemsContainer;

		private int _dragFromIndex = -1;
		private int _dragToIndex = -1;
		private VisualElement _dragElement;
		private VisualElement _dragPlaceholder;

		#endregion

		#region Public Interface

		public DictionaryField() : base(false)
		{
			BuildUi();
		}

		public bool AllowAdd
		{
			get => _allowAdd;
			set { _allowAdd = value; UpdateAddState(); }
		}

		public bool AllowRemove
		{
			get => _allowRemove;
			set { _allowRemove = value; UpdateRemoveState(); }
		}

		public bool AllowReorder
		{
			get => _allowReorder;
			set { _allowReorder = value; UpdateReorderState(); }
		}

		public string EmptyLabel
		{
			get => _emptyLabel;
			set { _emptyLabel = value; UpdateEmptyLabel(); }
		}

		public string EmptyTooltip
		{
			get => _emptyTooltip;
			set { _emptyTooltip = value; UpdateEmptyLabel(); }
		}

		public string AddTooltip
		{
			get => _addTooltip;
			set { _addTooltip = value; UpdateAddLabel(); }
		}

		public string AddPlaceholder
		{
			get => _addPlaceholder;
			set { _addPlaceholder = value; UpdateAddPlaceholder(); }
		}

		public string RemoveTooltip
		{
			get => _removeTooltip;
			set { _removeTooltip = value; UpdateRemoveLabels(); }
		}

		public string ReorderTooltip
		{
			get => _reorderTooltip;
			set { _reorderTooltip = value; UpdateReorderLabels(); }
		}

		public IDictionaryProxy Proxy => _proxy;
		public Type ItemType => _itemType;
		public bool AllowDerived => _allowDerived;

		public void SetProxy(IDictionaryProxy proxy, Type itemType, bool allowDerived)
		{
			if (itemType != null && !allowDerived && !itemType.IsCreatable())
			{
				Debug.LogWarningFormat(_invalidTypeError, itemType.FullName);
				return;
			}

			_proxy = proxy;
			_itemType = itemType;
			_allowDerived = allowDerived && itemType != null;

			UpdateProxy();
			UpdateReorderState();
			UpdateItemType();
		}

		public void Rebuild()
		{
			UpdateItemsWithoutNotify();
		}

		#endregion

		#region Ui

		private void BuildUi()
		{
			AddToClassList(UssClassName);
			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);

			_addField = new TextField();
			_addField.AddToClassList(HeaderKeyTextUssClassName);
			_addField.RegisterValueChangedCallback(evt => AddKeyChanged(evt.newValue));
			_addField.Q(TextField.textInputUssName).RegisterCallback<KeyDownEvent>(evt => AddKeyPressed(evt));

			_addPlaceholderText = new Placeholder(_addPlaceholder);
			_addPlaceholderText.AddToField(_addField);

			Header.Add(_addField);
			_addField.PlaceBehind(HeaderButtons);

			_addButton = AddHeaderButton(_addIcon.Texture, _addTooltip, AddButtonUssClassName, DoAdd);
			_removeButtons = Content.Query<IconButton>(className: RemoveButtonUssClassName).Build();
			_reorderHandles = Content.Query<Image>(className: DragHandleUssClassName).Build();

			_emptyText = new TextElement();
			_emptyText.AddToClassList(EmptyLabelUssClassName);

			_itemsContainer = new VisualElement();
			_itemsContainer.AddToClassList(ItemsUssClassName);

			Content.Add(_emptyText);
			Content.Add(_itemsContainer);

			_dragPlaceholder = new VisualElement();
			_dragPlaceholder.AddToClassList(DragPlaceholderUssClassName);

			_typeProvider = ScriptableObject.CreateInstance<TypeProvider>();

			UpdateAddState();
			UpdateRemoveState();
			UpdateReorderState();

			UpdateEmptyLabel();
			UpdateAddLabel();
			UpdateAddPlaceholder();
			UpdateRemoveLabels();
			UpdateReorderLabels();

			UpdateProxy();
			UpdateItemType();
		}

		private void UpdateAddState()
		{
			EnableInClassList(AddDisabledUssClassName, !_allowAdd);
		}

		private void UpdateRemoveState()
		{
			EnableInClassList(RemoveDisabledUssClassName, !_allowRemove);
		}

		private void UpdateReorderState()
		{
			var allow = _allowReorder && (_proxy != null && _proxy.IsReorderable);

			EnableInClassList(ReorderDisabledUssClassName, !allow);

			UnregisterCallback<MouseMoveEvent>(UpdateDrag);
			UnregisterCallback<MouseUpEvent>(StopDrag);

			if (allow)
			{
				RegisterCallback<MouseMoveEvent>(UpdateDrag);
				RegisterCallback<MouseUpEvent>(StopDrag);
			}
		}

		private void UpdateEmptyLabel()
		{
			_emptyText.text = _emptyLabel;
			_emptyText.tooltip = _emptyTooltip;
		}

		private void UpdateAddLabel()
		{
			_addButton.tooltip = _addTooltip;
		}

		private void UpdateAddPlaceholder()
		{
			_addPlaceholderText.text = _addPlaceholder;
		}

		private void UpdateRemoveLabels()
		{
			_removeButtons.ForEach(button => button.tooltip = _removeTooltip);
		}

		private void UpdateReorderLabels()
		{
			_reorderHandles.ForEach(button => button.tooltip = _reorderTooltip);
		}

		private void UpdateProxy()
		{
			_itemsContainer.Clear();

			if (_proxy != null)
				UpdateItemsWithoutNotify();
			else
				EnableInClassList(EmptyUssClassName, true);
		}

		private void UpdateItemType()
		{
			if (_itemType != null)
			{
				var types = TypeHelper.GetTypeList(_itemType, false);
				_typeProvider.Setup(_itemType.Name, types.Paths, types.Types, GetIcon, AddItem);
			}
		}

		private void UpdateItems()
		{
			UpdateItemsWithoutNotify();

			using (var e = ItemsChangedEvent.GetPooled())
			{
				e.target = this;
				SendEvent(e);
			}
		}

		private void UpdateItemsWithoutNotify()
		{
			EnableInClassList(EmptyUssClassName, _proxy.Count == 0);

			while (_itemsContainer.childCount > _proxy.Count)
				_itemsContainer.RemoveAt(_itemsContainer.childCount - 1);

			for (var i = 0; i < _proxy.Count; i++)
			{
				var key = _proxy.GetKey(i);

				if (i < _itemsContainer.childCount)
					CheckElement(i, key);
				else
					CreateElement(i, key);
			}

			AddKeyChanged(_addField.text);

			_removeButtons.ForEach(button =>
			{
				var key = GetKey(button.parent);
				var index = GetIndex(button.parent);
				var removable = _proxy.CanRemove(index, key);

				button.SetEnabled(removable);
			});
		}

		private void CreateElement(int index, string key)
		{
			var item = new VisualElement();
			item.AddToClassList(ItemUssClassName);
			_itemsContainer.Add(item);

			var dragHandle = new Image { image = _dragIcon.Texture, tooltip = _reorderTooltip };
			dragHandle.AddToClassList(DragHandleUssClassName);
			dragHandle.RegisterCallback((MouseDownEvent e) => StartDrag(e, item));
			item.Add(dragHandle);

			var remove = new IconButton(_removeIcon.Texture, _removeTooltip, () => RemoveItem(item));
			remove.AddToClassList(RemoveButtonUssClassName);
			item.Add(remove);

			UpdateContent(item, index, key);
		}

		private void CheckElement(int index, string key)
		{
			var item = _itemsContainer[index];
			var current = GetKey(item);

			if (key != current)
			{
				item.RemoveAt(1);
				UpdateContent(item, index, key);
			}
		}

		private void UpdateContent(VisualElement item, int index, string key)
		{
			SetKey(item, key);
			item.EnableInClassList(ItemEvenUssClassName, index % 2 == 0);
			item.EnableInClassList(ItemOddUssClassName, index % 2 != 0);

			var content = _proxy.CreateElement(index, key);
			content.AddToClassList(ItemContentUssClassName);
			item.Insert(1, content);
		}

		#endregion

		#region Item Management

		private int GetIndex(VisualElement element)
		{
			return element.parent.IndexOf(element);
		}

		private string GetKey(VisualElement element)
		{
			return element.userData is string s ? s : null;
		}

		private void SetKey(VisualElement element, string key)
		{
			element.userData = key;
		}

		private void AddKeyChanged(string newValue)
		{
			// Separately check for empty so it shows up as valid even if it's not.

			var empty = string.IsNullOrEmpty(newValue);
			var validKey = _proxy != null && _proxy.CanAdd(newValue);
			var validType = _proxy != null && (_allowDerived || _proxy.CanAdd(_itemType));

			EnableInClassList(AddKeyInvalidUssClassName, !empty && !validKey);
			EnableInClassList(AddKeyValidUssClassName, !empty && validKey);

			_addButton.SetEnabled(validKey && validType);
		}

		private void AddKeyPressed(KeyDownEvent evt)
		{
			if (evt.keyCode == KeyCode.KeypadEnter || evt.keyCode == KeyCode.Return)
			{
				DoAdd();
				evt.StopPropagation();
				evt.PreventDefault();
			}
		}

		private void DoAdd()
		{
			if (_allowDerived)
				SelectType();
			else
				AddItem(_itemType);
		}

		private void SelectType()
		{
			var key = _addField.text;

			if (_allowAdd && _proxy.CanAdd(key))
			{
				var position = new Vector2(_addButton.worldBound.center.x, _addButton.worldBound.yMax + _addButton.worldBound.height * 0.5f);
				SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(position)), _typeProvider);
			}
		}

		private Texture GetIcon(Type type)
		{
			return AssetPreview.GetMiniTypeThumbnail(type);
		}

		private void AddItem(Type selected)
		{
			var key = _addField.text;

			if (_allowAdd && _proxy.CanAdd(key) && _proxy.CanAdd(selected))
			{
				if (!_proxy.AddItem(key, selected))
				{
					Debug.LogErrorFormat(_failedAddError, key, selected != null ? selected.FullName : _unspecifiedType);
					return;
				}

				_addField.value = string.Empty;
				UpdateItemsWithoutNotify();

				using (var e = ItemAddedEvent.GetPooled(key))
				{
					e.target = this;
					SendEvent(e);
				}
			}
		}

		private void RemoveItem(VisualElement item)
		{
			var key = GetKey(item);
			var index = GetIndex(item);

			if (_allowRemove && _proxy.CanRemove(index, key))
			{
				Proxy.RemoveItem(index, key);
				UpdateItemsWithoutNotify();

				using (var e = ItemRemovedEvent.GetPooled(key))
				{
					e.target = this;
					SendEvent(e);
				}
			}
		}

		private void ReorderItem(int from, int to)
		{
			Proxy.ReorderItem(from, to);
			UpdateItemsWithoutNotify();

			using (var e = ItemReorderedEvent.GetPooled(from, to))
			{
				e.target = this;
				SendEvent(e);
			}
		}

		#endregion

		#region Dragging

		private void StartDrag(MouseDownEvent e, VisualElement item)
		{
			if (_allowReorder && e.button == (int)MouseButton.LeftMouse)
			{
				var index = GetIndex(item);
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

		#region Binding

		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);

			if (this.TryGetPropertyBindEvent(evt, out var property))
			{
				var arrayProperty = property.FindPropertyRelative("Array.size");

				if (arrayProperty != null)
				{
					var sizeBinding = new ChangeTrigger<int>(null, (_, oldSize, newSize) => UpdateItems());
					sizeBinding.Watch(arrayProperty);

					Add(sizeBinding);
				}
				else
				{
					Debug.LogErrorFormat(_invalidBindingError, bindingPath, property.propertyPath, property.propertyType);
				}
			}
		}

		#endregion

		#region UXML Support

		public new class UxmlFactory : UxmlFactory<DictionaryField, UxmlTraits> { }
		public new class UxmlTraits : Frame.UxmlTraits
		{
			private readonly UxmlBoolAttributeDescription _allowAdd = new UxmlBoolAttributeDescription { name = "allow-add", defaultValue = DefaultAllowAdd };
			private readonly UxmlBoolAttributeDescription _allowRemove = new UxmlBoolAttributeDescription { name = "allow-remove", defaultValue = DefaultAllowRemove };
			private readonly UxmlBoolAttributeDescription _allowReorder = new UxmlBoolAttributeDescription { name = "allow-reorder", defaultValue = DefaultAllowReorder };
			private readonly UxmlStringAttributeDescription _emptyLabel = new UxmlStringAttributeDescription { name = "empty-label", defaultValue = DefaultEmptyLabel };
			private readonly UxmlStringAttributeDescription _emptyTooltip = new UxmlStringAttributeDescription { name = "empty-tooltip", defaultValue = DefaultEmptyTooltip };
			private readonly UxmlStringAttributeDescription _addTooltip = new UxmlStringAttributeDescription { name = "add-tooltip", defaultValue = DefaultAddTooltip };
			private readonly UxmlStringAttributeDescription _addPlaceholder = new UxmlStringAttributeDescription { name = "add-placeholder", defaultValue = DefaultAddPlaceholder };
			private readonly UxmlStringAttributeDescription _removeTooltip = new UxmlStringAttributeDescription { name = "remove-tooltip", defaultValue = DefaultRemoveTooltip };
			private readonly UxmlStringAttributeDescription _reorderTooltip = new UxmlStringAttributeDescription { name = "reorder-tooltip", defaultValue = DefaultReorderTooltip };

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);

				var dictionary = ve as DictionaryField;

				dictionary.AllowAdd = _allowAdd.GetValueFromBag(bag, cc);
				dictionary.AllowRemove = _allowRemove.GetValueFromBag(bag, cc);
				dictionary.AllowReorder = _allowReorder.GetValueFromBag(bag, cc);
				dictionary.EmptyLabel = _emptyLabel.GetValueFromBag(bag, cc);
				dictionary.EmptyTooltip = _emptyTooltip.GetValueFromBag(bag, cc);
				dictionary.AddTooltip = _addTooltip.GetValueFromBag(bag, cc);
				dictionary.AddPlaceholder = _addPlaceholder.GetValueFromBag(bag, cc);
				dictionary.RemoveTooltip = _removeTooltip.GetValueFromBag(bag, cc);
				dictionary.ReorderTooltip = _reorderTooltip.GetValueFromBag(bag, cc);
			}
		}

		#endregion
	}
}
