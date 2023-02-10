namespace com.F4A.MobileThird
{
    using Newtonsoft.Json;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    public static class DMCFileUtilities
    {
        public static string GetWritablePath(string relativeFilePath)
        {
            string empty = string.Empty;
            return Application.persistentDataPath + "/" + relativeFilePath;
        }

        public static byte[] LoadFile(string filePath, bool isAbsolutePath = false)
        {
            if (filePath == null || filePath.Length == 0)
            {
                return null;
            }
            string path = filePath;
            if (!isAbsolutePath)
            {
                path = GetWritablePath(filePath);
            }
            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }
            return null;
        }

        public static string LoadContentFile(string filePath, bool isAbsolutePath = false)
        {
            if (filePath == null || filePath.Length == 0)
            {
                return string.Empty;
            }
            string path = filePath;
            if (IsFileExist(path, isAbsolutePath))
            {
                if (!isAbsolutePath)
                {
                    path = GetWritablePath(filePath);
                }

                return File.ReadAllText(path);
            }
            return string.Empty;
        }

        public static T LoadContentWithPath<T>(string filePath, bool isAbsolutePath = false)
        {
            if (filePath == null || filePath.Length == 0)
            {
                return default(T);
            }
            string path = filePath;
            if (!isAbsolutePath)
            {
                path = GetWritablePath(filePath);
            }
            if (File.Exists(path))
            {
                try
                {
                    Debug.Log("DMCFileUtilities LoadFileWithJson");
                    string str = File.ReadAllText(path);
                    if (!string.IsNullOrEmpty(str))
                    {
                        return JsonConvert.DeserializeObject<T>(str);
                    }
                }
                catch(Exception ex)
                {
                    Debug.LogError("DMCFileUtilities LoadFileWithJson ex:" + ex.Message);
                }
            }
            return default(T);
        }

        public static T LoadContentFromResource<T>(string filePath)
        {
            if (filePath == null || filePath.Length == 0)
            {
                return default(T);
            }
            if (filePath.EndsWith(".json")) filePath = filePath.Substring(0, filePath.Length - 5);
            if (filePath.EndsWith(".txt")) filePath = filePath.Substring(0, filePath.Length - 4);

            TextAsset textAsset = Resources.Load(filePath) as TextAsset;
            if (textAsset != null && !string.IsNullOrEmpty(textAsset.text))
            {
                try
                {
                    Debug.Log("DMCFileUtilities LoadFileFromResource");
                    return JsonConvert.DeserializeObject<T>(textAsset.text);
                }
                catch (Exception ex)
                {
                    Debug.LogError("DMCFileUtilities LoadFileFromResource ex:" + ex.Message);
                }
            }
            return default(T);
        }

        public static bool IsFileExist(string filePath, bool isAbsolutePath = false)
        {
            if (filePath == null || filePath.Length == 0)
            {
                return false;
            }
            string path = filePath;
            if (!isAbsolutePath)
            {
                path = GetWritablePath(filePath);
            }
            return File.Exists(path);
        }

        public static string SaveFile(byte[] bytes, string filePath, bool isAbsolutePath = false, bool isSaveResource = false)
        {
            if (filePath == null || filePath.Length == 0)
            {
                return string.Empty;
            }
            string path = filePath;
            if (!isAbsolutePath)
            {
                path = GetWritablePath(filePath);
            }
            string directoryName = Path.GetDirectoryName(path);
            CreateDirectory(directoryName);
            File.WriteAllBytes(path, bytes);
            return path;
        }

        public static string SaveFile(string content, string filePath, bool isAbsolutePath = false, bool isSaveResource = false)
        {
            if (filePath == null || filePath.Length == 0)
            {
                return string.Empty;
            }
            string path = filePath;
            if (!isAbsolutePath)
            {
                path = GetWritablePath(filePath);
            }
            string directoryName = Path.GetDirectoryName(path);
            CreateDirectory(directoryName);
            File.WriteAllText(path, content);
            return path;
        }

        public static void SaveFileByData<T>(T content, string filePath, bool isAbsolutePath = false)
        {
            if (filePath == null || filePath.Length == 0)
            {
                return;
            }
            string path = filePath;
            if (!isAbsolutePath)
            {
                path = GetWritablePath(filePath);
            }

            if (content == null) return;
            try
            {
                var str = JsonConvert.SerializeObject(content);
                if (string.IsNullOrEmpty(str)) return;
                string directoryName = Path.GetDirectoryName(path);
                CreateDirectory(directoryName);
                File.WriteAllText(path, str);
            }
            catch (Exception ex)
            {
                Debug.Log($"@LOG DMCFileUtilities ex:{ex.Message}");
            }
        }

        public static string SaveFileOtherThread(byte[] bytes, string filePath)
        {
            if (filePath == null || filePath.Length == 0)
            {
                return null;
            }
            string directoryName = Path.GetDirectoryName(filePath);
            CreateDirectory(directoryName);
            File.WriteAllBytes(filePath, bytes);
            return filePath;
        }

        public static void DeleteFile(string filePath, bool isAbsolutePath = false)
        {
            if (filePath == null || filePath.Length == 0)
            {
                return;
            }
            if (isAbsolutePath)
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            else
            {
                string writablePath = GetWritablePath(filePath);
                DeleteFile(writablePath);
            }
        }


        public static string[] GetAllFileInDirectory(string targetDirectory)
        {
            if (!Directory.Exists(targetDirectory)) return null;
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            return fileEntries;
        }

        public static void CreateDirectory(string targetDirectory)
        {
            if (Directory.Exists(targetDirectory)) return;
            else
            {
                var parent = Directory.GetParent(targetDirectory);
                CreateDirectory(parent.FullName);
                Debug.Log("@LOG CreateDirectory " + targetDirectory);
                Directory.CreateDirectory(targetDirectory);
            }
        }
    }
}