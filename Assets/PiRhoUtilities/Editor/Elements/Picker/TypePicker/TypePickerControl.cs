using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.Utilities.Editor
{
	public class TypePickerControl : BasePickerControl<string>
	{
		private class TypeProvider : PickerProvider<Type> { }

		public new const string Stylesheet = "Picker/TypePicker/TypePickerStyle.uss";
		public new const string UssClassName = "pirho-type-picker";

		private const string _invalidTypeWarning = "(PUTPCIT) Invalid type for TypePicker: the type '{0}' could not be found";

		public Type Type { get; private set; }
		public bool ShowAbstract { get; private set; }

		public TypePickerControl(string value, Type type, bool showAbstract)
		{
			if (type == null)
			{
				Debug.LogWarningFormat(_invalidTypeWarning, type);
				return;
			}

			Type = type;
			ShowAbstract = showAbstract;

			var types = TypeHelper.GetTypeList(Type, showAbstract);
			var provider = ScriptableObject.CreateInstance<TypeProvider>();
			provider.Setup(types.BaseType.Name, types.Paths.Prepend("None").ToList(), types.Types.Prepend(null).ToList(), GetIcon, OnSelected);

			Setup(provider, value);

			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
			AddToClassList(UssClassName);
		}

		private Texture GetIcon(Type type)
		{
			return AssetPreview.GetMiniTypeThumbnail(type);
		}

		private void OnSelected(Type selected)
		{
			var previous = Value;
			SetValueWithoutNotify(selected?.AssemblyQualifiedName);
			this.SendChangeEvent(previous, Value);
		}

		protected override void Refresh()
		{
			var type = Type.GetType(Value ?? string.Empty);
			var text = type == null ? $"None ({Type.Name})" : type.Name;
			var icon = GetIcon(type);

			SetLabel(icon, text);
		}
	}
}
