using UnityEngine;
using UnityEditor;

namespace com.F4A.MobileThird
{
	[CustomEditor(typeof(F4AAssetBundleManager))]
	[CanEditMultipleObjects]
	public class F4AAssetBundleEditor : Editor {
		F4AAssetBundleManager assetBundleManager;
		
		SerializedProperty typeDownloadAssetBundle, platformBundleInfo, BundleInfos;
		SerializedProperty typeLoadConfigAB;
		SerializedProperty urlConfigAssetBundle, assetBundleConfig, configDefault;
		SerializedProperty platformName;
		
		// This function is called when the object is loaded.
		protected void OnEnable()
		{
			assetBundleManager = (F4AAssetBundleManager)target;

			typeDownloadAssetBundle = serializedObject.FindProperty("typeDownloadAssetBundle");
			platformBundleInfo = serializedObject.FindProperty("platformBundleInfo");
			BundleInfos = serializedObject.FindProperty("BundleInfos");
			typeLoadConfigAB = serializedObject.FindProperty("typeLoadConfigAB");
			urlConfigAssetBundle = serializedObject.FindProperty("urlConfigAssetBundle");
			assetBundleConfig = serializedObject.FindProperty("assetBundleConfig");
			configDefault = serializedObject.FindProperty("configDefault");
			platformName = serializedObject.FindProperty("platformName");
		}
		
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			serializedObject.Update ();
   //         GUILayout.Label("Action: OnLoadAllAssetStart");
   //         GUILayout.Label("Action: OnLoadAllAssetCompleted");
   //         GUILayout.Label("Action: OnLoadAllAssetFailed");
   //         GUILayout.Label("Action: OnLoadAssetStart");
   //         GUILayout.Label("Action: OnLoadAssetCompleted");
   //         GUILayout.Label("Action: OnLoadAssetFailed");
   //         GUILayout.Label("Action: OnDownloadProgress");
   //         GUILayout.Label("Action: OnRequestProgress");
   //         GUILayout.Label("----------------------------------------------------");

			//EditorGUILayout.PropertyField(typeDownloadAssetBundle, true, GUILayout.Width(200), GUILayout.MaxWidth(500));
			//EditorGUILayout.PropertyField(platformBundleInfo, true, GUILayout.Width(200), GUILayout.MaxWidth(500));
			//EditorGUILayout.PropertyField(BundleInfos, true);
			//EditorGUILayout.PropertyField(typeLoadConfigAB, true, GUILayout.Width(200), GUILayout.MaxWidth(500));
			//EditorGUILayout.PropertyField(configDefault, true, GUILayout.Width(200), GUILayout.MaxWidth(500));
			
			serializedObject.ApplyModifiedProperties ();
		}
	}
}