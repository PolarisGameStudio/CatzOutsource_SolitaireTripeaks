using Nightingale.Localization;
using System.Collections.Generic;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class Sample01Tutorial : MonoBehaviour
	{
		private bool handing;

		private int index;

		private CommonGuideUtility common;

		private void Awake()
		{
			PackData.Get().GetCommodity(BoosterType.Coins).Set(CommoditySource.Free, 13000);
			HandCardSystem.Get().IsClickEnable = false;
			HandCardSystem.Get().PutHandNumber(new List<int>
			{
				46,
				23,
				27,
				37,
				9,
				13,
				1,
				49,
				50,
				33,
				26,
				15,
				40,
				11,
				28,
				21,
				38,
				29,
				8,
				4,
				12,
				36,
				45,
				32
			});
			PlayScene.Get().UndoButton.gameObject.SetActive(value: false);
			PlayScene.Get().RocketButton.gameObject.SetActive(value: false);
			PlayScene.Get().WildButton.gameObject.SetActive(value: false);
			PlayScene.Get().BoosterParentTransform.gameObject.SetActive(value: false);
			PlayScene.Get().SetCanvasGraphicRaycaster(enabled: false);
			PlayDesk.Get().OnPlayChanged.AddListener(delegate
			{
				common = CommonGuideUtility.CreateCommonGuideUtility(PlayScene.Get().transform);
				common.CreateGuide(LocalizationUtility.Get("Localization_help.json").GetString("help_first_tap"), 0f, delegate
				{
					PlayScene.Get().SetCanvasGraphicRaycaster(enabled: true);
					TipPokerSystem.Get().OpenTip();
					PlayDesk.Get().OnClickCardChanged.AddListener(delegate(bool isDesk)
					{
						if (isDesk)
						{
							TipPokerSystem.Get().HideTip();
							PlayDesk.Get().OnClickCardChanged.RemoveAllListeners();
							common.ChangeGuide(LocalizationUtility.Get("Localization_help.json").GetString("help_first_tap_good"), 0f, delegate
							{
								OnDestopChanged();
								PlayDesk.Get().OnDestopChanged.AddListener(OnDestopChanged);
							});
						}
					});
				});
			});
		}

		private void OnDestroy()
		{
			TipPokerSystem.Get().HideTip();
		}

		private void OnDestopChanged()
		{
			TipPokerSystem.Get().HideTip();
			CancelInvoke("OpenTip");
			Invoke("OpenTip", (index >= 1) ? 6f : 2f);
			index++;
			if (!handing && TipPokerSystem.Get().OnlyHand())
			{
				handing = true;
				OpenTip();
				common.ChangeGuide(LocalizationUtility.Get("Localization_help.json").GetString("help_tap_stock"), 0f, delegate
				{
					HandCardSystem.Get().IsClickEnable = true;
					PlayDesk.Get().OnClickCardChanged.AddListener(delegate
					{
						PlayDesk.Get().OnClickCardChanged.RemoveAllListeners();
						common.ChangeGuide(LocalizationUtility.Get("Localization_help.json").GetString("help_got_it"), 3f, delegate
						{
							common.CloseGuide();
						});
					});
				});
			}
		}

		private void OpenTip()
		{
			CancelInvoke("OpenTip");
			TipPokerSystem.Get().OpenTip();
		}
	}
}
