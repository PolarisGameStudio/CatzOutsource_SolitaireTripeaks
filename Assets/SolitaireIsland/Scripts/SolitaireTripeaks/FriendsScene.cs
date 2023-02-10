using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Socials;
using Nightingale.Utilitys;
using System.Collections.Generic;
using System.Linq;

namespace SolitaireTripeaks
{
	public class FriendsScene : BaseScene
	{
		public FriendViewUI FriendViewUI;

		private void Awake()
		{
			base.IsStay = true;
			SingletonBehaviour<TripeaksPlayerHelper>.Get().AddListener(DownloadFriendsCompleted);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			SingletonBehaviour<TripeaksPlayerHelper>.Get().RemoveListener(DownloadFriendsCompleted);
		}

		private void DownloadFriendsCompleted(List<TripeaksPlayer> users)
		{
			LocalizationUtility utility = LocalizationUtility.Get("Localization_facebook.json");
			List<TripeaksPlayerInView> list = new List<TripeaksPlayerInView>();
			foreach (TripeaksPlayer user in users)
			{
				if (list.Find((TripeaksPlayerInView e) => e.player.id == user.id) == null)
				{
					list.Add(new TripeaksPlayerInView
					{
						player = user,
						IsWait = AuxiliaryData.Get()._Friends.Contains(user.id)
					});
				}
			}
			FriendViewUI.PutFriend(list, delegate(List<TripeaksPlayerInView> players)
			{
				if (players.Count > 50)
				{
					players = players.GetRange(0, 50);
				}
				SingletonBehaviour<FacebookMananger>.Get().AppRequest(delegate(int num)
				{
					if (num > 0)
					{
						players.ForEach(delegate(TripeaksPlayerInView p)
						{
							p.IsWait = true;
						});
						AchievementData.Get().DoAchievement(AchievementType.AskHelp, num);
						AuxiliaryData.Get().PutFriends((from e in players
							select e.player.id).ToList());
						TipPopupNoIconScene.ShowAskForHelp();
						FriendViewUI.UpdateViewContent();
					}
				}, utility.GetString("fb_ask_for_help"), (from e in players
					select e.player.id).ToList(), "Ask", "Pyramid Solitaire");
			});
		}

		public void InvitableAllFriends()
		{
			SingletonBehaviour<GlobalConfig>.Get().InvitableAllFriends();
		}
	}
}
