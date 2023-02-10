using Nightingale.Extensions;
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Nightingale.Utilitys
{
	public class FileUtility
	{
		public static AssetBundle ReadAssetBunld(string path)
		{
			if (File.Exists(Path.Combine(Application.persistentDataPath, path)))
			{
				byte[] bytes = File.ReadAllBytes(Path.Combine(Application.persistentDataPath, path));
				return bytes.ToAssetBundle();
			}
			return null;
		}

		public static string ReadText(string path)
		{
			if (File.Exists(Path.Combine(Application.persistentDataPath, path)))
			{
				return File.ReadAllText(Path.Combine(Application.persistentDataPath, path));
			}
			return string.Empty;
		}

		public static void CreateDirectory(string path)
		{
			string directoryName = Path.GetDirectoryName(path);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
		}

		public static void CreateFile(string path)
		{
			string directoryName = Path.GetDirectoryName(path);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			if (!File.Exists(path))
			{
				File.Create(path).Dispose();
			}
		}

		public static void DeleteFile(string path)
		{
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}

		public static void SaveFile(string path, string fileName, long seek, byte[] bytes)
		{
			fileName = Path.Combine(path, fileName);
			try
			{
				CreateDirectory(fileName);
			}
			catch (Exception)
			{
			}
			using (FileStream fileStream = File.OpenWrite(fileName))
			{
				fileStream.SetLength(0L);
				fileStream.Position = 0L;
				fileStream.Seek(seek, SeekOrigin.Begin);
				fileStream.Write(bytes, 0, bytes.Length);
			}
		}

		public static void SaveFile(string path, string fileName, byte[] bytes)
		{
			SaveFile(path, fileName, 0L, bytes);
		}

		public static void SaveFile(string path, string fileName, string content)
		{
			SaveFile(path, fileName, Encoding.UTF8.GetBytes(content));
		}

		public static void SaveFile(string path, byte[] bytes)
		{
			if (File.Exists(path))
			{
				File.Delete(path);
			}
			CreateDirectory(path);
			File.WriteAllBytes(path, bytes);
		}

		public static bool Exists(string path)
		{
			if (File.Exists(Path.Combine(Application.persistentDataPath, path)))
			{
				return true;
			}
			if (StreamingAssetsPathUtility.Get().Exists(path))
			{
				return true;
			}
			return false;
		}

		public static bool Exists(string root, string path)
		{
			return File.Exists(Path.Combine(root, path));
		}

		public static bool IsSameFile(string path1, string path2)
		{
			if (path1.Equals(path2))
			{
				return true;
			}
			path1 = path1.Replace("\\", "/").Replace("//", "/").ToLower();
			path2 = path2.Replace("\\", "/").Replace("//", "/").ToLower();
			if (path1.EndsWith($"/{path2}"))
			{
				return true;
			}
			if (path2.EndsWith($"/{path1}"))
			{
				return true;
			}
			return false;
		}
	}
}
