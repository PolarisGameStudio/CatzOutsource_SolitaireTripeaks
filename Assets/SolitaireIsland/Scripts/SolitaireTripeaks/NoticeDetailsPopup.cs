using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Toasts;
using Nightingale.Utilitys;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class NoticeDetailsPopup : SoundScene
	{
		public Text TitleLabel;

		public Text DescriptionLabel;

		public Text ButtonLabel;

		public Button NormalButton;

		public Button ReplyButton;

		public RectTransform Content;

		public RectTransform Layout;

		public void OnStart(MessageContent content, MessageData messageData, UnityAction unityAction)
		{
			TitleLabel.text = content.title;
			DescriptionLabel.text = content.details;
			string playerId = string.Empty;
			if (messageData.Tag.ToUpper() == "NOTICE")
			{
				playerId = Guid.Empty.ToString();
			}
			BonusInfo bonusInfo = TripeaksBonusScene.GetBonusInfo(content.code, playerId);
			switch (messageData.Tag.ToUpper())
			{
			case "REWARD":
				ShowBoosters(bonusInfo);
				UpdateNormalButton(bonusInfo);
				ReplyButton.gameObject.SetActive(value: false);
				break;
			case "RECOVER":
				ButtonLabel.text = LocalizationUtility.Get("Localization_report.json").GetString("btn_load_game_data");
				ReplyButton.gameObject.SetActive(value: false);
				break;
			case "TEXTONLY":
				UpdateNormalButton(bonusInfo);
				break;
			default:
				ShowBoosters(bonusInfo);
				UpdateNormalButton(bonusInfo);
				ReplyButton.gameObject.SetActive(value: false);
				break;
			}
			NormalButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Close();
				if (string.IsNullOrEmpty(content.synchronizeId))
				{
					if (unityAction != null)
					{
						unityAction();
					}
					if (!string.IsNullOrEmpty(content.code))
					{
						TripeaksBonusScene.Normal(content.code, playerId);
					}
				}
				else
				{
					LoadingHelper.Get("SynchronizeScene").ShowLoading(delegate
					{
					}, delegate(LoadingHelper helper, float total)
					{
						helper.StopLoading();
					}, LocalizationUtility.Get("Localization_LeaderBoard.json").GetString("loading_leaderboard_rewards"));
					SingletonBehaviour<SynchronizeUtility>.Get().SynchronizeGameData(content.synchronizeId, delegate
					{
						LoadingHelper.Get("SynchronizeScene").StopLoading();
						if (unityAction != null)
						{
							unityAction();
						}
					});
				}
			});
			ReplyButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Popup<ReportScene>("Scenes/ReportScene.prefab");
			});
		}

		private void ShowBoosters(BonusInfo bonusInfo)
		{
			if (bonusInfo != null)
			{
				Content.sizeDelta = new Vector2(850f, 450f);
				Layout.gameObject.SetActive(value: true);
				for (int i = 0; i < bonusInfo.commoditys.Length; i++)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>("UI/RewardBoosterNumberUI"));
					gameObject.transform.SetParent(Layout, worldPositionStays: false);
					gameObject.GetComponent<RewardBoosterNumberUI>().SetPurchasingCommodity(bonusInfo.commoditys[i]);
				}
			}
		}

		private void UpdateNormalButton(BonusInfo bonusInfo)
		{
			if (bonusInfo == null)
			{
				ButtonLabel.text = LocalizationUtility.Get("Localization_report.json").GetString("btn_ok");
			}
			else
			{
				ButtonLabel.text = ((bonusInfo.maxLevel <= 0) ? LocalizationUtility.Get("Localization_report.json").GetString("btn_collect") : LocalizationUtility.Get("Localization_report.json").GetString("btn_load_game_data"));
			}
		}
	}
}
