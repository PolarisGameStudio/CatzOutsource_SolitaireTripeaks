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

	[CustomEditor(typeof (SpeedHackDetector))]
	internal class SpeedHackDetectorEditor : ACTkDetectorEditor
	{
		private SerializedProperty interval;
		private SerializedProperty maxFalsePositives;
		private SerializedProperty coolDown;

		protected override void FindUniqueDetectorProperties()
		{
			interval = serializedObject.FindProperty("interval");
			maxFalsePositives = serializedObject.FindProperty("maxFalsePositives");
			coolDown = serializedObject.FindProperty("coolDown");
		}

		protected override bool DrawUniqueDetectorProperties()
		{
			DrawHeader("Specific settings");

			EditorGUILayout.PropertyField(interval);
			EditorGUILayout.PropertyField(maxFalsePositives);
			EditorGUILayout.PropertyField(coolDown);

			return true;
		}
	}
}
#endif