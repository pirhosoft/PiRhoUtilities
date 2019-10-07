using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.Utilities.Editor
{
	public class BoolPreference
	{
		private string _name;
		private bool _default;

		public BoolPreference(string name, bool defaultValue)
		{
			_name = name;
			_default = defaultValue;
		}

		public bool Value
		{
			get { return EditorPrefs.GetBool(_name, _default); }
			set { EditorPrefs.SetBool(_name, value); }
		}
	}

	public class IntPreference
	{
		private string _name;
		private int _default;

		public IntPreference(string name, int defaultValue)
		{
			_name = name;
			_default = defaultValue;
		}

		public int Value
		{
			get { return EditorPrefs.GetInt(_name, _default); }
			set { EditorPrefs.SetInt(_name, value); }
		}
	}

	public class FloatPreference
	{
		private string _name;
		private float _default;

		public FloatPreference(string name, float defaultValue)
		{
			_name = name;
			_default = defaultValue;
		}

		public float Value
		{
			get { return EditorPrefs.GetFloat(_name, _default); }
			set { EditorPrefs.SetFloat(_name, value); }
		}
	}

	public class StringPreference
	{
		private string _name;
		private string _default;

		public StringPreference(string name, string defaultValue)
		{
			_name = name;
			_default = defaultValue;
		}

		public string Value
		{
			get { return EditorPrefs.GetString(_name, _default); }
			set { EditorPrefs.SetString(_name, value); }
		}
	}

	public class JsonPreference<T>
	{
		private string _name;
		private string _default;

		public JsonPreference(string name)
		{
			_name = name;
			_default = "{}";
		}

		public T Value
		{
			get
			{
				var json = EditorPrefs.GetString(_name, _default);
				var state = JsonUtility.FromJson<T>(json);
				return state;
			}
			set
			{
				var json = JsonUtility.ToJson(value);
				EditorPrefs.SetString(_name, json);
			}
		}
	}
}
