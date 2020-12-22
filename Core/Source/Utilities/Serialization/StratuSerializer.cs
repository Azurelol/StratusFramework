using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Stratus.OdinSerializer;
using System.IO;

namespace Stratus
{
	public static class StratusJSONSerializerUtility
	{
		public static string Serialize(object value)
		{
			return JsonUtility.ToJson(value);
		}

		public static T Deserialize<T>(string serialization)
		{
			return JsonUtility.FromJson<T>(serialization);
		}

	}

	/// <summary>
	/// Base class for serializes without type constraints
	/// </summary>
	public abstract class StratusSerializer
	{
		protected abstract void OnSerialize(object value, string filePath);
		protected abstract object OnDeserialize(string filePath);

		public void Serialize(object data, string filePath)
		{
			if (data == null)
			{
				throw new ArgumentNullException("No data to serialize");
			}

			if (filePath.IsNullOrEmpty())
			{
				throw new ArgumentNullException("No file path given");
			}

			OnSerialize(data, filePath);
		}

		public object Deserialize(string filePath)
		{
			if (filePath.IsNullOrEmpty())
			{
				throw new ArgumentNullException("No file path given");
			}
			return OnDeserialize(filePath);
		}

		public bool TrySerialize(object data, string filePath)
		{
			try
			{
				Serialize(data, filePath);
			}
			catch (Exception e)
			{
				return false;
			}
			return true;
		}

		public bool TryDeserialize(string filePath, out object data)
		{
			try
			{
				data = Deserialize(filePath);
			}
			catch (Exception e)
			{
				data = null;
				return false;
			}
			return true;
		}
	}

	public interface IStratusSerializer
	{
		void Serialize(object data, string filePath);
		object Deserialize(string filePath);
	}

	/// <summary>
	/// Base class for object serializers with enforced type constraint
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class StratusSerializer<T>
		where T : class, new()
	{
		public Type type { get; } = typeof(T);

		protected abstract void OnSerialize(T value, string filePath);
		protected abstract T OnDeserialize(string filePath);

		public void Serialize(T data, string filePath)
		{
			if (data == null)
			{
				throw new ArgumentNullException("No data to serialize");
			}

			if (filePath.IsNullOrEmpty())
			{
				throw new ArgumentNullException("No file path given");
			}

			OnSerialize(data, filePath);
		}

		public T Deserialize(string filePath)
		{
			if (filePath.IsNullOrEmpty())
			{
				throw new ArgumentNullException("No file path given");
			}
			return OnDeserialize(filePath);
		}

		public bool TrySerialize(T data, string filePath)
		{
			try
			{
				Serialize(data, filePath);
			}
			catch (Exception e)
			{
				return false;
			}
			return true;
		}

		public bool TryDeserialize(string filePath, out T data)
		{
			try
			{
				data = Deserialize(filePath);
			}
			catch (Exception e)
			{
				data = null;
				return false;
			}
			return true;
		}
	}

	/// <summary>
	/// A serializer using the binary format
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class StratusBinarySerializer<T> : StratusSerializer<T>
		where T : class, new()
	{
		protected override T OnDeserialize(string filePath)
		{
			byte[] serialization = File.ReadAllBytes(filePath);
			return SerializationUtility.DeserializeValue<T>(serialization, DataFormat.Binary);
		}

		protected override void OnSerialize(T value, string filePath)
		{
			byte[] serialization = SerializationUtility.SerializeValue<T>(value, DataFormat.Binary);
			File.WriteAllBytes(filePath, serialization);
		}
	}

	/// <summary>
	/// A serializer using the binary format
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class StratusBinarySerializer : StratusSerializer
	{
		protected override object OnDeserialize(string filePath)
		{
			byte[] serialization = File.ReadAllBytes(filePath);
			return SerializationUtility.DeserializeValueWeak(serialization, DataFormat.Binary);
		}

		protected override void OnSerialize(object value, string filePath)
		{
			byte[] serialization = SerializationUtility.SerializeValue(value, DataFormat.Binary);
			File.WriteAllBytes(filePath, serialization);
		}
	}

	/// <summary>
	/// A serializer using the JSON format
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class StratusJSONSerializer<T> : StratusSerializer<T>
		where T : class, new()
	{
		protected override T OnDeserialize(string filePath)
		{
			byte[] serialization = File.ReadAllBytes(filePath);
			return SerializationUtility.DeserializeValue<T>(serialization, DataFormat.JSON);
		}

		protected override void OnSerialize(T value, string filePath)
		{
			byte[] serialization = SerializationUtility.SerializeValue<T>(value, DataFormat.JSON);
			File.WriteAllBytes(filePath, serialization);
		}
	}



	/// <summary>
	/// A serializer using the JSON format
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class StratusJSONSerializer : StratusSerializer
	{
		protected override object OnDeserialize(string filePath)
		{
			byte[] serialization = File.ReadAllBytes(filePath);
			return SerializationUtility.DeserializeValueWeak(serialization, DataFormat.JSON);
		}

		protected override void OnSerialize(object value, string filePath)
		{
			byte[] serialization = SerializationUtility.SerializeValue(value, DataFormat.JSON);
			File.WriteAllBytes(filePath, serialization);
		}
	}


}