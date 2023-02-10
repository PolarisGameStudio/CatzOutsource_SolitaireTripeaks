using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using TriPeaks.ProtoData.Club;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ClubPlayerScene : BaseScene
	{
		public Text MaxLevelLabel;

		public Text MasterMaxLevelLabel;

		public Text StarCountLabel;

		public Button CloseButton;

		public Button RemoveButton;

		public Button RoleButton;

		public Text RoleLabel;

		public ClubPlayerScene OnStart(Member member)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Club.json");
			base.IsStay = true;
			MaxLevelLabel.text = string.Format(localizationUtility.GetString("Max_Levels"), member.MaxLevel);
			MasterMaxLevelLabel.text = string.Format(localizationUtility.GetString("Master_Max_Levels"), member.MaxMasterLevel);
			StarCountLabel.text = string.Format(localizationUtility.GetString("Total_Stars"), member.StarCount);
			CloseButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Close(new ScaleEffect());
			});
			RemoveButton.onClick.AddListener(delegate
			{
				SingletonBehaviour<ClubSystemHelper>.Get().ShowRemoveUserClub(member);
			});
			RemoveButton.gameObject.SetActive(value: false);
			RoleButton.gameObject.SetActive(value: false);
			ClubRoles roleInClub = SingletonBehaviour<ClubSystemHelper>.Get().GetRoleInClub();
			if (SingletonBehaviour<ClubSystemHelper>.Get().IsContains(member))
			{
				switch (roleInClub)
				{
				case ClubRoles.Chairman:
					if (member.Role != 1)
					{
						RemoveButton.gameObject.SetActive(value: true);
					}
					if (member.Role == 2)
					{
						RoleButton.gameObject.SetActive(value: true);
						RoleLabel.text = localizationUtility.GetString("Editor_To_Member");
						RoleButton.onClick.AddListener(delegate
						{
							SingletonBehaviour<ClubSystemHelper>.Get().ShowEditorUserMessages(upgrde: false, member.PlayerName, delegate
							{
								SingletonClass<MySceneManager>.Get().Close(new ScaleEffect());
								member.Role = 3;
								SingletonBehaviour<ClubSystemHelper>.Get().EditorClubUser(member);
							});
						});
					}
					else if (member.Role == 3 && SingletonBehaviour<ClubSystemHelper>.Get().GetViceChairmanNumbers() < 3)
					{
						RoleButton.gameObject.SetActive(value: true);
						RoleLabel.text = localizationUtility.GetString("Editor_To_ViceChairman");
						RoleButton.onClick.AddListener(delegate
						{
							SingletonBehaviour<ClubSystemHelper>.Get().ShowEditorUserMessages(upgrde: true, member.PlayerName, delegate
							{
								SingletonClass<MySceneManager>.Get().Close(new ScaleEffect());
								member.Role = 2;
								SingletonBehaviour<ClubSystemHelper>.Get().EditorClubUser(member);
							});
						});
					}
					break;
				case ClubRoles.ViceChairman:
					if (member.Role == 3)
					{
						RemoveButton.gameObject.SetActive(value: true);
					}
					break;
				}
			}
			if (!RemoveButton.gameObject.activeSelf && !RoleButton.gameObject.activeSelf)
			{
				RoleButton.transform.parent.gameObject.SetActive(value: false);
			}
			return this;
		}
	}
}
