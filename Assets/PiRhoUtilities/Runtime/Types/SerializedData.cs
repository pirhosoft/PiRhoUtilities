using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities
{
	public interface ISerializableData
	{
		void Save(BinaryWriter writer, SerializedData data);
		void Load(BinaryReader reader, SerializedData data);
	}

	[Serializable]
	public class SerializedData
	{
		public int Version;
		public string Data;
		public List<Object> References;

		public void SaveData(ISerializableData data, int version)
		{
			Reset(version);

			using (var stream = new MemoryStream())
			{
				using (var writer = new BinaryWriter(stream))
				{
					writer.Write(Version);
					data.Save(writer, this);
				}

				Data = Convert.ToBase64String(stream.ToArray());
			}
		}

		public void LoadData(ISerializableData data)
		{
			var bytes = Convert.FromBase64String(Data);

			if (bytes != null && bytes.Length > 0)
			{
				using (var stream = new MemoryStream(bytes))
				{
					using (var reader = new BinaryReader(stream))
					{
						Version = reader.ReadInt32();
						data.Load(reader, this);
					}
				}
			}

			Reset(-1);
		}

		public void SaveClass(ISerializableData data, int version)
		{
			Reset(version);

			if (data != null)
			{
				using (var stream = new MemoryStream())
				{
					using (var writer = new BinaryWriter(stream))
					{
						writer.Write(Version);
						SaveInstance(writer, data);
					}

					Data = Convert.ToBase64String(stream.ToArray());
				}
			}
			else
			{
				Data = string.Empty;
			}
		}

		public void LoadClass<T>(out T data) where T : class, ISerializableData
		{
			var bytes = Convert.FromBase64String(Data);

			if (bytes != null && bytes.Length > 0)
			{
				using (var stream = new MemoryStream(bytes))
				{
					using (var reader = new BinaryReader(stream))
					{
						Version = reader.ReadInt32();
						data = LoadInstance<T>(reader);
					}
				}
			}
			else
			{
				data = null;
			}

			Reset(-1);
		}

		public void SaveReference(BinaryWriter writer, Object obj)
		{
			if (References == null)
				References = new List<Object>();

			writer.Write(References.Count);
			References.Add(obj);
		}

		public Object LoadReference(BinaryReader reader)
		{
			var index = reader.ReadInt32();
			return References != null && index < References.Count ? References[index] : null;
		}

		public void SaveInstance<T>(BinaryWriter writer, T obj)
		{
			writer.Write(obj != null);
			writer.Write(obj is ISerializableData);

			if (obj != null)
			{
				SaveType(writer, obj.GetType());

				if (obj is ISerializableData data)
				{
					data.Save(writer, this);
				}
				else
				{
					var json = JsonUtility.ToJson(obj);
					writer.Write(json);
				}
			}
		}

		public T LoadInstance<T>(BinaryReader reader)
		{
			var isValid = reader.ReadBoolean();
			var isData = reader.ReadBoolean();

			if (isValid)
			{
				var type = LoadType(reader);
				var obj = default(T);

				try { obj = (T)Activator.CreateInstance(type); }
				catch { }

				if (isData)
				{
					if (obj is ISerializableData data)
						data.Load(reader, this);
				}
				else
				{
					var json = reader.ReadString();

					if (obj != null)
						JsonUtility.FromJsonOverwrite(json, obj);
				}

				return obj;
			}

			return default;
		}

		public void SaveType(BinaryWriter writer, Type type)
		{
			var name = type != null ? type.AssemblyQualifiedName : string.Empty;
			writer.Write(name);
		}

		public Type LoadType(BinaryReader reader)
		{
			var name = reader.ReadString();

			if (!string.IsNullOrEmpty(name))
			{
				try { return Type.GetType(name); }
				catch { }
			}

			return null;
		}

		public void SaveEnum(BinaryWriter writer, Enum e)
		{
			// Saving as a string is the simplest way of handling enums with non Int32 underlying type. It also allows
			// reordering/adding/removing of enum values without affecting saved data.

			SaveType(writer, e.GetType());
			writer.Write(e.ToString());
		}

		public Enum LoadEnum(BinaryReader reader)
		{
			var type = LoadType(reader);
			var name = reader.ReadString();

			try { return (Enum)Enum.Parse(type, name); }
			catch { }

			return null;
		}

		private void Reset(int version)
		{
			Version = version;
			Data = null;
			References = null;
		}
	}
}