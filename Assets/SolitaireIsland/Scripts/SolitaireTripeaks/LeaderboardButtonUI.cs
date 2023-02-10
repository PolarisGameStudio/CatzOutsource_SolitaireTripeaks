using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Toasts;
using Nightingale.Utilitys;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class LeaderboardButtonUI : MonoBehaviour
	{
		public Button Button;

		private bool inMini;

		private void Awake()
		{
			inMini = (UnityEngine.Object.FindObjectOfType<MiniLeaderBoardUI>() != null);
			base.gameObject.SetActive(SingletonBehaviour<LeaderBoardUtility>.Get().IsOepn);
			if (base.gameObject.activeSelf)
			{
				RewardChanged();
				Button.onClick.AddListener(delegate
				{
					TryShowLeaderboard();
				});
			}
			SingletonBehaviour<LeaderBoardUtility>.Get().RewardChanged.AddListener(RewardChanged);
		}

		private void OnDestroy()
		{
			SingletonBehaviour<LeaderBoardUtility>.Get().RewardChanged.RemoveListener(RewardChanged);
		}

		private void RewardChanged()
		{
			if (inMini)
			{
				SingletonBehaviour<GlobalConfig>.Get().CreateExclamationMark(base.gameObject, RankCoinData.Get().GetRewards().Count > 0, left: false, 0.7f, -10f, -10f);
			}
			else
			{
				SingletonBehaviour<GlobalConfig>.Get().CreateExclamationMark(base.gameObject, RankCoinData.Get().GetRewards().Count > 0);
			}
		}

		private static bool IsConnect()
		{
			IEnumerable<RankRewardData> source = from e in RankCoinData.Get().Rewardings
				where !e.syn
				select e;
			if (source.Count() > 0 || SingletonBehaviour<LeaderBoardUtility>.Get().RewardRequest != RequestState.Success)
			{
				return false;
			}
			return true;
		}

		public static void TryShowLeaderboard()
		{
			UnityAction LeaderboardRewards = null;
			LeaderboardRewards = delegate
			{
				List<RankRewardData> rewards = RankCoinData.Get().GetRewards();
				if (rewards.Count > 0)
				{
					rewards[0].CurrentStage = RankCoinData.Get().Staged;
					RankCoinData.Get().Staged = rewards[0].NextStage;
					SingletonClass<MySceneManager>.Get().Popup<LeaderboardRewardScene>("Scenes/LeaderboardRewardScene", new NavigationEffect()).OnStart(isClan: false, rewards[0], LeaderboardRewards);
				}
				else if (IsConnect())
				{
					SingletonClass<MySceneManager>.Get().Popup<LeaderboardScene>("Scenes/LeaderboardScene", new JoinEffect());
				}
				else
				{
					if (SingletonBehaviour<LeaderBoardUtility>.Get().RewardRequest == RequestState.Fail)
					{
						SingletonBehaviour<LeaderBoardUtility>.Get().GetReward();
					}
					RankCoinData.Get().SynOldVersion();
					LoadingHelper.Get("LeaderboardRewardsScene").ShowLoading(delegate(LoadingHelper helper, float total)
					{
						if (IsConnect())
						{
							TryShowLeaderboard();
							helper.StopLoading();
						}
						else if (total > 15f)
						{
							helper.StopLoading();
						}
					}, delegate(LoadingHelper helper, float total)
					{
						helper.StopLoading();
					}, LocalizationUtility.Get("Localization_LeaderBoard.json").GetString("loading_leaderboard_rewards"));
				}
			};
			LeaderboardRewards();
		}
	}
}
