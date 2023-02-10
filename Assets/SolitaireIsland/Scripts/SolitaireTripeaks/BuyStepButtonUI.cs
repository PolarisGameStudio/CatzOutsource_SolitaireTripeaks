using Nightingale.Localization;
using Nightingale.UIExtensions;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class BuyStepButtonUI : MonoBehaviour
	{
		public Button BuyStepButton;

		public CashText BuyStepLabel;

		public LocalizationLabel BuyStepRatioLabel;

		public GameObject DoubleStepGameObject;

		public GameObject NormalStepGameObject;

		private void Start()
		{
			bool doubleBuyStep = SingletonClass<DayActivityHelper>.Get().HasDayActivty(DayActivityType.DoubleBuyStep);
			DoubleStepGameObject.SetActive(doubleBuyStep);
			NormalStepGameObject.SetActive(!doubleBuyStep);
			UpdateUI();
			BuyStepButton.onClick.AddListener(delegate
			{
				if (PlayDesk.Get().IsPlaying)
				{
					int num = Mathf.CeilToInt((float)ScoringSystem.Get().BuyStepCoins * (1f - SingletonBehaviour<ClubSystemHelper>.Get().GetSkillRatioCoins(ClubSkill.Hand)));
					if (SessionData.Get().UseCommodity(BoosterType.Coins, num, "BuyStepCoins"))
					{
						SingletonClass<OnceGameData>.Get().BuyStepCoins += num;
						SingletonClass<OnceGameData>.Get().Use(BoosterType.BuyStep);
						HandCardSystem.Get().AppendLeftCards((!doubleBuyStep) ? 5 : 10);
						HandCardSystem.Get().UpdateNeedHasCard();
						ScoringSystem.Get().BuyStep();
						SingletonBehaviour<ClubSystemHelper>.Get().ClubSkillOnce(ClubSkill.Hand);
						UpdateUI();
						OperatingHelper.Get().ClearStep();
					}
					else
					{
						StoreScene.ShowOutofCoins();
					}
				}
			});
		}

		private void UpdateUI()
		{
			float skillRatioCoins = SingletonBehaviour<ClubSystemHelper>.Get().GetSkillRatioCoins(ClubSkill.Hand);
			int num = Mathf.CeilToInt((float)ScoringSystem.Get().BuyStepCoins * (1f - skillRatioCoins));
			BuyStepLabel.SetSmallCash(num);
			if (skillRatioCoins > 0f)
			{
				BuyStepRatioLabel.gameObject.SetActive(value: true);
				BuyStepRatioLabel.SetText(skillRatioCoins);
			}
			else
			{
				BuyStepRatioLabel.gameObject.SetActive(value: false);
			}
		}
	}
}
