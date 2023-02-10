using Nightingale.Extensions;
using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using TriPeaks.ProtoData.Club;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ClubItemUI : MonoBehaviour
	{
		public Image StageIcon;

		public Image Icon;

		public Text NameLabel;

		public Text RankLabel;

		public Text ScoreLabel;

		public LocalizationLabel MembersLabel;

		public LocalizationLabel ClubLevelLabel;

		public Button Button;

		protected void ScrollCellContent(Club club)
		{
			Button.onClick.RemoveAllListeners();
			StageIcon.sprite = SingletonBehaviour<StageIconHelper>.Get().GetSprite(club.Stage, isClan: true);
			Icon.sprite = SingletonBehaviour<ClubSystemHelper>.Get().GetClubAvatar(club);
			ScoreLabel.text = club.LeaderboardScore.ToString();
			RankLabel.text = ((club.LeaderboardRank <= 0) ? "#" : club.LeaderboardRank.ToString());
			NameLabel.text = club.ClubName;
			ClubLevelLabel.SetText(club.Level);
			MembersLabel.SetText(club.MemberCount, club.MemberLimit, SingletonBehaviour<ClubSystemHelper>.Get().GetMembership(club.Private));
			Button.onClick.AddListener(delegate
			{
				Object.FindObjectsOfType<ClubScene>().ForEach(delegate(ClubScene e)
				{
					e.IsStay = false;
				});
				SingletonClass<MySceneManager>.Get().Popup<ClubScene>("Scenes/ClubScene").OnStart(club);
			});
		}
	}
}
