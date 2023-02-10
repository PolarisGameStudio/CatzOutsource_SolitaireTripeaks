using System;
using System.IO;
using System.Text;
using TriPeaks.ProtoData;

namespace SolitaireTripeaks
{
	public class ProtoDataUtility
	{
		private static Serializer _ser = new Serializer();

		public static string SerializeToBase64(object obj)
		{
			return Convert.ToBase64String(Serialize(obj));
		}

		public static byte[] Serialize(object obj)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				_ser.Serialize(memoryStream, obj);
				return memoryStream.ToArray();
			}
		}

		public static string SerializeToString(object obj)
		{
			return Encoding.UTF8.GetString(Serialize(obj));
		}

		public static void Serialize(Stream stream, object obj)
		{
			_ser.Serialize(stream, obj);
		}

		public static T Deserialize<T>(byte[] bytes)
		{
			try
			{
				using (MemoryStream source = new MemoryStream(bytes))
				{
					return (T)_ser.Deserialize(source, null, typeof(T));
				}
			}
			catch (Exception)
			{
				return default(T);
			}
		}

		public static T Deserialize<T>(ArraySegment<byte> bytes)
		{
			using (MemoryStream source = new MemoryStream(bytes.Array, bytes.Offset, bytes.Count))
			{
				return (T)_ser.Deserialize(source, null, typeof(T));
			}
		}

		public static T Deserialize<T>(Stream stream)
		{
			return (T)_ser.Deserialize(stream, null, typeof(T));
		}

		public static T Deserialize<T>(string content)
		{
			return Deserialize<T>(Encoding.UTF8.GetBytes(content));
		}
	}
}
