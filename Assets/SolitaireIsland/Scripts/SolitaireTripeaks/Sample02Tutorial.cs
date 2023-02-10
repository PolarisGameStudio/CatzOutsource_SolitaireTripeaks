using Nightingale.Localization;
using System.Collections.Generic;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class Sample02Tutorial : MonoBehaviour
	{
		private CommonGuideUtility common;

		private void Awake()
		{
			HandCardSystem.Get().IsClickEnable = false;
			HandCardSystem.Get().PutHandNumber(new List<int>
			{
				25,
				22,
				51,
				20,
				39,
				11,
				37,
				6,
				7,
				45,
				38,
				50,
				4,
				1,
				26,
				8,
				10,
				52,
				44,
				29,
				21,
				18,
				43,
				14
			});
			PlayScene.Get().UndoButton.gameObject.SetActive(value: false);
			PlayScene.Get().RocketButton.gameObject.SetActive(value: false);
			PlayScene.Get().BoosterParentTransform.gameObject.SetActive(value: false);
			PlayScene.Get().SetCanvasGraphicRaycaster(enabled: false);
			PlayDesk.Get().OnPlayChanged.AddListener(delegate
			{
				common = CommonGuideUtility.CreateCommonGuideUtility(PlayScene.Get().transform);
				common.CreateGuide(LocalizationUtility.Get("Localization_help.json").GetString("help_play_multiple"), 0f, delegate
				{
					PlayScene.Get().SetCanvasGraphicRaycaster(enabled: true);
					TipPokerSystem.Get().OpenTip();
					PlayDesk.Get().OnDestopChanged.AddListener(OnDestopChanged);
					PlayStreaksSystem.Get().StreaksLevelChanged.AddListener(delegate
					{
						if (PlayStreaksSystem.Get().StreaksLevel >= 2)
						{
							PlayStreaksSystem.Get().StreaksLevelChanged.RemoveAllListeners();
							common.ChangeGuide(LocalizationUtility.Get("Localization_help.json").GetString("help_play_multiple_continue"));
						}
					});
					PlayStreaksSystem.Get().StreaksGradeChanged.AddListener(delegate
					{
						if (PlayStreaksSystem.Get().StreaksGrade == 1)
						{
							common.ChangeGuide(LocalizationUtility.Get("Localization_help.json").GetString("help_play_multiple_bonus"));
						}
						if (PlayStreaksSystem.Get().StreaksGrade >= 3)
						{
							PlayStreaksSystem.Get().StreaksGradeChanged.RemoveAllListeners();
							common.ChangeGuide(LocalizationUtility.Get("Localization_help.json").GetString("help_play_multiple_got_star"), 4f, delegate
							{
								common.CloseGuide();
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
			Invoke("OpenTip", 6f);
			if (TipPokerSystem.Get().OnlyHand() && !HandCardSystem.Get().IsClickEnable)
			{
				OpenTip();
				HandCardSystem.Get().IsClickEnable = true;
			}
		}

		private void OpenTip()
		{
			CancelInvoke("OpenTip");
			TipPokerSystem.Get().OpenTip();
		}
	}
}
