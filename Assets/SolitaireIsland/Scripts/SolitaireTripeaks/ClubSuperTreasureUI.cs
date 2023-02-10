using Nightingale.Extensions;
using Nightingale.Localization;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ClubSuperTreasureUI : MonoBehaviour
	{
		public Button CollectButton;

		public GameObject ButtonLabel;

		public RemainLabel RemainLabel;

		public GameObject LockObject;

		private void Awake()
		{
			UpdateUI();
		}

		private void UpdateUI()
		{
			CollectButton.onClick.RemoveAllListeners();
			LockObject.SetActive(value: false);
			switch (SingletonBehaviour<ClubSystemHelper>.Get().HasSuperTreasure())
			{
			default:
				ButtonLabel.SetActive(value: true);
				LockObject.SetActive(value: true);
				CollectButton.onClick.AddListener(delegate
				{
					LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Club.json");
					TipPopupNoIconScene.ShowTitleDescription(localizationUtility.GetString("SuperTreasure_Tips_Title"), localizationUtility.GetString("SuperTreasure_Tips_Lock_Description"));
				});
				break;
			case SuperTreasure.Collected:
				ButtonLabel.SetActive(value: false);
				CollectButton.interactable = false;
				SingletonBehaviour<GlobalConfig>.Get().SetColor(CollectButton.transform, Color.gray);
				RemainLabel.SetRemainTime(SingletonBehaviour<ClubSystemHelper>.Get().GetSuperRemain());
				RemainLabel.OnCompleted.RemoveAllListeners();
				RemainLabel.OnCompleted.AddListener(delegate
				{
					UpdateUI();
				});
				break;
			case SuperTreasure.Normal:
				ButtonLabel.SetActive(value: true);
				CollectButton.onClick.AddListener(delegate
				{
					if (SingletonBehaviour<ClubSystemHelper>.Get().ClubLeaderboardScore() > 688)
					{
						SingletonBehaviour<ClubSystemHelper>.Get().CollectSuperTreasure();
						Object.FindObjectsOfType<ClubButtonUI>().ForEach(delegate(ClubButtonUI e)
						{
							e.UpdateExclamationMark();
						});
						Object.FindObjectsOfType<ClubScene>().ForEach(delegate(ClubScene club)
						{
							if (club.StoreButton.gameObject.activeSelf)
							{
								SingletonBehaviour<GlobalConfig>.Get().CreateExclamationMark(club.StoreButton.gameObject, number: false);
							}
						});
						UpdateUI();
					}
					else
					{
						LocalizationUtility localizationUtility2 = LocalizationUtility.Get("Localization_Club.json");
						TipPopupNoIconScene.ShowTitleDescription(localizationUtility2.GetString("SuperTreasure_Tips_Title"), string.Format(localizationUtility2.GetString("SuperTreasure_Tips_Lock_ByScore_Description"), 688, SingletonBehaviour<ClubSystemHelper>.Get().ClubLeaderboardScore()));
					}
				});
				break;
			}
		}
	}
}
