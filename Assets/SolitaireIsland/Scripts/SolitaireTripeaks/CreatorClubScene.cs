using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.SensitiveWords;
using Nightingale.Utilitys;
using TriPeaks.ProtoData.Club;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class CreatorClubScene : SoundScene
	{
		public Button CloseButton;

		public Button AvatarButton;

		public InputField NameInputField;

		public InputField DescriptionInputField;

		public PickOnceUI Membership;

		public Button CreatorButton;

		public Text CreatorLabel;

		public GameObject CreatorCoinsGameObject;

		private Club clubData;

		public void OnStart(Club club = null)
		{
			if (club == null)
			{
				CreatorCoinsGameObject.SetActive(value: true);
				club = new Club();
			}
			else
			{
				CreatorCoinsGameObject.SetActive(value: false);
			}
			clubData = club;
			if (string.IsNullOrEmpty(clubData.ClubIcon))
			{
				clubData.ClubIcon = "000";
			}
			AvatarButton.image.sprite = SingletonBehaviour<ClubSystemHelper>.Get().GetClubAvatar(clubData);
			base.IsStay = true;
			NameInputField.text = clubData.ClubName;
			DescriptionInputField.text = clubData.ClubDescription;
			Membership.PutLabels(new string[2]
			{
				SingletonBehaviour<ClubSystemHelper>.Get().GetMembership(isPrivate: false),
				SingletonBehaviour<ClubSystemHelper>.Get().GetMembership(isPrivate: true)
			}, clubData.Private ? 1 : 0);
			if (string.IsNullOrEmpty(clubData.ClubId))
			{
				CreatorLabel.text = LocalizationUtility.Get("Localization_Club.json").GetString("btn_Create");
			}
			else
			{
				NameInputField.interactable = false;
				CreatorLabel.text = LocalizationUtility.Get("Localization_Club.json").GetString("btn_Modify");
			}
			AvatarButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Popup<ClubAvatarScene>("Scenes/ClubAvatarScene").OnStart(delegate(Sprite avatar)
				{
					clubData.ClubIcon = avatar.name;
					AvatarButton.image.sprite = avatar;
				});
			});
			CreatorButton.onClick.AddListener(delegate
			{
				clubData.ClubName = SingletonClass<SensitiveWords>.Get().ProfanityFilter(NameInputField.text);
				clubData.ClubDescription = SingletonClass<SensitiveWords>.Get().ProfanityFilter(DescriptionInputField.text);
				clubData.Private = (Membership.GetIndex() == 1);
				if (string.IsNullOrEmpty(clubData.ClubIcon))
				{
					clubData.ClubIcon = "000";
				}
				if (string.IsNullOrEmpty(clubData.ClubName) || string.IsNullOrEmpty(clubData.ClubDescription))
				{
					SingletonBehaviour<ClubSystemHelper>.Get().ShowClubNameCanotEmpty();
				}
				else if (string.IsNullOrEmpty(clubData.ClubId))
				{
					if (PackData.Get().GetCommodity(BoosterType.Coins).GetTotal() < 5000)
					{
						StoreScene.ShowOutofCoins();
					}
					else
					{
						SingletonBehaviour<ClubSystemHelper>.Get().CreatorClub(clubData, delegate(bool success)
						{
							if (success)
							{
								SessionData.Get().UseCommodity(BoosterType.Coins, 5000L, "CreatorClub");
								SingletonBehaviour<ClubSystemHelper>.Get().CloseAllShowClubScene();
							}
							else
							{
								SingletonBehaviour<ClubSystemHelper>.Get().ShowOperationFaild();
							}
						});
					}
				}
				else
				{
					SingletonBehaviour<ClubSystemHelper>.Get().ModifyClub(clubData, delegate(bool success)
					{
						if (success)
						{
							SingletonBehaviour<ClubSystemHelper>.Get().ShowModifyClubCompleted(delegate
							{
								SingletonClass<MySceneManager>.Get().Close();
							});
						}
						else
						{
							SingletonBehaviour<ClubSystemHelper>.Get().ShowOperationFaild();
						}
					});
				}
			});
			CloseButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Close(new JoinEffect());
				if (string.IsNullOrEmpty(clubData.ClubId))
				{
					SingletonClass<MySceneManager>.Get().Popup<JoinClubScene>("Scenes/JoinClubScene");
				}
			});
		}
	}
}
