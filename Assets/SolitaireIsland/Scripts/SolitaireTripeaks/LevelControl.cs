using Nightingale.U2D;
using Nightingale.Utilitys;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class LevelControl : MonoBehaviour
	{
		public Text LevelLabel;

		public List<LevelStarIslandData> LevelStarUIs;

		public List<LevelStarIslandData> LeaderBoardStarUIs;

		public Transform AvaterTransform;

		public DoubleSpriteUI doubleSpriteUI;

		public GameObject OpenGameObject;

		public GameObject CloseGameObject;

		private ScheduleData schedule;

		private int Level;

		private void OnDestroy()
		{
			SingletonBehaviour<LeaderBoardUtility>.Get().RankChanged.RemoveListener(ChangeRankType);
			SingletonBehaviour<SpecialActivityUtility>.Get().OnRefresh.RemoveListener(UpdateSpecialActivity);
		}

		public void SetInfo(ScheduleData schedule, UnityAction<ScheduleData> unityAction)
		{
			if (AuxiliaryData.Get().IsTreasure(schedule))
			{
				Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(IslandScene).Name, "UI/TreasureUI")).transform.SetParent(base.transform, worldPositionStays: false);
			}
			Level = SingletonClass<AAOConfig>.Get().GetLevel(schedule);
			this.schedule = schedule;
			Button component = GetComponent<Button>();
			LevelLabel.text = $"{SingletonClass<AAOConfig>.Get().GetLevelInWorld(schedule) + 1}";
			component.onClick.RemoveAllListeners();
			component.onClick.AddListener(delegate
			{
				if (unityAction != null)
				{
					unityAction(schedule);
				}
			});
			SingletonBehaviour<LeaderBoardUtility>.Get().RankChanged.RemoveListener(ChangeRankType);
			SingletonBehaviour<SpecialActivityUtility>.Get().OnRefresh.RemoveListener(UpdateSpecialActivity);
			SingletonBehaviour<SpecialActivityUtility>.Get().OnRefresh.AddListener(UpdateSpecialActivity);
			UpdateSpecialActivity();
			LevelData levelData = PlayData.Get().GetLevelData(schedule);
			SetLevelData(levelData);
			if (levelData == null)
			{
				if ((schedule.chapter == 0 && schedule.level == 0) || PlayData.Get().HasLevelData(SingletonClass<AAOConfig>.Get().GetPreSchedule(schedule)))
				{
					levelData = new LevelData();
					SetLevelOpen(visable: true);
					if (schedule.world == -1)
					{
						SetLevelData(levelData);
						return;
					}
					GameObject gameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>("UI/LevelPointParticle"), base.transform);
					gameObject.transform.SetAsFirstSibling();
					gameObject.transform.localPosition = new Vector3(0f, 8f, 0f);
					gameObject.transform.localEulerAngles = new Vector3(-20f, 0f, 0f);
				}
				else
				{
					SetLevelOpen(visable: false);
				}
			}
			else
			{
				SetLevelOpen(visable: true);
			}
		}

		public void AddFriendSchedule(List<TripeaksPlayer> players)
		{
			FriendAvaterUI[] componentsInChildren = AvaterTransform.GetComponentsInChildren<FriendAvaterUI>();
			FriendAvaterUI[] array = componentsInChildren;
			foreach (FriendAvaterUI friendAvaterUI in array)
			{
				UnityEngine.Object.Destroy(friendAvaterUI.gameObject);
			}
			if (players == null)
			{
				return;
			}
			TripeaksPlayer[] array2 = (from e in players
				where e.IsInLevele(schedule)
				select e).ToArray();
			int num = 0;
			GameObject asset = SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>("UI/FriendAvaterUI");
			TripeaksPlayer[] array3 = array2;
			foreach (TripeaksPlayer user in array3)
			{
				if (num > 4)
				{
					break;
				}
				num++;
				GameObject gameObject = UnityEngine.Object.Instantiate(asset);
				gameObject.transform.SetParent(AvaterTransform, worldPositionStays: false);
				gameObject.GetComponent<FriendAvaterUI>().SetUser(user);
			}
		}

		private void SetLevelData(LevelData levelData)
		{
			foreach (LevelStarIslandData levelStarUI in LevelStarUIs)
			{
				levelStarUI.SetLevelData(levelData);
			}
			if (levelData == null)
			{
				foreach (LevelStarIslandData leaderBoardStarUI in LeaderBoardStarUIs)
				{
					leaderBoardStarUI.SetActive(active: false);
				}
				return;
			}
			ChangeRankType(SingletonBehaviour<LeaderBoardUtility>.Get().GetRankType());
			SingletonBehaviour<LeaderBoardUtility>.Get().RankChanged.AddListener(ChangeRankType);
		}

		private void SetLevelOpen(bool visable)
		{
			doubleSpriteUI.SetState(visable);
			GetComponent<Button>().interactable = visable;
			if (OpenGameObject != null)
			{
				OpenGameObject.SetActive(visable);
			}
			if (CloseGameObject != null)
			{
				CloseGameObject.SetActive(!visable);
			}
		}

		private void ChangeRankType(RankType rankType)
		{
			if (RankCoinData.Get().HasTreasure(Level))
			{
				LevelData levelData = RankCoinData.Get().GetLevelData(Level);
				foreach (LevelStarIslandData leaderBoardStarUI in LeaderBoardStarUIs)
				{
					leaderBoardStarUI.SetActive(active: true);
					leaderBoardStarUI.SetLevelData(levelData);
				}
			}
			else
			{
				foreach (LevelStarIslandData leaderBoardStarUI2 in LeaderBoardStarUIs)
				{
					leaderBoardStarUI2.SetActive(active: false);
				}
			}
		}

		private void UpdateSpecialActivity()
		{
			if (SingletonBehaviour<SpecialActivityUtility>.Get().CreateSpecialActivityInDetail(schedule, base.transform))
			{
				SingletonBehaviour<SpecialActivityUtility>.Get().OnRefresh.RemoveListener(UpdateSpecialActivity);
			}
		}
	}
}
