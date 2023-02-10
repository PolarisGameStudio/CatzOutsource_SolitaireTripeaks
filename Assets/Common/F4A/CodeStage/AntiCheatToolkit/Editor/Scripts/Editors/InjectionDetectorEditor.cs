#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2013-2019 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

#if UNITY_EDITOR
namespace CodeStage.AntiCheat.EditorCode.Editors
{
	using Detectors;
	using Windows;

	using UnityEditor;
	using UnityEngine;

	[CustomEditor(typeof (InjectionDetector))]
	internal class InjectionDetectorEditor : ACTkDetectorEditor
	{
		protected override bool DrawUniqueDetectorProperties()
		{
			if (!EditorPrefs.GetBool(ACTkEditorGlobalStuff.PrefsInjectionEnabled))
			{
				EditorGUILayout.Separator();
				EditorGUILayout.LabelField("Injection Detector is not enabled!", EditorStyles.boldLabel);
				if (GUILayout.Button("Enable in Settings..."))
				{
					ACTkSettings.ShowWindow();
				}

				return true;
			}

			if (!ACTkPostprocessor.IsInjectionDetectorTargetCompatible())
			{
				EditorGUILayout.LabelField("Injection Detector disabled for this platform.", EditorStyles.boldLabel);
				return true;
			}

			return false;
		}
	}
}
#endif