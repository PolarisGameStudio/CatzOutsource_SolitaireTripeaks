using Nightingale.Utilitys;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class SpecialActivityButton : MonoBehaviour
	{
		public GameObject SpecialActivityGameObject;

		public Button Button;

		public Text SpecialActivityRemainTimeLabel;

		public Image ButtonImage;

		private void Awake()
		{
			Button.onClick.AddListener(delegate
			{
				AuxiliaryData.Get().PutView("Special_Active");
				SingletonBehaviour<SpecialActivityUtility>.Get().ShowSpecialActivity(null);
			});
			ChangeSpecialActivity(SingletonBehaviour<SpecialActivityUtility>.Get().IsActive());
			InvokeRepeating("RepeatingUpdate", 0f, 1f);
		}

		private void ChangeSpecialActivity(bool state)
		{
			if (state)
			{
				SpecialActivityGameObject.SetActive(value: true);
				ButtonImage.sprite = SingletonBehaviour<SpecialActivityUtility>.Get().GetSprite();
				ButtonImage.SetNativeSize();
			}
			else
			{
				SpecialActivityGameObject.SetActive(value: false);
			}
			BankButtonUI.UUUUpdateUI();
		}

		private void RepeatingUpdate()
		{
			if (SingletonBehaviour<SpecialActivityUtility>.Get().IsActive() && !SpecialActivityGameObject.activeSelf)
			{
				ChangeSpecialActivity(state: true);
			}
			if (!SingletonBehaviour<SpecialActivityUtility>.Get().IsActive() && SpecialActivityGameObject.activeSelf)
			{
				ChangeSpecialActivity(state: false);
			}
			if (SpecialActivityGameObject.activeSelf)
			{
				SpecialActivityRemainTimeLabel.text = SpecialActivityConfig.Get().GetEndTime().Subtract(DateTime.Now)
					.TOString();
			}
		}
	}
}
