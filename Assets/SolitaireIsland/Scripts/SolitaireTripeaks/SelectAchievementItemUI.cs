using Nightingale.Localization;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class SelectAchievementItemUI : MonoBehaviour
	{
		public Text AchievementTitle;

		public Text AchievementDescription;

		public Text AchievementProgress;

		public Image AchievementFillProgress;

		public GameObject AchievementProgressGameObject;

		public Button SelectButton;

		private AchievementInfo _AchievementInfo;

		private GameObject _selectGameObject;

		private void Awake()
		{
			_selectGameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(AchievementScene).Name, "UI/AchievementSelect"));
			SelectButton.onClick.AddListener(SelectAvatrt);
		}

		private void SelectAvatrt()
		{
			SingletonBehaviour<TripeaksPlayerHelper>.Get().GetSelf().SetAvatar(_AchievementInfo.GetConfig().AvaterFileName);
			SingletonBehaviour<ClubSystemHelper>.Get().Profile(AuxiliaryData.Get().GetNickName(), AuxiliaryData.Get().AvaterFileName);
			Object.FindObjectOfType<AchievementScene>().UpdateRestButton();
		}

		public void SetSelectAchievementInfo(AchievementItemUI achievementItemUI, AchievementInfo data)
		{
			_selectGameObject.transform.SetParent(achievementItemUI.transform, worldPositionStays: false);
			_selectGameObject.transform.SetAsLastSibling();
			_AchievementInfo = data;
			AchievementProgressGameObject.SetActive(value: false);
			SelectButton.gameObject.SetActive(value: false);
			if (data == null)
			{
				AchievementTitle.text = LocalizationUtility.Get("Localization_achievement.json").GetString("Coming Soon").ToUpper();
				AchievementDescription.text = LocalizationUtility.Get("Localization_achievement.json").GetString("Coming Soon");
				return;
			}
			AchievementTitle.text = LocalizationUtility.Get("Localization_achievement.json").GetString(data.GetTitle()).ToUpper();
			AchievementDescription.text = data.GetDescription();
			if (data.IsActive() && !data.IsComplete())
			{
				AchievementProgressGameObject.gameObject.SetActive(value: true);
				AchievementProgress.text = data.GetProgressString();
				AchievementFillProgress.fillAmount = data.GetProgress();
			}
			if (data.IsComplete())
			{
				SelectButton.gameObject.SetActive(value: true);
			}
		}
	}
}
