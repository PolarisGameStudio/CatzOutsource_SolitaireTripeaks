#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2013-2019 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

#if UNITY_EDITOR
namespace CodeStage.AntiCheat.EditorCode.PropertyDrawers
{
	using ObscuredTypes;

	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(ObscuredQuaternion))]
	internal class ObscuredQuaternionDrawer : ObscuredPropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
		{

        }

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return -EditorGUIUtility.standardVerticalSpacing;
		}
	}
}
#endif