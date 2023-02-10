#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2013-2019 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

#if UNITY_EDITOR
namespace CodeStage.AntiCheat.EditorCode.Editors
{
	using Detectors;

	using UnityEditor;
	using UnityEngine;

	internal class ACTkDetectorEditor : Editor
	{
		protected SerializedProperty autoStart;
		protected SerializedProperty autoDispose;
		protected SerializedProperty keepAlive;
		protected SerializedProperty detectionEvent;
		protected SerializedProperty detectionEventHasListener;

		protected ACTkDetectorBase self;

		public virtual void OnEnable()
		{
			autoStart = serializedObject.FindProperty("autoStart");
			autoDispose = serializedObject.FindProperty("autoDispose");
			keepAlive = serializedObject.FindProperty("keepAlive");
			detectionEvent = serializedObject.FindProperty("detectionEvent");
			detectionEventHasListener = serializedObject.FindProperty("detectionEventHasListener");

			self = target as ACTkDetectorBase;

			FindUniqueDetectorProperties();
		}

		public override void OnInspectorGUI()
		{
			if (self == null) return;

			serializedObject.Update();

			EditorGUIUtility.labelWidth = 140;
			EditorGUILayout.Space();
			DrawHeader("Base settings");
			
			EditorGUILayout.PropertyField(autoStart);
			detectionEventHasListener.boolValue = ACTkEditorGlobalStuff.CheckUnityEventHasActivePersistentListener(detectionEvent);

			CheckAdditionalEventsForListeners();

			if (autoStart.boolValue && !detectionEventHasListener.boolValue && !AdditionalEventsHasListeners())
			{
				EditorGUILayout.LabelField(new GUIContent("You need to add at least one active item to the Events in order to use Auto Start feature!"), ACTkEditorGUI.BoldLabel);
			}
			else if (!autoStart.boolValue)
			{
				EditorGUILayout.LabelField(new GUIContent("Don't forget to start detection!", "You should start detector from code using ObscuredCheatingDetector.StartDetection() method. See readme for details."), ACTkEditorGUI.BoldLabel);
				EditorGUILayout.Separator();
			}
			EditorGUILayout.PropertyField(autoDispose);
			EditorGUILayout.PropertyField(keepAlive);

			EditorGUILayout.Separator();

			if (DrawUniqueDetectorProperties())
			{
				EditorGUILayout.Separator();
			}

			//DrawHeader("Events");

			EditorGUILayout.PropertyField(detectionEvent);
			DrawAdditionalEvents();
			serializedObject.ApplyModifiedProperties();

			EditorGUIUtility.labelWidth = 0;
		}

		protected virtual void DrawHeader(string text)
		{
			ACTkEditorGUI.DrawHeader(text);
		}

		protected virtual bool AdditionalEventsHasListeners()
		{
			return true;
		}

		protected virtual void FindUniqueDetectorProperties() {}
		protected virtual bool DrawUniqueDetectorProperties() { return false; }
		protected virtual void CheckAdditionalEventsForListeners() {}
		protected virtual void DrawAdditionalEvents() {}
	}
}
#endif