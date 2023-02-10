using Nightingale.Localization;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class OfflineCoinsUI : MonoBehaviour
	{
		public Text FreeCoinLabel;

		public LocalizationLabel LimitCoinsLabel;

		public Text CollectButtonLabel;

		public Button CollectButton;

		public GameObject NormalObject;

		public GameObject LoadingObject;

		private string btn_collect_string = string.Empty;

		private void Start()
		{
			btn_collect_string = LocalizationUtility.Get("Localization_free.json").GetString("btn_collect");
			CollectButton.onClick.AddListener(CollectClick);
			InvokeRepeating("UpdateFreeCoinLabel", 0f, 1f);
		}

		private void OnApplicationFocus(bool focus)
		{
			if (!focus)
			{
				NormalObject.SetActive(value: false);
				LoadingObject.SetActive(value: true);
			}
		}

		private void UpdateFreeCoinLabel()
		{
			NormalObject.SetActive(SystemTime.IsConnect);
			LoadingObject.SetActive(!SystemTime.IsConnect);
			if (!NormalObject.activeSelf)
			{
				return;
			}
			LimitCoinsLabel.SetText(AuxiliaryData.Get().CoinsLimit());
			int coinBack = AuxiliaryData.Get().GetCoinBack();
			FreeCoinLabel.text = $"{coinBack:N0}";
			if (AuxiliaryData.Get().IsCollect())
			{
				if (!CollectButton.interactable)
				{
					CollectButton.interactable = true;
				}
				SingletonBehaviour<GlobalConfig>.Get().CreateExclamationMark(CollectButton.gameObject, number: true, left: false, 1f, -5f, -5f);
				CollectButtonLabel.text = btn_collect_string;
			}
			else
			{
				if (CollectButton.interactable)
				{
					CollectButton.interactable = false;
					SingletonBehaviour<GlobalConfig>.Get().CreateExclamationMark(CollectButton.gameObject, number: false, left: false, 1f, -5f, -5f);
				}
				CollectButtonLabel.text = AuxiliaryData.Get().GetTimeLeft();
			}
		}

		private void CollectClick()
		{
			if (AuxiliaryData.Get().IsCollect())
			{
				AuxiliaryData.Get().CollectCoins();
				SingletonBehaviour<EffectUtility>.Get().CreateCoinEffect(CollectButton.transform.position);
			}
		}
	}
}
