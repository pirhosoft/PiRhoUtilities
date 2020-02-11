using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[InitializeOnLoad]
	public class HistoryWindow : EditorWindow
	{
		#region Class Names

		public const string Stylesheet = "HistoryStyle.uss";
		public const string UssClassName = "pirho-history";
		public const string HeaderUssClassName = UssClassName + "__header";
		public const string HeaderButtonUssClassName = HeaderUssClassName + "__button";
		public const string ListUssClassName = UssClassName + "__list";
		public const string ListItemUssClassName = ListUssClassName + "__item";
		public const string ListItemIconUssClassName = ListItemUssClassName + "__icon";
		public const string ListItemLabelUssClassName = ListItemUssClassName + "__label";
		public const string CurrentListItemUssClassName = ListItemUssClassName + "--current";

		#endregion

		#region Window Names

		private const string _windowMenu = "Window/PiRho Utilities/History";
		private const string _moveBackMenu = "Edit/History/Move Back &LEFT";
		private const string _moveForwardMenu = "Edit/History/Move Forward &RIGHT";

		#endregion

		#region Icons

		private static readonly Icon _icon = Icon.BuiltIn("VerticalLayoutGroup Icon");

		#endregion

		#region Members

		private Button _back;
		private Button _forward;
		private ListView _listView;

		#endregion

		#region Window Management

		[MenuItem(_windowMenu)]
		public static void Open()
		{
			var window = GetWindow<HistoryWindow>();
			window.titleContent = new GUIContent("History", _icon.Texture);
			window.Show();
		}

		[MenuItem(_moveBackMenu, validate = true)]
		public static bool CanMoveBack()
		{
			return HistoryList.Current > 0;
		}

		[MenuItem(_moveForwardMenu, validate = true)]
		public static bool CanMoveForward()
		{
			return HistoryList.Current < HistoryList.History.Count - 1;
		}

		[MenuItem(_moveBackMenu, priority = 1)]
		public static void MoveBack()
		{
			if (CanMoveBack())
				HistoryList.Select(HistoryList.Current - 1);
		}

		[MenuItem(_moveForwardMenu, priority = 2)]
		public static void MoveForward()
		{
			if (CanMoveForward())
				HistoryList.Select(HistoryList.Current + 1);
		}

		#endregion

		#region ListView Management

		private void OnEnable()
		{
			rootVisualElement.AddStyleSheet(Stylesheet);
			rootVisualElement.AddToClassList(UssClassName);

			var header = new VisualElement();
			header.AddToClassList(HeaderUssClassName);

			_back = new Button(MoveBack) { text = "Back" };
			_back.AddToClassList(HeaderButtonUssClassName);

			_forward = new Button(MoveForward) { text = "Forward" };
			_forward.AddToClassList(HeaderButtonUssClassName);

			_listView = new ListView(HistoryList.History, 21, MakeItem, BindItem);
			_listView.AddToClassList(ListUssClassName);
			_listView.selectionType = SelectionType.Single;
			_listView.onItemChosen += item => Select();
			_listView.onSelectionChanged += selection => Highlight();

			header.Add(_back);
			header.Add(_forward);
			rootVisualElement.Add(header);
			rootVisualElement.Add(_listView);

			Refresh();

			Selection.selectionChanged += Refresh;
		}

		private void OnDisable()
		{
			Selection.selectionChanged -= Refresh;
		}

		private void Refresh()
		{
			_back.SetEnabled(CanMoveBack());
			_forward.SetEnabled(CanMoveForward());
			_listView.Refresh();
		}

		private VisualElement MakeItem()
		{
			return new HistoryItem();
		}

		private void BindItem(VisualElement item, int index)
		{
			if (item is HistoryItem history)
				history.Bind(index);
		}

		private void Select()
		{
			if (_listView.selectedIndex != HistoryList.Current)
			{
				HistoryList.Select(_listView.selectedIndex);
				Refresh();
			}
		}

		private void Highlight()
		{
			if (_listView.selectedItem is Object[] obj && obj.Length > 0)
				EditorGUIUtility.PingObject(obj[0]);
		}

		private class HistoryItem : VisualElement, IDraggable
		{
			public DragState DragState { get; set; }
			public string DragText => _label.text;
			public Object[] DragObjects => HistoryList.History[_index];
			public object DragData => DragObjects;

			private readonly Image _icon;
			private readonly Label _label;

			private int _index;

			public HistoryItem()
			{
				_icon = new Image();
				_icon.AddToClassList(ListItemIconUssClassName);

				_label = new Label { pickingMode = PickingMode.Ignore };
				_label.AddToClassList(ListItemLabelUssClassName);
				
				Add(_icon);
				Add(_label);
				AddToClassList(ListItemUssClassName);

				this.MakeDraggable();
			}

			public void Bind(int index)
			{
				_index = index;
				_icon.image = HistoryList.GetIcon(_index);
				_label.text = HistoryList.GetName(_index);
				_label.EnableInClassList(CurrentListItemUssClassName, _index == HistoryList.Current);
			}
		}

		#endregion

		#region HistoryList Management

		private static class HistoryList
		{
			private const int _capacity = 100;

			private static bool _skipNextSelection = false;

			public static int Current { get; private set; }
			public static List<Object[]> History { get; private set; } = new List<Object[]>();

			static HistoryList()
			{
				Selection.selectionChanged += SelectionChanged;
				EditorApplication.playModeStateChanged += OnPlayModeChanged;
			}

			public static void Select(int index)
			{
				Current = index;
				_skipNextSelection = true;
				Selection.objects = History[index];
			}

			public static Texture GetIcon(int index)
			{
				var objects = History[index];

				return GetIcon(objects[0]);
			}

			private static Texture GetIcon(Object obj)
			{
				return obj ? AssetPreview.GetMiniThumbnail(obj) : null;
			}

			public static string GetName(int index)
			{
				var objects = History[index];
				if (objects.Length > 1)
					return string.Join(", ", objects.Select(obj => GetName(obj)));

				return GetName(objects[0]);
			}

			private static string GetName(Object obj)
			{
				if (obj == null)
					return "(missing)";

				if (string.IsNullOrEmpty(obj.name))
					return $"'{obj.GetType().Name}'";

				return obj.name;
			}

			private static void Clear()
			{
				Current = 0;
				History.Clear();
			}

			private static void SelectionChanged()
			{
				if (!_skipNextSelection)
				{
					if (Selection.objects != null && Selection.objects.Length > 0 && Selection.objects[0] is Object obj)
					{
						if (History.Count == 0 || History[Current][0] != obj)
						{
							var trailing = History.Count - Current - 1;

							if (trailing > 0)
								History.RemoveRange(Current + 1, trailing);

							if (Current == _capacity)
								History.RemoveAt(0);

							History.Add(Selection.objects);
							Current = History.Count - 1;
						}
					}
				}
				else
				{
					_skipNextSelection = false;
				}
			}

			private static void OnPlayModeChanged(PlayModeStateChange state)
			{
				switch (state)
				{
					case PlayModeStateChange.ExitingEditMode: Clear(); break;
					case PlayModeStateChange.EnteredEditMode: Clear(); break;
				}
			}
		}

		#endregion
	}
}