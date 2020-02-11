using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities
{
	public interface ISerializableData
	{
		void Save(SerializedDataWriter writer);
		void Load(SerializedDataReader reader);
	}

	[Serializable]
	public class SerializedData
	{
		[SerializeField] private string _content = string.Empty;
		[SerializeField] private List<Object> _references = new List<Object>();

		public bool IsEmpty => string.IsNullOrEmpty(_content);

		internal void SetData(byte[] data)
		{
			_content = Convert.ToBase64String(data);
		}

		internal byte[] GetData()
		{
			return Convert.FromBase64String(_content);
		}

		internal int AddReference(Object o)
		{
			_references.Add(o);
			return _references.Count - 1;
		}

		internal Object GetReference(int index)
		{
			return index >= 0 && index < _references.Count
				? _references[index]
				: null;
		}

		#region Editor Access

		public string EditorContent
		{
			get => _content;
			set => _content = value;
		}

		public List<Object> EditorReferences
		{
			get => _references;
			set => _references = value;
		}

		#endregion
	}

	public class SerializedDataWriter : IDisposable
	{
		public MemoryStream Stream;
		public BinaryWriter Writer;
		public SerializedData Data;

		public SerializedDataWriter(SerializedData data)
		{
			Stream = new MemoryStream();
			Writer = new BinaryWriter(Stream);
			Data = data;
		}

		public void Dispose()
		{
			Data.SetData(Stream.ToArray());

			Writer.Dispose();
			Stream.Dispose();
		}

		public void SaveReference(Object obj)
		{
			// The instance id is written to force the content string to change when the object changes, thus properly
			// updating any bindings.

			var index = Data.AddReference(obj);
			Writer.Write(obj.GetInstanceID());
			Writer.Write(index);
		}

		public void SaveInstance<T>(T obj)
		{
			Writer.Write(obj != null);
			Writer.Write(obj is ISerializableData);

			if (obj != null)
			{
				SaveType(obj.GetType());

				if (obj is ISerializableData data)
				{
					data.Save(this);
				}
				else
				{
					var json = JsonUtility.ToJson(obj);
					Writer.Write(json);
				}
			}
		}

		public void SaveType(Type type)
		{
			var name = type != null ? type.AssemblyQualifiedName : string.Empty;
			Writer.Write(name);
		}

		public void SaveEnum(Enum e)
		{
			// Saving as a string is the simplest way of handling enums with non Int32 underlying type. It also allows
			// reordering/adding/removing (but not renaming) of enum values without affecting saved data.

			SaveType(e.GetType());
			Writer.Write(e.ToString());
		}
	}

	public class SerializedDataReader : IDisposable
	{
		public MemoryStream Stream;
		public BinaryReader Reader;
		public SerializedData Data;

		public SerializedDataReader(SerializedData data)
		{
			var bytes = data.GetData();

			Stream = new MemoryStream(bytes);
			Reader = new BinaryReader(Stream);
			Data = data;
		}

		public void Dispose()
		{
			Reader.Dispose();
			Stream.Dispose();
		}

		public Object LoadReference()
		{
			var id = Reader.ReadInt32();
			var index = Reader.ReadInt32();
			return Data.GetReference(index);
		}

		public T LoadInstance<T>()
		{
			var isValid = Reader.ReadBoolean();
			var isData = Reader.ReadBoolean();

			if (isValid)
			{
				var type = LoadType();
				var obj = default(T);

				try { obj = (T)Activator.CreateInstance(type); }
				catch { }

				if (isData)
				{
					if (obj is ISerializableData data)
						data.Load(this);
				}
				else
				{
					var json = Reader.ReadString();

					if (obj != null)
						JsonUtility.FromJsonOverwrite(json, obj);
				}

				return obj;
			}

			return default;
		}

		public Type LoadType()
		{
			var name = Reader.ReadString();

			if (!string.IsNullOrEmpty(name))
			{
				try { return Type.GetType(name); }
				catch { }
			}

			return null;
		}

		public Enum LoadEnum()
		{
			var type = LoadType();
			var name = Reader.ReadString();

			try { return (Enum)Enum.Parse(type, name); }
			catch { }

			return null;
		}
	}
}
