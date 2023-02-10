using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Nightingale.Utilitys
{
	public class FileAsynUtility : SingletonBehaviour<FileAsynUtility>
	{
		public static List<string> GetFiles(string path)
		{
			List<string> list = new List<string>();
			if (Exists(path))
			{
				list.AddRange(Directory.GetFiles(path));
				list.AddRange(Directory.GetDirectories(path));
			}
			return list;
		}

		public static List<string> GetAllFiles(string path)
		{
			List<string> list = new List<string>();
			if (Exists(path))
			{
				list.AddRange(Directory.GetFiles(path));
				string[] directories = Directory.GetDirectories(path);
				string[] array = directories;
				foreach (string path2 in array)
				{
					list.AddRange(GetAllFiles(path2));
				}
			}
			return list;
		}

		public static string ReadFile(string path)
		{
			if (Exists(Application.persistentDataPath, path))
			{
				return File.ReadAllText(Path.Combine(Application.persistentDataPath, path));
			}
			return string.Empty;
		}

		public static void DeleteFile(string path)
		{
			if (Exists(Application.persistentDataPath, path))
			{
				File.Delete(Path.Combine(Application.persistentDataPath, path));
			}
		}

		public static void CreateDirectory(string path)
		{
			string directoryName = Path.GetDirectoryName(path);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
		}

		public static void SaveFile(string path, string fileName, string content)
		{
			SaveFile(path, fileName, Encoding.UTF8.GetBytes(content));
		}

		public static void SaveFileEnd(string path, string fileName, byte[] bytes)
		{
			fileName = Path.Combine(path, fileName);
			CreateDirectory(fileName);
			using (FileStream fileStream = File.OpenWrite(fileName))
			{
				fileStream.Position = 0L;
				fileStream.Seek(0L, SeekOrigin.End);
				fileStream.Write(bytes, 0, bytes.Length);
			}
		}

		public static void SaveFile(string path, string fileName, byte[] bytes)
		{
			SaveFile(path, fileName, 0L, bytes);
		}

		public static void SaveFile(string path, string fileName, long seek, byte[] bytes)
		{
			fileName = Path.Combine(path, fileName);
			CreateDirectory(fileName);
			using (FileStream fileStream = File.OpenWrite(fileName))
			{
				fileStream.Position = 0L;
				fileStream.Seek(seek, SeekOrigin.Begin);
				fileStream.Write(bytes, 0, bytes.Length);
			}
		}

		public static long GetFileSeek(string path, string fileName)
		{
			fileName = Path.Combine(path, fileName);
			if (Exists(fileName))
			{
				using (FileStream fileStream = File.OpenWrite(fileName))
				{
					return fileStream.Length;
				}
			}
			return 0L;
		}

		public static bool Exists(string path, string fileName)
		{
			return Exists(Path.Combine(path, fileName));
		}

		public static bool Exists(string fileName)
		{
			if (Directory.Exists(fileName))
			{
				Debug.Log("FileAsynUtility Exists 1");
                return true;
			}
			Debug.Log("FileAsynUtility Exists 2");
            return File.Exists(fileName);
		}
	}
}
