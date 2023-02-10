using Nightingale.Extensions;
using Nightingale.Localization;
using Nightingale.Utilitys;
using System.Collections.Generic;
using System.Linq;
using TriPeaks.ProtoData.Club;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ClubLeaderboardView : DelayBehaviour
	{
		public LoopScrollRect loopScrollRect;

		public ClubLeaderboardUI _PlayerLeaderboardUI;

		public GameObject LoadGameObject;

		public GameObject LeaderBoardCdGameObject;

		public GameObject LeaderBoardSettleGameObject;

		public GameObject LeaderBoardUploadGameObject;

		public GameObject LoadRankGameObject;

		public Text UploadRemainTimeLabel;

		public LocalizationLabel RewardRemainTimeLabel;

		public LocalizationLabel SettleRemainTimeLabel;

		private void Start()
		{
			ChangeRank(SingletonBehaviour<ClubSystemHelper>.Get().GetRankType());
			SingletonBehaviour<ClubSystemHelper>.Get().RankChanged.AddListener(ChangeRank);
			InvokeRepeating("RepeatingUpdate", 0f, 1f);
		}

		private void RepeatingUpdate()
		{
			switch (SingletonBehaviour<ClubSystemHelper>.Get().GetRankType())
			{
			case RankType.Upload:
				UploadRemainTimeLabel.text = SingletonBehaviour<ClubSystemHelper>.Get().GetUploadRemainTime().TOString();
				break;
			case RankType.Settle:
				SettleRemainTimeLabel.SetText(SingletonBehaviour<ClubSystemHelper>.Get().GetSettleRemainTime().TOString());
				break;
			case RankType.Reward:
				RewardRemainTimeLabel.SetText(SingletonBehaviour<ClubSystemHelper>.Get().GetRewardRemainTime().TOString());
				break;
			}
		}

		private void RankLoadCompleted(ClubLeaderboardListResponse response)
		{
			LoadRankGameObject.SetActive(value: false);
			LeaderBoardUploadGameObject.SetActive(value: true);
			List<RankClub> arrays = (from e in response.TopPlayers.ToList()
				select new RankClub(e, response.Stage, response.UpgradePosition, response.DowngradePosition)).ToList();
			loopScrollRect.objectsToFill = arrays.ToArray();
			loopScrollRect.totalCount = arrays.Count;
			loopScrollRect.RefreshCells();
			loopScrollRect.onValueChanged.RemoveAllListeners();
			if (response.TopPlayers.Count <= 7)
			{
				_PlayerLeaderboardUI.gameObject.SetActive(value: false);
			}
			else
			{
				RankClub finder = arrays.Find((RankClub e) => SingletonBehaviour<ClubSystemHelper>.Get().GetClubIdentifier().Equals(e.ClubId));
				if (finder != null)
				{
					_PlayerLeaderboardUI.ScrollCellContent(finder);
					loopScrollRect.onValueChanged.AddListener(delegate
					{
						_PlayerLeaderboardUI.gameObject.SetActive(value: false);
						List<ClubLeaderboardUI> list = Object.FindObjectsOfType<ClubLeaderboardUI>().ToList();
						list.Remove(_PlayerLeaderboardUI);
						ClubLeaderboardUI clubLeaderboardUI = list.Find((ClubLeaderboardUI e) => e.RankIndex == finder.Rank);
						if (clubLeaderboardUI != null)
						{
							Vector3 vector = clubLeaderboardUI.transform.parent.InverseTransformVector(clubLeaderboardUI.transform.position);
							if (vector.y > 360f)
							{
								_PlayerLeaderboardUI.gameObject.SetActive(value: true);
								(_PlayerLeaderboardUI.transform as RectTransform).anchoredPosition = new Vector2(0f, 690f);
							}
							else if (vector.y < -360f)
							{
								_PlayerLeaderboardUI.gameObject.SetActive(value: true);
								(_PlayerLeaderboardUI.transform as RectTransform).anchoredPosition = new Vector2(0f, 110f);
							}
						}
						else if (list.Find((ClubLeaderboardUI e) => e.RankIndex > finder.Rank) == null)
						{
							_PlayerLeaderboardUI.gameObject.SetActive(value: true);
							(_PlayerLeaderboardUI.transform as RectTransform).anchoredPosition = new Vector2(0f, 110f);
						}
						else if (list.Find((ClubLeaderboardUI e) => e.RankIndex < finder.Rank) == null)
						{
							_PlayerLeaderboardUI.gameObject.SetActive(value: true);
							(_PlayerLeaderboardUI.transform as RectTransform).anchoredPosition = new Vector2(0f, 690f);
						}
					});
				}
			}
			LoopDelayDo(delegate
			{
				if (loopScrollRect.gameObject.activeInHierarchy)
				{
					int num = arrays.FindIndex((RankClub e) => SingletonBehaviour<ClubSystemHelper>.Get().GetClubIdentifier().Equals(e.ClubId)) - 2;
					if (num > arrays.Count - 6)
					{
						loopScrollRect.RefillCellsFromEnd();
						return false;
					}
					if (num < 0)
					{
						num = 0;
					}
					loopScrollRect.RefillCells(num);
					return false;
				}
				return true;
			}, null);
		}

		private void ChangeRank(RankType type)
		{
			switch (type)
			{
			case RankType.None:
				LoadGameObject.SetActive(value: true);
				LoadRankGameObject.SetActive(value: false);
				LeaderBoardCdGameObject.SetActive(value: false);
				loopScrollRect.onValueChanged.RemoveAllListeners();
				loopScrollRect.ClearCells();
				LeaderBoardSettleGameObject.SetActive(value: false);
				LeaderBoardUploadGameObject.SetActive(value: false);
				SingletonBehaviour<ClubSystemHelper>.Get().RankEvent.RemoveListener(RankLoadCompleted);
				UploadRemainTimeLabel.text = "--:--:--";
				break;
			default:
				UploadRemainTimeLabel.text = SingletonBehaviour<ClubSystemHelper>.Get().GetUploadRemainTime().TOString();
				LoadGameObject.SetActive(value: false);
				LoadRankGameObject.SetActive(value: true);
				LeaderBoardCdGameObject.SetActive(value: false);
				LeaderBoardSettleGameObject.SetActive(value: false);
				LeaderBoardUploadGameObject.SetActive(value: false);
				loopScrollRect.onValueChanged.RemoveAllListeners();
				loopScrollRect.ClearCells();
				SingletonBehaviour<ClubSystemHelper>.Get().RankEvent.AddListener(RankLoadCompleted);
				SingletonBehaviour<ClubSystemHelper>.Get().GetLeaderboardRank();
				break;
			case RankType.Settle:
				_PlayerLeaderboardUI.gameObject.SetActive(value: false);
				loopScrollRect.onValueChanged.RemoveAllListeners();
				loopScrollRect.ClearCells();
				LoadGameObject.SetActive(value: false);
				LeaderBoardCdGameObject.SetActive(value: false);
				LeaderBoardSettleGameObject.SetActive(value: true);
				LeaderBoardUploadGameObject.SetActive(value: false);
				LoadRankGameObject.SetActive(value: false);
				SingletonBehaviour<ClubSystemHelper>.Get().RankEvent.RemoveListener(RankLoadCompleted);
				SettleRemainTimeLabel.SetText(SingletonBehaviour<ClubSystemHelper>.Get().GetSettleRemainTime().TOString());
				UploadRemainTimeLabel.text = "--:--:--";
				break;
			case RankType.Reward:
				loopScrollRect.onValueChanged.RemoveAllListeners();
				loopScrollRect.ClearCells();
				LoadGameObject.SetActive(value: false);
				LeaderBoardCdGameObject.SetActive(value: true);
				LeaderBoardSettleGameObject.SetActive(value: false);
				LeaderBoardUploadGameObject.SetActive(value: false);
				LoadRankGameObject.SetActive(value: false);
				SingletonBehaviour<ClubSystemHelper>.Get().RankEvent.RemoveListener(RankLoadCompleted);
				RewardRemainTimeLabel.SetText(SingletonBehaviour<ClubSystemHelper>.Get().GetRewardRemainTime().TOString());
				UploadRemainTimeLabel.text = "--:--:--";
				break;
			}
			RepeatingUpdate();
		}

		private void OnDestroy()
		{
			SingletonBehaviour<ClubSystemHelper>.Get().RankEvent.RemoveListener(RankLoadCompleted);
			SingletonBehaviour<ClubSystemHelper>.Get().RankChanged.RemoveListener(ChangeRank);
		}
	}
}
