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

	public abstract class SerializedData
	{
		[SerializeField] public List<Object> _references = new List<Object>();

		public void SetData(byte[] data)
		{
			var content = Convert.ToBase64String(data);
			SetContent(content);
		}

		public byte[] GetData(int index)
		{
			var content = GetContent(index);
			return Convert.FromBase64String(content);
		}

		public int AddReference(Object o)
		{
			_references.Add(o);
			return _references.Count - 1;
		}

		public Object GetReference(int index)
		{
			return index >= 0 && index < _references.Count
				? _references[index]
				: null;
		}

		public void Clear()
		{
			_references.Clear();
			ClearContent();
		}

		#region Abstract Interface

		protected abstract void ClearContent();
		protected abstract void SetContent(string content);
		protected abstract string GetContent(int index);

		#endregion
	}

	[Serializable]
	public class SerializedDataItem : SerializedData
	{
		public const string ContentProperty = nameof(_content);

		[SerializeField] private string _content = string.Empty;

		protected override void ClearContent() => _content = string.Empty;
		protected override void SetContent(string content) => _content = content;
		protected override string GetContent(int index) => _content;
	}

	[Serializable]
	public class SerializedDataList : SerializedData
	{
		public const string ContentProperty = nameof(_content);

		[SerializeField] private List<string> _content = new List<string>();

		public int Count => _content.Count;
		protected override void ClearContent() => _content.Clear();
		protected override void SetContent(string content) => _content.Add(content);
		protected override string GetContent(int index) => index >= 0 && index < _content.Count ? _content[index] : string.Empty;
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
			// the instance id is written to force the content string to change when the object changes, thus properly
			// updating any bindings

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
			// reordering/adding/removing of enum values without affecting saved data.

			SaveType(e.GetType());
			Writer.Write(e.ToString());
		}
	}

	public class SerializedDataReader : IDisposable
	{
		public MemoryStream Stream;
		public BinaryReader Reader;
		public SerializedData Data;

		public SerializedDataReader(SerializedDataItem data)
		{
			var bytes = data.GetData(0);

			Stream = new MemoryStream(bytes);
			Reader = new BinaryReader(Stream);
			Data = data;
		}

		public SerializedDataReader(SerializedDataList data, int index)
		{
			var bytes = data.GetData(index);

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
			var index = Reader.ReadInt32();
			var id = Reader.ReadInt32();
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
