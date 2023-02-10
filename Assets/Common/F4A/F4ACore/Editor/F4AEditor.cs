using System.IO;
using UnityEngine;
using UnityEditor;
using com.F4A.MobileThird;
using cookapps;

public class F4AEditor
{
    /// <summary>
    /// Careful when using
    /// Delete all empty folders in project (In folder Assets)
    /// </summary>
    /// <author>sinh.nguyen</author>
    [MenuItem("F4A/Unity/Delete Empty Directories In Project")]
    public static void DeleteEmptyDirsInProject()
    {
        string dir = Application.dataPath;

        foreach (var directory in Directory.GetDirectories(dir))
        {
            DeleteEmptyDirs(directory);
            if (Directory.GetFiles(directory).Length == 0 &&
                Directory.GetDirectories(directory).Length == 0)
            {
                Directory.Delete(directory, false);
            }
        }

        UnityEditor.AssetDatabase.Refresh();
    }

    private static void DeleteEmptyDirs(string dir)
    {
        if (string.IsNullOrEmpty(dir))
            return;

        foreach (var directory in Directory.GetDirectories(dir))
        {
            DeleteEmptyDirs(directory);
            if (Directory.GetFiles(directory).Length == 0 &&
                Directory.GetDirectories(directory).Length == 0)
            {
                Directory.Delete(directory, false);

#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif
            }
        }
    }

    [MenuItem("F4A/Unity/PlayerPres/Delete All")]
    public static void DeletePlayerPres()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("F4A/Unity/Editor/Delete All Pref")]
    static void DeleteAllEditorPrefs()
    {
        EditorPrefs.DeleteAll();
    }

    [MenuItem("F4A/Unity/Caching/Clear Cache")]
    static void ClearCache()
    {
        Caching.ClearCache();
    }

    private const string C_FOLDER_DATA_ENCRYPT = "table";
    private const string C_FOLDER_DATA_DECRYPT = "table_decrypt";
    [MenuItem("F4A/Cookapps/DecryptString")]
    static void Cookapps_DecryptString()
    {
        string pathInfo = Path.Combine(Application.dataPath, "Resources/" + C_FOLDER_DATA_ENCRYPT);
        string pathOut = Path.Combine(Application.dataPath, "Resources/" + C_FOLDER_DATA_DECRYPT);
        var pathFiles = DMCFileUtilities.GetAllFileInDirectory(pathInfo);
        if(pathFiles != null && pathFiles.Length > 0)
        {
            foreach (var pathFile in pathFiles)
            {
                try
                {
                    var text = File.ReadAllText(pathFile);
                    if (!string.IsNullOrEmpty(text))
                    {
                        Debug.Log($"@LOG Cookapps_DecryptString file:{pathFile}");
                        text = DataEncryption.DecryptString(text);
                        FileInfo info = new FileInfo(pathFile);
                        DMCFileUtilities.CreateDirectory(pathOut);
                        System.IO.File.WriteAllText(Path.Combine(pathOut, info.Name), text);
                        //Debug.Log($"@LOG file:{pathFile}".Color(Color.red));
                    }
                }
                catch
                {

                }
            }

        }
        UnityEditor.AssetDatabase.Refresh();
    }

    [MenuItem("F4A/Cookapps/EncryptString")]
    static void Cookapps_EncryptString()
    {
        string pathInfo = Path.Combine(Application.dataPath, "Resources/" + C_FOLDER_DATA_DECRYPT);
        string pathOut = Path.Combine(Application.dataPath, "Resources/" + C_FOLDER_DATA_ENCRYPT);
        if (Directory.Exists(pathOut))
        {
            pathOut += "_01";
        }
        else
        {
            Directory.CreateDirectory(pathOut);
        }
        var pathFiles = DMCFileUtilities.GetAllFileInDirectory(pathInfo);
        if (pathFiles != null && pathFiles.Length > 0)
        {
            foreach (var pathFile in pathFiles)
            {
                try
                {
                    if (!pathFile.EndsWith(".txt")) continue;
                    var text = File.ReadAllText(pathFile);
                    if (!string.IsNullOrEmpty(text))
                    {
                        text = DataEncryption.EncryptString(text);
                        FileInfo info = new FileInfo(pathFile);
                        DMCFileUtilities.CreateDirectory(pathOut);
                        System.IO.File.WriteAllText(Path.Combine(pathOut, info.Name), text);
                    }
                }
                catch
                {

                }
            }

        }
        UnityEditor.AssetDatabase.Refresh();
    }
}