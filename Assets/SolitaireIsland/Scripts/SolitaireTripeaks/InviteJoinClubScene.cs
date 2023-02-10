using Nightingale.Utilitys;
using System.Collections.Generic;
using System.Linq;

namespace SolitaireTripeaks
{
	public class InviteJoinClubScene : SoundScene
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
			users = (from user in users
				where !string.IsNullOrEmpty(user.GetPlayerId())
				select user).ToList();
			FriendViewUI.PutFriend((from e in users
				select new TripeaksPlayerInView
				{
					player = e
				}).ToList(), delegate(List<TripeaksPlayerInView> players)
			{
				SingletonBehaviour<ClubSystemHelper>.Get().InviteJoinClub(players);
				players.ForEach(delegate(TripeaksPlayerInView e)
				{
					e.IsWait = true;
				});
				FriendViewUI.UpdateViewContent();
			});
		}

		public void InvitableAllFriends()
		{
			SingletonBehaviour<GlobalConfig>.Get().InvitableAllFriends();
		}
	}
}
