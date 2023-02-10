using DG.Tweening;
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
	public class LeaderboardRewardScene : BaseScene
	{
		public Button ContinueButton;

		public Text ButtonLabel;

		public LabelUI TitleLabel;

		public Text TypeLabel;

		public Image StageImage;

		public Transform ContentTransform;

		public Transform LayoutTransform;

		public void OnStart(bool isClan, RankRewardData rankRewardData, UnityAction unityAction)
		{
			List<PurchasingCommodity> commoditys = rankRewardData.commoditys;
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_LeaderBoard.json");
			bool rewarding = false;
			if (RankCoinData.Get().Staged <= rankRewardData.NextStage)
			{
				AudioUtility.GetSound().Play("Audios/Leaderboard_win.mp3");
			}
			else
			{
				AudioUtility.GetSound().Play("Audios/Leaderboard_lose.mp3");
			}
			if (commoditys == null || commoditys.Count == 0 || commoditys.Count((PurchasingCommodity e) => e.count == 0) == commoditys.Count)
			{
				TitleLabel.SetString(localizationUtility.GetString("Out_of_rank"));
				commoditys = new List<PurchasingCommodity>
				{
					new PurchasingCommodity
					{
						boosterType = BoosterType.Coins,
						count = 0
					}
				};
			}
			else
			{
				rewarding = true;
				if (rankRewardData.CurrentStage <= rankRewardData.NextStage)
				{
					Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(LeaderboardRewardScene).Name, "Particles/Ribbons"), base.transform);
					TitleLabel.SetString(localizationUtility.GetString("Congratulations"));
				}
				else
				{
					TitleLabel.SetString(localizationUtility.GetString("Oops"));
				}
			}
			TypeLabel.text = string.Format("{0}: #{1}", (!isClan) ? "You are ranked" : "Your Clan Ranked", (rankRewardData.rank <= 0) ? "5000+" : rankRewardData.rank.ToString());
			TypeLabel.transform.localPosition = Vector3.zero;
			TypeLabel.transform.localScale = Vector3.zero;
			TitleLabel.transform.localPosition = Vector3.zero;
			TitleLabel.transform.localScale = Vector3.zero;
			LayoutTransform.localPosition = Vector3.zero;
			StageImage.transform.localScale = Vector3.zero;
			StageImage.transform.localPosition = new Vector3(0f, -220f, 0f);
			ContinueButton.gameObject.SetActive(value: false);
			ContinueButton.transform.localScale = Vector3.zero;
			ButtonLabel.text = localizationUtility.GetString("btn_Continue");
			Sequence s = DOTween.Sequence();
			s.Append(TitleLabel.transform.DOScale(1.2f, 0.2f));
			s.Append(TitleLabel.transform.DOScale(1f, 0.1f));
			s.AppendInterval(0.4f);
			s.Append(TitleLabel.transform.DOLocalMoveY(500f, 0.3f));
			s.Append(TitleLabel.transform.DOLocalMoveY(450f, 0.1f));
			s.AppendInterval(0.4f);
			s.Append(TypeLabel.transform.DOScale(1.2f, 0.2f));
			s.Append(TypeLabel.transform.DOScale(1f, 0.1f));
			s.AppendInterval(0.4f);
			s.Append(TypeLabel.transform.DOLocalMoveY(350f, 0.3f));
			s.Append(TypeLabel.transform.DOLocalMoveY(310f, 0.1f));
			GameObject asset = SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(LeaderboardRewardScene).Name, "UI/RewardBoosterNumberUI");
			for (int i = 0; i < commoditys.Count; i++)
			{
				GameObject gameObject = Object.Instantiate(asset);
				gameObject.transform.SetParent(ContentTransform, worldPositionStays: false);
				gameObject.GetComponent<RewardBoosterNumberUI>().SetPurchasingCommodity(commoditys[i]);
				gameObject.transform.localScale = Vector3.zero;
				s.Append(gameObject.transform.DOScale(1.2f, 0.2f));
				s.Append(gameObject.transform.DOScale(1f, 0.1f));
			}
			s.AppendInterval(0.4f);
			s.Append(LayoutTransform.DOLocalMoveY(130f, 0.3f));
			s.Append(LayoutTransform.DOLocalMoveY(110f, 0.1f));
			s.AppendCallback(delegate
			{
				StageImage.sprite = SingletonBehaviour<StageIconHelper>.Get().GetSprite((int)rankRewardData.CurrentStage, isClan);
			});
			s.Append(StageImage.transform.DOScale(1.2f, 0.2f));
			s.Append(StageImage.transform.DOScale(1f, 0.1f));
			if (rankRewardData.CurrentStage != rankRewardData.NextStage)
			{
				s.AppendInterval(0.5f);
				if (rankRewardData.CurrentStage < rankRewardData.NextStage)
				{
					s.AppendCallback(delegate
					{
						AudioUtility.GetSound().Play("Audios/StageUpgrade.wav");
						GameObject gameObject2 = Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(LeaderboardRewardScene).Name, "Particles/StageUpgrade"));
						gameObject2.transform.SetParent(base.transform, worldPositionStays: false);
						gameObject2.transform.position = StageImage.transform.position;
					});
				}
				s.Append(StageImage.transform.DOScaleX(0f, 0.2f));
				s.AppendCallback(delegate
				{
					StageImage.sprite = SingletonBehaviour<StageIconHelper>.Get().GetSprite((int)rankRewardData.NextStage, isClan);
				});
				s.Append(StageImage.transform.DOScaleX(1f, 0.2f));
			}
			s.AppendCallback(delegate
			{
				ContinueButton.gameObject.SetActive(value: true);
			});
			s.Append(ContinueButton.transform.DOScale(1f, 0.5f));
			ContinueButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Close(new NavigationEffect());
				rankRewardData.reward = true;
				SingletonBehaviour<LeaderBoardUtility>.Get().RewardChanged.Invoke();
				if (rewarding)
				{
					foreach (PurchasingCommodity item in commoditys)
					{
						SessionData.Get().PutCommodity(item.boosterType, CommoditySource.Rank, item.count, changed: false);
					}
					PurchasSuccessPopup.ShowPurchasSuccessPopup(commoditys.ToArray(), unityAction);
				}
				else if (unityAction != null)
				{
					unityAction();
				}
			});
		}
	}
}
