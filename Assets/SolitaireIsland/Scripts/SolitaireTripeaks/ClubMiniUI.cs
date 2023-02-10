using Nightingale.Utilitys;
using TriPeaks.ProtoData.Club;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ClubMiniUI : MonoBehaviour
	{
		public Image Icon;

		public Text NameLabel;

		public Text DescriptionLabel;

		public Button JoinButton;

		public GameObject LoadingGameObject;

		private void Awake()
		{
			JoinButton.interactable = false;
			SingletonBehaviour<GlobalConfig>.Get().SetColor(JoinButton.gameObject.transform, Color.gray);
		}

		public void SetInfo(Club clubData)
		{
			LoadingGameObject.SetActive(value: false);
			Icon.sprite = SingletonBehaviour<ClubSystemHelper>.Get().GetClubAvatar(clubData);
			Icon.SetNativeSize();
			NameLabel.text = clubData.ClubName;
			DescriptionLabel.text = clubData.ClubDescription;
			JoinButton.interactable = true;
			SingletonBehaviour<GlobalConfig>.Get().SetColor(JoinButton.gameObject.transform, Color.white);
			JoinButton.onClick.RemoveAllListeners();
			JoinButton.onClick.AddListener(delegate
			{
				SingletonBehaviour<ClubSystemHelper>.Get().ShowRequestJoinClubScene(clubData);
			});
		}
	}
}
