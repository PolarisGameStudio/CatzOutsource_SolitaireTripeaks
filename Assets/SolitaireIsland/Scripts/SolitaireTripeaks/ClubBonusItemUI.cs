using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ClubBonusItemUI : MonoBehaviour
	{
		public GameObject CompletedGameObject;

		public GameObject LockGameObject;

		public GameObject NormalGameObject;

		public ProgressBarUI ProgressBarUI;

		public Text ProgressBarLabel;

		public Button Button;

		public void SetInfo(int level, int index, long score, long minScore, long maxScore)
		{
			CompletedGameObject.SetActive(level > index);
			LockGameObject.SetActive(level < index);
			NormalGameObject.SetActive(level == index);
			if (NormalGameObject.activeSelf)
			{
				float num = (float)(score - minScore) / (float)(maxScore - minScore);
				ProgressBarLabel.text = $"{num:0.00%}";
				ProgressBarUI.SetFillAmount(num);
				Button.onClick.AddListener(delegate
				{
					JoinPlayHelper.JoinPlayByQuest(PlayData.Get().GetMaxPlayScheduleData());
				});
			}
		}
	}
}
