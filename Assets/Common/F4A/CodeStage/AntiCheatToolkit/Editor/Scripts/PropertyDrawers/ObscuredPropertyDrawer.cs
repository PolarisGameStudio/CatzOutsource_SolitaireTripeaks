#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2013-2019 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

#if UNITY_EDITOR
namespace CodeStage.AntiCheat.EditorCode.PropertyDrawers
{
	using System.Reflection;
	using UnityEditor;

	internal class ObscuredPropertyDrawer : PropertyDrawer
	{
		protected MethodInfo boldFontMethodInfo;

		protected void SetBoldIfValueOverridePrefab(SerializedProperty parentProperty, SerializedProperty valueProperty)
		{
			if (parentProperty.isInstantiatedPrefab)
			{
				SetBoldDefaultFont(valueProperty.prefabOverride);
			}
		}

		protected void ResetBoldFont()
		{
			SetBoldDefaultFont(false);
		}

		protected void SetBoldDefaultFont(bool value)
		{
			if (boldFontMethodInfo == null)
			{
				boldFontMethodInfo = typeof (EditorGUIUtility).GetMethod("SetBoldDefaultFont", BindingFlags.Static | BindingFlags.NonPublic);
			}
			boldFontMethodInfo.Invoke(null, new[] {value as object});
		}
	}
}
#endif