using PiRhoSoft.Utilities.Editor;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Samples	
{
	public abstract class CodeSample
	{
		public abstract void Create(VisualElement root);
	}

	public abstract class UxmlSample
	{
		public abstract void Setup(VisualElement uxml);
	}

	public class SamplesWindow : EditorWindow
	{
		private static string RootPath;
		private const string Stylesheet = "SamplesWindow.uss";
		private const string Uxml = "SamplesWindow.uxml";

		private class SampleData
		{
			public string Name { get; private set; }
			public CodeSample Code { get; private set; }
			public UxmlSample Uxml { get; private set; }

			public SampleData(string name, CodeSample code, UxmlSample uxml)
			{
				Name = name;
				Code = code;
				Uxml = uxml;
			}
		}

		private static SampleData[] _items = new SampleData[]
		{
			new SampleData("ChangeTrigger", new ChangeTriggerCodeSample(), new ChangeTriggerUxmlSample()),
			new SampleData("ComboBox", new ComboBoxCodeSample(), new ComboBoxUxmlSample()),
			new SampleData("Dictionary", new DictionaryCodeSample(), new DictionaryUxmlSample()),
			new SampleData("EnumButtons", new EnumButtonsCodeSample(), new EnumButtonsUxmlSample()),
			new SampleData("Euler", new EulerCodeSample(), new EulerUxmlSample()),
			new SampleData("Frame", new FrameCodeSample(), new FrameUxmlSample()),
			new SampleData("IconButton", new IconButtonCodeSample(), new IconButtonUxmlSample()),
			new SampleData("List", new ListCodeSample(), new ListUxmlSample()),
			new SampleData("MessageBox", new MessageBoxCodeSample(), new MessageBoxUxmlSample()),
			new SampleData("ObjectPicker", new ObjectPickerCodeSample(), new ObjectPickerUxmlSample()),
			new SampleData("TypePicker", new TypePickerCodeSample(), new TypePickerUxmlSample()),
			new SampleData("Placeholder", new PlaceholderCodeSample(), new PlaceholderUxmlSample()),
			new SampleData("Popup", new PopupCodeSample(), new PopupUxmlSample()),
			new SampleData("Reference", new ReferenceCodeSample(), new ReferenceUxmlSample()),
			new SampleData("Slider", new SliderCodeSample(), new SliderUxmlSample()),
			new SampleData("Tabs", new TabsCodeSample(), new TabsUxmlSample()),
		};

		[MenuItem("Window/PiRho Utilities/UI Samples")]
		public static void ShowWindow()
		{
			var window = GetWindow<SamplesWindow>("PiRho Utilities UI Samples");
			window.minSize = new Vector2(400, 150);
		}

		private ListView _sampleList;
		private VisualElement _codeView;
		private VisualElement _uxmlView;
		private TextField _codeText;
		private TextField _uxmlText;
		private TextField _uxmlCodeText;

		private void OnEnable()
		{
			if (string.IsNullOrEmpty(RootPath))
				RootPath = AssetHelper.GetScriptPath();

			rootVisualElement.AddUxml(Uxml);
			rootVisualElement.AddStyleSheet(Stylesheet);

			_sampleList = rootVisualElement.Query<ListView>("sample-list");
			_sampleList.makeItem = () => new Label();
			_sampleList.bindItem = (element, index) => (element as Label).text = _items[index].Name;
			_sampleList.onSelectionChanged += SelectionChanged;
			_sampleList.itemsSource = _items;
			_sampleList.selectionType = SelectionType.Single;

			_codeView = rootVisualElement.Query("code-sample");
			_codeText = rootVisualElement.Query<TextField>("code-text");
			_codeText.isReadOnly = true;

			_uxmlView = rootVisualElement.Query("uxml-sample");
			_uxmlText = rootVisualElement.Query<TextField>("uxml-text");
			_uxmlText.isReadOnly = true;
			_uxmlCodeText = rootVisualElement.Query<TextField>("uxml-code-text");
			_uxmlCodeText.isReadOnly = true;
		}

		private void SelectionChanged(List<object> items)
		{
			if (items.Count > 0)
				ShowSample(items[0] as SampleData);
		}

		private void ShowSample(SampleData sample)
		{
			_codeView.Clear();
			_uxmlView.Clear();

			_uxmlView.AddUxml($"{sample.Name}/{sample.Name}UxmlSample.uxml");

			sample.Code.Create(_codeView);
			sample.Uxml.Setup(_uxmlView);

			_codeText.value = GetCodeText(sample.Name);
			_uxmlText.value = GetUxmlText(sample.Name);
			_uxmlCodeText.value = GetUxmlCodeText(sample.Name);
		}

		private string GetCodeText(string name)
		{
			return File.ReadAllText($"{RootPath}{name}/{name}CodeSample.cs");
		}

		private string GetUxmlText(string name)
		{
			return File.ReadAllText($"{RootPath}{name}/{name}UxmlSample.uxml");
		}

		private string GetUxmlCodeText(string name)
		{
			return File.ReadAllText($"{RootPath}{name}/{name}UxmlSample.cs");
		}
	}
}
