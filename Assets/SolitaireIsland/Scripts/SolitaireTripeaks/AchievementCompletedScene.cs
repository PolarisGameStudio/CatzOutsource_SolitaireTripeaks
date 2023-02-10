using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class AchievementCompletedScene : BaseScene
	{
		public Transform NumberTransform;

		public Text TitleLabel;

		public Text NumberLabel;

		public Text ButtonLabel;

		public Image Avater;

		public Button _NextButton;

		public Transform AnimatorTransform;

		public static void TryShow()
		{
			List<AchievementInfo> list = (from e in AchievementData.Get().AchievementDatas
				where !e.IsTips && e.IsComplete()
				select e).ToList();
			if (list.Count > 0)
			{
				SingletonClass<MySceneManager>.Get().Popup<AchievementCompletedScene>("Scenes/AchievementCompletedScene").OnStart(list);
			}
		}

		private void OnStart(List<AchievementInfo> completeds)
		{
			UnityAction unityAction = delegate
			{
				if (completeds.Count > 0)
				{
					AchievementInfo info = completeds[0];
					completeds.RemoveAt(0);
					UpdateAchievementInfoUI(info, completeds);
				}
				else
				{
					SingletonClass<MySceneManager>.Get().Close();
				}
			};
			unityAction();
			_NextButton.onClick.AddListener(unityAction);
			AudioUtility.GetSound().Play("Audios/pop_Scene.mp3");
		}

		public void OnCloseClick()
		{
			List<AchievementInfo> list = (from e in AchievementData.Get().AchievementDatas
				where !e.IsTips && e.IsComplete()
				select e).ToList();
			foreach (AchievementInfo item in list)
			{
				item.IsTips = true;
			}
			AchievementData.Get().DOChanged();
			SingletonClass<MySceneManager>.Get().Close();
			AudioUtility.GetSound().Play("Audios/close_scene.mp3");
		}

		private void UpdateAchievementInfoUI(AchievementInfo info, List<AchievementInfo> completeds)
		{
			info.IsTips = true;
			AchievementData.Get().DOChanged();
			NumberTransform.gameObject.SetActive(completeds.Count > 0);
			NumberLabel.text = completeds.Count.ToString();
			TitleLabel.text = LocalizationUtility.Get("Localization_achievement.json").GetString(info.GetTitle()).ToUpper();
			Avater.sprite = SingletonClass<AvaterUtility>.Get().GetAvater(info.GetConfig().AvaterFileName);
			Avater.SetNativeSize();
			if (completeds.Count > 0)
			{
				ButtonLabel.text = LocalizationUtility.Get("Localization_achievement.json").GetString("btn_next");
			}
			else
			{
				ButtonLabel.text = LocalizationUtility.Get("Localization_achievement.json").GetString("btn_ok");
			}
		}
	}
}
