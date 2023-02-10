using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class SpecialActivityScene : SoundScene
	{
		public Transform ContentTransform;

		public Image IconImage;

		public Image SmallIconImage;

		public Image FindIconImage;

		public Text SpecialActivityNumberLabel;

		public Text RemainTimeLabel;

		public Button CloseButton;

		public Button FindButton;

		public Text DesLabel;

		private void Awake()
		{
			base.IsStay = true;
			SpecialActivityData.Get().OpenSpecialNumbers++;
			UpdateSpecialActivityNumberUI();
			IconImage.sprite = SingletonBehaviour<SpecialActivityUtility>.Get().GetSprite();
			IconImage.SetNativeSize();
			SmallIconImage.sprite = SingletonBehaviour<SpecialActivityUtility>.Get().GetMiniSprite();
			SmallIconImage.SetNativeSize();
			FindIconImage.sprite = SingletonBehaviour<SpecialActivityUtility>.Get().GetMiniSprite();
			FindIconImage.SetNativeSize();
			IEnumerable<ExchangeConfig> enumerable = from e in SpecialActivityConfig.Get().Configs
				where e.IsActive()
				select e;
			foreach (ExchangeConfig item in enumerable)
			{
				SpecialActivityExchangeUI.Create(ContentTransform, item, delegate
				{
					UpdateSpecialActivityNumberUI();
				});
			}
			CloseButton.onClick.AddListener(delegate
			{
				SpecialActivityButton specialActivityButton = UnityEngine.Object.FindObjectOfType<SpecialActivityButton>();
				if (specialActivityButton != null)
				{
					SingletonClass<MySceneManager>.Get().Close(new PivotScaleEffect(specialActivityButton.transform.position));
				}
				else
				{
					SingletonClass<MySceneManager>.Get().Close();
				}
			});
			SingletonBehaviour<GlobalConfig>.Get().CreateNumber(FindButton.gameObject, 0.8f, SpecialActivityData.Get().ScheduleDatas.Count, left: false, -10f, -10f);
			if (SpecialActivityConfig.Get().IsActive() && SpecialActivityData.Get().ScheduleDatas.Count > 0)
			{
				FindButton.interactable = true;
				SingletonBehaviour<GlobalConfig>.Get().SetColor(FindButton.transform, Color.white);
			}
			else
			{
				FindButton.interactable = false;
				SingletonBehaviour<GlobalConfig>.Get().SetColor(FindButton.transform, Color.gray);
			}
			FindButton.onClick.AddListener(delegate
			{
				if (SpecialActivityConfig.Get().IsActive() && SpecialActivityData.Get().ScheduleDatas.Count > 0)
				{
					LeveEndScene levelEndScene = UnityEngine.Object.FindObjectOfType<LeveEndScene>();
					if (levelEndScene != null)
					{
						SingletonClass<MySceneManager>.Get().Close(null, delegate
						{
							levelEndScene.CloseAnimator(delegate
							{
								QuestsScene.TryShow(delegate
								{
									JoinPlayHelper.JoinPlayByQuest(SpecialActivityData.Get().RandomScheduleData());
								});
							});
						});
					}
					else
					{
						JoinPlayHelper.JoinPlayByQuest(SpecialActivityData.Get().RandomScheduleData());
					}
				}
			});
			DesLabel.text = string.Format(LocalizationUtility.Get("Localization_SpecialActivity.json").GetString("SpecialActivity_Des"), SpecialActivityConfig.Get().Name);
			InvokeRepeating("RepeatingUpdate", 0f, 1f);
		}

		private void RepeatingUpdate()
		{
			RemainTimeLabel.text = SpecialActivityConfig.Get().GetEndTime().Subtract(DateTime.Now)
				.TOString();
		}

		private void UpdateSpecialActivityNumberUI()
		{
			SpecialActivityNumberLabel.text = $"{SpecialActivityData.Get().Numbers}";
		}
	}
}
