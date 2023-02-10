using DG.Tweening;
using Nightingale.ScenesManager;
using Nightingale.Socials;
using Nightingale.Utilitys;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class AchievementScene : SoundScene
	{
		public Button ClosedButton;

		public Button RestButton;

		public Transform ContentTransform;

		public RectTransform AvaterTransform;

		public FriendAvaterUI FriendAvaterUI;

		public SelectAchievementItemUI selectAchievementItemUI;

		protected override void OnDestroy()
		{
			base.OnDestroy();
			SingletonBehaviour<LoaderUtility>.Get().UnLoadScene(typeof(AchievementScene).Name);
		}

		public override void OnSceneStateChanged(SceneState state)
		{
			base.OnSceneStateChanged(state);
			if (state == SceneState.Closing)
			{
				AvaterTransform.DOAnchorPosY(500f, 0.3f);
			}
		}

		private void Start()
		{
			base.IsStay = true;
			Sequence s = DOTween.Sequence();
			s.AppendInterval(0.5f);
			s.Append(AvaterTransform.DOAnchorPosY(-100f, 0.3f));
			s.Append(AvaterTransform.DOAnchorPosY(-85f, 0.1f));
			CreateView(AchievementData.Get().AchievementDatas);
			ClosedButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Close();
			});
			RestButton.gameObject.SetActive(SingletonBehaviour<FacebookMananger>.Get().IsLogin());
			RestButton.onClick.AddListener(delegate
			{
				SingletonBehaviour<TripeaksPlayerHelper>.Get().GetSelf().SetAvatar(string.Empty);
				SingletonBehaviour<ClubSystemHelper>.Get().Profile(AuxiliaryData.Get().GetNickName(), AuxiliaryData.Get().AvaterFileName);
				UpdateRestButton();
			});
			UpdateRestButton();
			FriendAvaterUI.SetUser(SingletonBehaviour<TripeaksPlayerHelper>.Get().GetSelf());
			AchievementCompletedScene.TryShow();
		}

		private void CreateView(List<AchievementInfo> achievementDatas)
		{
			bool flag = true;
			GameObject asset = SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(AchievementScene).Name, "UI/AchievementItemUI");
			achievementDatas = (from e in achievementDatas
				orderby e.GetConfig().OrderIndex
				select e).ToList();
			foreach (AchievementInfo achievementData in achievementDatas)
			{
				AchievementConfig config = achievementData.GetConfig();
				if ((config.achievementType != AchievementType.CompeletedChapter && config.achievementType != AchievementType.CollectedAllStarsInChapter) || UniverseConfig.Get().GetChapterConfig(config.scheduleData.world, config.scheduleData.chapter) != null)
				{
					AchievementItemUI component = Object.Instantiate(asset).GetComponent<AchievementItemUI>();
					component.SetAchievementData(achievementData, selectAchievementItemUI.SetSelectAchievementInfo);
					component.transform.SetParent(ContentTransform, worldPositionStays: false);
					if (flag)
					{
						selectAchievementItemUI.SetSelectAchievementInfo(component, achievementData);
						flag = false;
					}
				}
			}
		}

		public void UpdateRestButton()
		{
			RestButton.interactable = !string.IsNullOrEmpty(AuxiliaryData.Get().AvaterFileName);
		}
	}
}
