using UnityEngine;
using UnityEditor;
public class FindMissingScriptsRecursively : EditorWindow
{
    static int go_count = 0, components_count = 0, missing_count = 0;

    [MenuItem("F4A/Unity/Remove mission scripts recursively")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FindMissingScriptsRecursively));
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Find Missing Scripts in selected GameObjects"))
        {
            FindInSelected();
        }
    }
    private static void FindInSelected()
    {
        GameObject[] go = Selection.gameObjects;
        go_count = 0;
        components_count = 0;
        missing_count = 0;
        foreach (GameObject g in go)
        {
            FindInGO(g);
        }
        Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
    }

    private static void FindInGO(GameObject g)
    {
        go_count++;
        Component[] components = g.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            components_count++;
            if (components[i] == null)
            {
                missing_count++;
                string s = g.name;
                Transform t = g.transform;
                while (t.parent != null)
                {
                    s = t.parent.name + "/" + s;
                    t = t.parent;
                }
                Debug.Log(s + " has an empty script attached in position: " + i, g);
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(g);
            }
        }
        // Now recurse through each child GO (if there are any):
        foreach (Transform childT in g.transform)
        {
            //Debug.Log("Searching " + childT.name  + " " );
            FindInGO(childT.gameObject);
        }
    }


    //private static int _goCount;
    //private static int _componentsCount;
    //private static int _missingCount;

    //private static bool _bHaveRun;

    //[MenuItem("FLGCore/Editor/Utility/FindMissingScriptsRecursivelyAndRemove")]
    //public static void ShowWindow()
    //{
    //    GetWindow(typeof(FindMissingScriptsRecursively));
    //}

    //public void OnGUI()
    //{
    //    if (GUILayout.Button("Find Missing Scripts in selected GameObjects"))
    //    {
    //        FindInSelected();
    //    }

    //    if (!_bHaveRun) return;

    //    EditorGUILayout.TextField($"{_goCount} GameObjects Selected");
    //    if (_goCount > 0) EditorGUILayout.TextField($"{_componentsCount} Components");
    //    if (_goCount > 0) EditorGUILayout.TextField($"{_missingCount} Deleted");
    //}

    //private static void FindInSelected()
    //{
    //    var go = Selection.gameObjects;
    //    _goCount = 0;
    //    _componentsCount = 0;
    //    _missingCount = 0;
    //    foreach (var g in go)
    //    {
    //        FindInGo(g);
    //    }

    //    _bHaveRun = true;
    //    Debug.Log($"Searched {_goCount} GameObjects, {_componentsCount} components, found {_missingCount} missing");

    //    AssetDatabase.SaveAssets();
    //}

    //private static void FindInGo(GameObject g)
    //{
    //    _goCount++;
    //    var components = g.GetComponents<Component>();

    //    var r = 0;

    //    for (var i = 0; i < components.Length; i++)
    //    {
    //        _componentsCount++;
    //        if (components[i] != null) continue;
    //        _missingCount++;
    //        var s = g.name;
    //        var t = g.transform;
    //        while (t.parent != null)
    //        {
    //            s = t.parent.name + "/" + s;
    //            t = t.parent;
    //        }

    //        Debug.Log($"{s} has a missing script at {i}", g);

    //        var serializedObject = new SerializedObject(g);

    //        var prop = serializedObject.FindProperty("m_Component");

    //        prop.DeleteArrayElementAtIndex(i - r);
    //        r++;
    //        serializedObject.ApplyModifiedProperties();
    //    }

    //    foreach (Transform childT in g.transform)
    //    {
    //        FindInGo(childT.gameObject);
    //    }
    //}
}