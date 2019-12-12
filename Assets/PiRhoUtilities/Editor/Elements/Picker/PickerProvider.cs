using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace PiRhoSoft.Utilities.Editor
{
	public class PickerProvider<ValueType> : ScriptableObject, ISearchWindowProvider where ValueType : class
	{
		private string _title;
		private List<string> _paths;
		private List<ValueType> _items;
		private Func<ValueType, Texture> _getIcon;
		private Action<ValueType> _onSelected;
		private Texture2D _emptyIcon;

		private void OnEnable()
		{
			_emptyIcon = new Texture2D(1, 1);
			_emptyIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
			_emptyIcon.Apply();
		}

		private void OnDisable()
		{
			DestroyImmediate(_emptyIcon);
		}

		public void Setup(string title, List<string> paths, List<ValueType> items, Func<ValueType, Texture> getIcon, Action<ValueType> onSelected)
		{
			_title = title;
			_paths = paths;
			_items = items;
			_getIcon = getIcon;
			_onSelected = onSelected;
		}

		public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
		{
			var tree = new List<SearchTreeEntry> { new SearchTreeGroupEntry(new GUIContent(_title), 0) };

			if (_paths != null && _items != null)
			{
				var groups = new List<string>();

				for (var i = 0; i < _paths.Count && i < _items.Count; i++)
				{
					var path = _paths[i];
					var item = _items[i];
					var icon = _getIcon(item);

					var menus = path.Split('/');
					var createIndex = -1;

					for (var index = 0; index < menus.Length - 1; index++)
					{
						var group = menus[index];
						if (index >= groups.Count)
						{
							createIndex = index;
							break;
						}

						if (groups[index] != group)
						{
							groups.RemoveRange(index, groups.Count - index);
							createIndex = index;
							break;
						}
					}

					if (createIndex >= 0)
					{
						for (var index = createIndex; index < menus.Length - 1; index++)
						{
							var group = menus[index];
							groups.Add(group);
							tree.Add(new SearchTreeGroupEntry(new GUIContent(group)) { level = index + 1 });
						}
					}

					tree.Add(new SearchTreeEntry(new GUIContent(menus.Last(), icon == null ? _emptyIcon : icon)) { level = menus.Length, userData = item });
				}
			}

			return tree;
		}

		public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
		{
			_onSelected?.Invoke(searchTreeEntry.userData as ValueType);
			return true;
		}
	}
}
