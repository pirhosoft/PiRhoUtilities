using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[InitializeOnLoad]
	public class History : EditorWindow
	{
		public const string Stylesheet = "History/HistoryStyle.uss";
		public const string UssClassName = "pirho-history";
		public const string HeaderUssClassName = UssClassName + "__header";
		public const string HeaderButtonUssClassName = HeaderUssClassName + "__button";
		public const string ListUssClassName = UssClassName + "__list";
		public const string ListItemUssClassName = ListUssClassName + "__item";
		public const string ListItemLabelUssClassName = ListItemUssClassName + "__label";
		public const string CurrentListItemUssClassName = ListItemUssClassName + "--current";

		private Button _back;
		private Button _forward;
		private ListView _listView;

		void OnEnable()
		{
			rootVisualElement.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
			rootVisualElement.AddToClassList(UssClassName);

			var header = new VisualElement();
			header.AddToClassList(HeaderUssClassName);

			_back = new Button(HistoryList.MoveBack) { text = "Back" };
			_back.AddToClassList(HeaderButtonUssClassName);

			_forward = new Button(HistoryList.MoveForward) { text = "Forward" };
			_forward.AddToClassList(HeaderButtonUssClassName);

			_listView = new ListView();
			_listView.AddToClassList(ListUssClassName);
			_listView.selectionType = SelectionType.Single;
			_listView.itemsSource = HistoryList.History;
			_listView.makeItem = MakeItem;
			_listView.bindItem = BindItem;
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
			_back.SetEnabled(HistoryList.CanMoveBack());
			_forward.SetEnabled(HistoryList.CanMoveForward());
			_listView.Refresh();
		}

		private VisualElement MakeItem()
		{
			var item = new VisualElement();
			item.AddToClassList(ListItemUssClassName);

			var label = new Label();
			label.AddToClassList(ListItemLabelUssClassName);

			item.Add(label);
			return item;
		}

		private void BindItem(VisualElement container, int index)
		{
			var label = container.ElementAt(0) as Label;
			label.text = HistoryList.GetName(index);

			if (index == HistoryList.Current)
				label.AddToClassList(CurrentListItemUssClassName);
			else
				label.RemoveFromClassList(CurrentListItemUssClassName);
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

		private static class HistoryList
		{
			private const string _windowMenu = "Window/PiRho Utilities/History";
			private const string _moveBackMenu = "Edit/History/Move Back &LEFT";
			private const string _moveForwardMenu = "Edit/History/Move Forward &RIGHT";

			private const int _capacity = 100;

			private static readonly Icon _icon = Icon.BuiltIn("VerticalLayoutGroup Icon");

			private static bool _skipNextSelection = false;

			public static int Current { get; private set; }
			public static List<Object[]> History { get; private set; } = new List<Object[]>();

			static HistoryList()
			{
				Selection.selectionChanged += SelectionChanged;
				EditorApplication.playModeStateChanged += OnPlayModeChanged;
			}

			[MenuItem(_windowMenu)]
			private static void Open()
			{
				var window = GetWindow<History>();
				window.titleContent = new GUIContent("History", _icon.Texture);
				window.Show();
			}

			[MenuItem(_moveBackMenu, validate = true)]
			public static bool CanMoveBack()
			{
				return Current > 0;
			}

			[MenuItem(_moveForwardMenu, validate = true)]
			public static bool CanMoveForward()
			{
				return Current < History.Count - 1;
			}

			[MenuItem(_moveBackMenu, priority = 1)]
			public static void MoveBack()
			{
				if (CanMoveBack())
					Select(--Current);
			}

			[MenuItem(_moveForwardMenu, priority = 2)]
			public static void MoveForward()
			{
				if (CanMoveForward())
					Select(++Current);
			}

			public static void Select(int index)
			{
				Current = index;
				_skipNextSelection = true;
				Selection.objects = History[index];
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
	}
}