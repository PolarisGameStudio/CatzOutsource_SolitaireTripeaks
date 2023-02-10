using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using TriPeaks.ProtoData.Club;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class JoinClubScene : SoundScene
	{
		public Button BtnCreatorClub;

		public Button BtnFindClub;

		public ClubMiniUI ClubMiniUI;

		private void Awake()
		{
			BtnCreatorClub.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Close();
				SingletonClass<MySceneManager>.Get().Popup<CreatorClubScene>("Scenes/CreatorClubScene").OnStart();
			});
			BtnFindClub.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Close();
				SingletonClass<MySceneManager>.Get().Popup<ClubScene>("Scenes/ClubScene").OnFind();
			});
			SingletonBehaviour<ClubSystemHelper>.Get().RandomClub();
			SingletonBehaviour<ClubSystemHelper>.Get().AddClubListener(ClubInfo);
			SingletonBehaviour<ClubSystemHelper>.Get().AddMyClubResponseListener(ClubResponse);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			SingletonBehaviour<ClubSystemHelper>.Get().RemoveClubListener(ClubInfo);
			SingletonBehaviour<ClubSystemHelper>.Get().RemoveMyClubResponseListener(ClubResponse);
		}

		private void ClubInfo(string info, Club club)
		{
			if ("Random".Equals(info))
			{
				ClubMiniUI.SetInfo(club);
			}
		}

		private void ClubResponse(MyClubResponse response)
		{
			if (response != null && response.Club != null)
			{
				SingletonBehaviour<ClubSystemHelper>.Get().RemoveClubListener(ClubInfo);
				SingletonBehaviour<ClubSystemHelper>.Get().RemoveMyClubResponseListener(ClubResponse);
				SingletonClass<MySceneManager>.Get().Close();
				SingletonBehaviour<ClubSystemHelper>.Get().ShowClubScene();
			}
		}
	}
}
