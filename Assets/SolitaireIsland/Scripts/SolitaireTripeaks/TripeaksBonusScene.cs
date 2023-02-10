using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class TripeaksBonusScene : BaseScene
	{
		public InputField InputField;

		public Button SureButton;

		public Text SureButtonLabel;

		public Button CloseButton;

		private void Awake()
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SureButtonLabel.text = localizationUtility.GetString("btn_ok");
			SureButton.onClick.AddListener(delegate
			{
				Normal(InputField.text, string.Empty);
			});
			CloseButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Close();
			});
		}

		public static void Normal(string systemCopyBuffer, string playerId = "")
		{
			if (!Silent(systemCopyBuffer, playerId))
			{
				TipPopupNoIconScene.ShowBonusErrorCode();
			}
		}

		public static bool Silent(string systemCopyBuffer, string playerId = "")
		{
			if (!string.IsNullOrEmpty(systemCopyBuffer))
			{
				if (string.IsNullOrEmpty(playerId))
				{
					playerId = SolitaireTripeaksData.Get().GetPlayerId();
				}
				if (systemCopyBuffer.Contains("["))
				{
					systemCopyBuffer = systemCopyBuffer.Substring(systemCopyBuffer.IndexOf("[") + 1);
				}
				if (systemCopyBuffer.Contains("]"))
				{
					systemCopyBuffer = systemCopyBuffer.Substring(0, systemCopyBuffer.LastIndexOf("]"));
				}
				if (!AuxiliaryData.Get().HasView(systemCopyBuffer))
				{
					try
					{
						AuxiliaryData.Get().PutView(systemCopyBuffer);
						systemCopyBuffer = EncryptionUtility.Decrypt(playerId, systemCopyBuffer);
						BonusInfo info = JsonUtility.FromJson<BonusInfo>(systemCopyBuffer);
						UnityAction unityAction = delegate
						{
							if (info.maxLevel > 0)
							{
								List<ScheduleData> allScheduleDatas = UniverseConfig.Get().GetAllScheduleDatas();
								foreach (ScheduleData item in allScheduleDatas)
								{
									int levels = UniverseConfig.Get().GetLevels(item);
									if (levels < info.maxLevel)
									{
										if (levels < 3)
										{
											PlayData.Get().PutLevelData(item, new LevelData
											{
												StarComplete = true,
												StarSteaks = true,
												StarTime = true
											});
										}
										else
										{
											PlayData.Get().PutLevelData(item, new LevelData
											{
												StarComplete = true,
												StarSteaks = (UnityEngine.Random.Range(0, 10) % 2 == 0),
												StarTime = (UnityEngine.Random.Range(0, 10) % 2 == 0)
											});
										}
									}
								}
								SingletonClass<MySceneManager>.Get().Navigation<LoadingScene>("Scenes/LoadingScene");
							}
							if (!string.IsNullOrEmpty(info.playerId))
							{
								SolitaireTripeaksData.Get().PlayerId = info.playerId;
							}
						};
						if (info.commoditys.Length > 0)
						{
							PurchasingCommodity[] commoditys = info.commoditys;
							foreach (PurchasingCommodity purchasingCommodity in commoditys)
							{
								SessionData.Get().PutCommodity(purchasingCommodity.boosterType, CommoditySource.Free, purchasingCommodity.count, changed: false);
							}
							PurchasSuccessPopup.ShowPurchasSuccessPopup(info.commoditys, unityAction);
						}
						else
						{
							unityAction();
						}
						return true;
					}
					catch (Exception ex)
					{
						UnityEngine.Debug.Log(ex.Message);
					}
				}
			}
			return false;
		}

		public static BonusInfo GetBonusInfo(string systemCopyBuffer, string playerId = "")
		{
			if (!string.IsNullOrEmpty(systemCopyBuffer))
			{
				if (string.IsNullOrEmpty(playerId))
				{
					playerId = SolitaireTripeaksData.Get().GetPlayerId();
				}
				if (systemCopyBuffer.Contains("["))
				{
					systemCopyBuffer = systemCopyBuffer.Substring(systemCopyBuffer.IndexOf("[") + 1);
				}
				if (systemCopyBuffer.Contains("]"))
				{
					systemCopyBuffer = systemCopyBuffer.Substring(0, systemCopyBuffer.LastIndexOf("]"));
				}
				try
				{
					systemCopyBuffer = EncryptionUtility.Decrypt(playerId, systemCopyBuffer);
					return JsonUtility.FromJson<BonusInfo>(systemCopyBuffer);
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.Log(ex.Message);
				}
			}
			return null;
		}
	}
}
