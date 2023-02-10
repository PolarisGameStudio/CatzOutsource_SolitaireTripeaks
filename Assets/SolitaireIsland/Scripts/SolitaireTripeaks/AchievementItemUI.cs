using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class AchievementItemUI : MonoBehaviour
	{
		public FriendAvaterUI FriendAvaterUI;

		public Image ProgressImage;

		public Transform ProgressTransform;

		public void SetAchievementData(AchievementInfo data, UnityAction<AchievementItemUI, AchievementInfo> selectUnityAction)
		{
			ProgressTransform.gameObject.SetActive(value: false);
			Button component = GetComponent<Button>();
			component.onClick.AddListener(delegate
			{
				if (selectUnityAction != null)
				{
					selectUnityAction(this, data);
				}
			});
			if (data != null)
			{
				if (data.IsComplete())
				{
					FriendAvaterUI.SetUser(data.GetConfig().AvaterFileName);
				}
				else
				{
					FriendAvaterUI.AvaterImage.color = Color.gray;
					FriendAvaterUI.AvaterImage.sprite = SingletonClass<AvaterUtility>.Get().GetAvater(data.GetConfig().AvaterFileName);
				}
				if (data.IsActive() && !data.IsComplete())
				{
					ProgressTransform.gameObject.SetActive(value: true);
					ProgressImage.fillAmount = data.GetProgress();
				}
			}
		}
	}
}
