using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace TripeaksTools
{
	public class DiffUtilityScene : MonoBehaviour
	{
		private string calcNumberLabel;

		private string handNumberLabel;

		private bool customize;

		private bool isExpert;

		private string StartLevelLabel;

		private string EndLevelLabel;

		private string LevelsLabel;

		private float calcProcess = 1f;

		[CompilerGenerated]
		private static Func<string, int> _003C_003Ef__mg_0024cache0;

		private void OnGUI()
		{
			int calcCount = 0;
			int handNumber = 0;
			GUI.skin.button.fontSize = 50;
			GUI.skin.button.fontStyle = FontStyle.Normal;
			GUI.skin.textField.fontSize = 50;
			GUI.skin.label.fontSize = 50;
			GUILayout.BeginVertical();
			GUILayout.Label("减少手牌数目：");
			handNumberLabel = GUILayout.TextField(handNumberLabel);
			if (!string.IsNullOrEmpty(handNumberLabel))
			{
				handNumber = int.Parse(handNumberLabel);
			}
			GUILayout.Label("跑动次数");
			calcNumberLabel = GUILayout.TextField(calcNumberLabel);
			if (!string.IsNullOrEmpty(calcNumberLabel))
			{
				calcCount = int.Parse(calcNumberLabel);
			}
			if (GUILayout.Button((!customize) ? "自定义关卡已关闭" : "自定义关卡已开启"))
			{
				customize = !customize;
			}
			if (GUILayout.Button((!isExpert) ? "精英关卡已关闭" : "精英关卡已开启"))
			{
				isExpert = !isExpert;
			}
			List<int> levels = new List<int>();
			int startLevel = 0;
			int endLevel = 0;
			if (customize)
			{
				LevelsLabel = GUILayout.TextField(LevelsLabel);
				if (!string.IsNullOrEmpty(LevelsLabel))
				{
					try
					{
						levels = LevelsLabel.Split(',').ToList().Select(int.Parse)
							.ToList();
					}
					catch (Exception ex)
					{
						UnityEngine.Debug.Log(ex.Message);
					}
				}
			}
			else
			{
				GUILayout.Label("连续关卡（开始）");
				StartLevelLabel = GUILayout.TextField(StartLevelLabel);
				if (!string.IsNullOrEmpty(StartLevelLabel))
				{
					startLevel = int.Parse(StartLevelLabel);
				}
				GUILayout.Label("连续关卡（结束）");
				EndLevelLabel = GUILayout.TextField(EndLevelLabel);
				if (!string.IsNullOrEmpty(EndLevelLabel))
				{
					endLevel = int.Parse(EndLevelLabel);
				}
			}
			if (calcProcess == 1f && GUILayout.Button("开始跑动"))
			{
				calcProcess = 0f;
				DiffUtility diffUtility = base.gameObject.AddComponent<DiffUtility>();
				diffUtility.customize = customize;
				diffUtility.isExpert = isExpert;
				diffUtility.Levels = levels;
				diffUtility.StartLevel = startLevel;
				diffUtility.EndLevel = endLevel;
				diffUtility.CalcCount = calcCount;
				diffUtility.HandNumber = handNumber;
				diffUtility.OnLoading.AddListener(delegate(float pr)
				{
					calcProcess = pr;
				});
			}
			GUILayout.Label("计算进度:" + calcProcess);
			GUILayout.EndVertical();
		}

		public static void OnSpace()
		{
			DiffUtilityScene diffUtilityScene = UnityEngine.Object.FindObjectOfType<DiffUtilityScene>();
			if (diffUtilityScene == null)
			{
				GameObject gameObject = new GameObject("Diff Utility Scene");
				gameObject.AddComponent<DiffUtilityScene>();
				GameObject gameObject2 = new GameObject("Canvas");
				gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
				Canvas canvas = gameObject2.AddComponent<Canvas>();
				canvas.renderMode = RenderMode.ScreenSpaceOverlay;
				canvas.sortingOrder = int.MaxValue;
				gameObject2.AddComponent<GraphicRaycaster>();
				GameObject gameObject3 = new GameObject("Image");
				gameObject3.transform.SetParent(gameObject2.transform, worldPositionStays: false);
				Image image = gameObject3.AddComponent<Image>();
				image.rectTransform.anchorMin = new Vector2(0f, 0f);
				image.rectTransform.anchorMax = new Vector2(1f, 1f);
				image.color = Color.black;
			}
			else
			{
				UnityEngine.Object.Destroy(diffUtilityScene.gameObject);
			}
		}
	}
}
