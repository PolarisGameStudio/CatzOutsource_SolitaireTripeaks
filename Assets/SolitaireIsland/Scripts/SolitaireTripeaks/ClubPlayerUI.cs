using Nightingale.Extensions;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System;
using TriPeaks.ProtoData.Club;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ClubPlayerUI : MonoBehaviour
	{
		public Text RankLabel;

		public FriendAvaterUI AvaterUI;

		public Text NameLabel;

		public Text MemberLabel;

		public Text TimeLabel;

		public Text JoinTimeLabel;

		public Text ScoreLabel;

		public Image StageIcon;

		public Button Button;

		private void ScrollCellContent(Member member)
		{
			StageIcon.sprite = SingletonBehaviour<StageIconHelper>.Get().GetSprite(member.Stage);
			bool flag = member.PlayerId.Equals(SolitaireTripeaksData.Get().GetPlayerId());
			RankLabel.text = member.Rank.ToString();
			AvaterUI.SetUser(member.SocialId, member.SocialPlatform, member.Avatar);
			NameLabel.text = member.PlayerName;
			if (flag)
			{
				NameLabel.color = new Color32(5, 112, 5, byte.MaxValue);
			}
			else
			{
				switch (SingletonBehaviour<ClubSystemHelper>.Get().GetClubRoles(member.Role))
				{
				case ClubRoles.Chairman:
					NameLabel.color = new Color32(222, 0, 156, byte.MaxValue);
					break;
				case ClubRoles.ViceChairman:
					NameLabel.color = new Color32(0, 107, 195, byte.MaxValue);
					break;
				default:
					NameLabel.color = new Color32(112, 42, 14, byte.MaxValue);
					break;
				}
			}
			MemberLabel.text = SingletonBehaviour<ClubSystemHelper>.Get().GetRole(member.Role);
			TimeLabel.text = SingletonBehaviour<ClubSystemHelper>.Get().GetOfflineTimeAgo((!flag) ? member.IdleTime : (-1));
			JoinTimeLabel.text = $"Joined {Mathf.CeilToInt((float)DateTime.UtcNow.Subtract(new DateTime(member.JoinTime)).TotalDays)} days";
			ScoreLabel.text = member.LeaderboardScore.ToString();
			Button.onClick.RemoveAllListeners();
			Button.onClick.AddListener(delegate
			{
				UnityEngine.Object.FindObjectOfType<ClubScene>().HasVaule(delegate(ClubScene e)
				{
					e.IsStay = true;
				});
				SingletonClass<MySceneManager>.Get().Popup<ClubPlayerScene>("Scenes/ClubPlayerScene", new ScaleEffect()).OnStart(member)
					.AddClosedListener(delegate
					{
						UnityEngine.Object.FindObjectOfType<ClubScene>().HasVaule(delegate(ClubScene e)
						{
							e.IsStay = false;
						});
					});
			});
		}
	}
}
