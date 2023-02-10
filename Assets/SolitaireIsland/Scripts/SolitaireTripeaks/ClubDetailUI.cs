using Nightingale.Localization;
using Nightingale.Utilitys;
using TriPeaks.ProtoData.Club;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ClubDetailUI : MonoBehaviour
	{
		public Image Icon;

		public Image StageImage;

		public Text NameLabel;

		public Text DescriptionLabel;

		public Text MembershipLabel;

		public Text ScoreLabel;

		public Text NumberLabel;

		public LocalizationLabel ClubLevelLabel;

		public Image ProgressBarUI;

		public void SetInfo(Club clubData)
		{
			if (clubData == null)
			{
				return;
			}
			StageImage.sprite = SingletonBehaviour<StageIconHelper>.Get().GetSprite(clubData.Stage, isClan: true);
			StageImage.SetNativeSize();
			Icon.sprite = SingletonBehaviour<ClubSystemHelper>.Get().GetClubAvatar(clubData);
			NameLabel.text = clubData.ClubName;
			DescriptionLabel.text = clubData.ClubDescription;
			MembershipLabel.text = SingletonBehaviour<ClubSystemHelper>.Get().GetMembership(clubData.Private);
			ClubLevelLabel.SetText(clubData.Level);
			if (SingletonBehaviour<ClubSystemHelper>.Get()._MyClubResponse != null)
			{
				ClubLevelConfig clubLevelConfig = SingletonBehaviour<ClubSystemHelper>.Get()._MyClubResponse.LevelConfigs.ToList().Find((ClubLevelConfig e) => e.Level == clubData.Level);
				if (clubLevelConfig == null || clubData.Level == 10)
				{
					ProgressBarUI.fillAmount = 1f;
				}
				else
				{
					ProgressBarUI.fillAmount = (float)(clubData.Score - clubLevelConfig.MinScore) / (float)(clubLevelConfig.MaxScore - clubLevelConfig.MinScore);
				}
			}
			ScoreLabel.text = clubData.LeaderboardScore.ToString();
			NumberLabel.text = $"{clubData.MemberCount}/{clubData.MemberLimit}";
		}
	}
}
