using Nightingale.Socials;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using TriPeaks.ProtoData.Leaderboard;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace SolitaireTripeaks
{
	public class LeaderBoardUtility : SingletonBehaviour<LeaderBoardUtility>
	{
		private static readonly object RequestLock = new object();

		private static int _requestId;

		private bool _isDownloading;

		public UnityEvent RewardChanged = new UnityEvent();

		public RankEvent RankEvent = new RankEvent();

		public RankTypeEvent RankChanged = new RankTypeEvent();

		private readonly List<LeaderboardListResponse> _leaderBoardCaches = new List<LeaderboardListResponse>();

		public MiniLeaderBoardEvent MiniLeaderBoardEvent = new MiniLeaderBoardEvent();

		private List<RewardItem> RewardItems;

		private Coroutine _coroutine;

		private string _leaderBoardApi;

		private bool _caching;

		private bool _connect;

		private double StateCountdown;

		private RankType _lastRankType;

		private RankType _currentRankType;

		public TopPlayerEvent TopPlayerEvent = new TopPlayerEvent();

		private bool isTopDownloading;

		private Dictionary<string, List<TopPlayer>> topplayers = new Dictionary<string, List<TopPlayer>>();

		private static int RequestId
		{
			get
			{
				lock (RequestLock)
				{
					return ++_requestId;
				}
			}
		}

		public bool IsUploadEnable => IsOepn && GetRankType() == RankType.Upload;

		public bool IsOepn => PlayData.Get().HasThanLevelData(0, 0, 9);

		public RequestState RewardRequest
		{
			get;
			private set;
		}

		public RankType GetRankType()
		{
			if (_connect)
			{
				return _currentRankType;
			}
			return RankType.None;
		}

		public RankType GetRankTypeOffline()
		{
			return _currentRankType;
		}

		public void OnAppStart()
		{
			_leaderBoardApi = NightingaleConfig.Get().LeaderBoardApi;
			GetState();
			if (IsOepn)
			{
				GetReward();
			}
		}

		private void OnApplicationFocus(bool focus)
		{
			SystemTime.IsConnect = false;
			if (focus)
			{
				GetState();
			}
		}

		public TimeSpan GetUploadRemainTime()
		{
			return TimeSpan.FromMilliseconds(StateCountdown);
		}

		public TimeSpan GetSettleRemainTime()
		{
			return TimeSpan.FromMilliseconds(StateCountdown);
		}

		public TimeSpan GetRewardRemainTime()
		{
			return TimeSpan.FromMilliseconds(StateCountdown);
		}

		public void GetState()
		{
#if ENABLE_AZURE
            LeaderboardRequest leaderboardRequest = new LeaderboardRequest();
			leaderboardRequest.RequestId = RequestId;
			leaderboardRequest.Cmd = 205;
			leaderboardRequest.Platform = Application.platform.ToString();
			leaderboardRequest.AppVersion = Application.version;
			leaderboardRequest.Channel = "Official";
			leaderboardRequest.Group = "Normal";
			leaderboardRequest.PlayerId = SolitaireTripeaksData.Get().GetPlayerId();
			LeaderboardRequest obj = leaderboardRequest;
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_leaderBoardApi}/container/leaderboard?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
			UnityEngine.Debug.Log("-------------------获取高分榜状态：" + unityWebRequest.url);
			SystemTime.IsConnect = false;
			_connect = false;
			RankChanged.Invoke(RankType.None);
			if (_coroutine != null)
			{
				StopCoroutine(_coroutine);
			}
			_coroutine = StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				if (download.isDone)
				{
					LeaderboardStateResponse leaderboardStateResponse = ProtoDataUtility.Deserialize<LeaderboardStateResponse>(download.data);
					if (leaderboardStateResponse == null)
					{
						return;
					}
					if (leaderboardStateResponse.ErrorCode == 1)
					{
						UnityEngine.Debug.Log($"获取高分榜状态成功，State: {leaderboardStateResponse.State}，StateCountdown: {leaderboardStateResponse.StateCountdown}, Ticks: {leaderboardStateResponse.Ticks}, RankId: {leaderboardStateResponse.RankId}");
						SystemTime.UtcNow = new DateTime(leaderboardStateResponse.Ticks, DateTimeKind.Utc);
						RankCoinData.Get().SetRank(leaderboardStateResponse.RankId);
						ChangeRankType(EnumUtility.GetEnumType(leaderboardStateResponse.State, RankType.Reward), leaderboardStateResponse.StateCountdown);
						RankCoinData.Get().SynOldVersion();
						return;
					}
					UnityEngine.Debug.LogWarning($"获取高分榜状态，代码：{leaderboardStateResponse.ErrorCode}");
				}
				DelayDo(new WaitForSeconds(10f), GetState);
			}));
#endif
		}

		public void GetMiniRank()
		{
			string text = RankCoinData.Get().NewRankId;
			if (string.IsNullOrEmpty(text))
			{
				text = string.Empty;
			}
			if (RankCoinData.Get().RankCoinNumbers == 0)
			{
				SingletonData<RankCache>.Get().Put(text, -1);
				MiniLeaderBoardEvent.Invoke(-1, RankGrow.Default);
				return;
			}
			if (_caching && text.Equals(SingletonData<RankCache>.Get().RankId))
			{
				MiniLeaderBoardEvent.Invoke(SingletonData<RankCache>.Get().Rank, SingletonData<RankCache>.Get().RankGrow);
				return;
			}
			LeaderboardRequest leaderboardRequest = new LeaderboardRequest();
			leaderboardRequest.RequestId = RequestId;
			leaderboardRequest.Cmd = 201;
			leaderboardRequest.Platform = Application.platform.ToString();
			leaderboardRequest.AppVersion = Application.version;
			leaderboardRequest.Channel = "Official";
			leaderboardRequest.Group = "Normal";
			leaderboardRequest.PlayerId = SolitaireTripeaksData.Get().GetPlayerId();
			LeaderboardRequest obj = leaderboardRequest;
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_leaderBoardApi}/container/leaderboard?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
			UnityEngine.Debug.Log("-------------------Mini排行榜：" + unityWebRequest.url);
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				if (download.isDone)
				{
					LeaderboardMeResponse leaderboardMeResponse = ProtoDataUtility.Deserialize<LeaderboardMeResponse>(download.data);
					if (leaderboardMeResponse != null)
					{
						if (leaderboardMeResponse.ErrorCode == 1)
						{
							UnityEngine.Debug.Log($"获取Mini高分榜成功, RankId: {leaderboardMeResponse.RankId}, Stage: {leaderboardMeResponse.Stage}, Position: {leaderboardMeResponse.Position}, State: {leaderboardMeResponse.State}, StateCountdown: {leaderboardMeResponse.StateCountdown}");
							RankCoinData.Get().SetRank(leaderboardMeResponse.RankId);
							ChangeRankType(EnumUtility.GetEnumType(leaderboardMeResponse.State, RankType.Reward), leaderboardMeResponse.StateCountdown);
							if (!string.IsNullOrEmpty(leaderboardMeResponse.RankId))
							{
								_caching = true;
								SingletonData<RankCache>.Get().Put(leaderboardMeResponse.RankId, leaderboardMeResponse.Position);
								if (SingletonData<RankCache>.Get().RankGrow != RankGrow.Default)
								{
									_leaderBoardCaches.Clear();
								}
								MiniLeaderBoardEvent.Invoke(leaderboardMeResponse.Position, SingletonData<RankCache>.Get().RankGrow);
							}
						}
						else
						{
							UnityEngine.Debug.Log($"获取自己高分榜信息失败, 代码：{leaderboardMeResponse.ErrorCode}");
						}
					}
				}
			}));
		}

		public void GetRank()
		{
			Func<bool> callBack = delegate
			{
				LeaderboardListResponse leaderboardListResponse2 = _leaderBoardCaches.Find((LeaderboardListResponse e) => e.RankId == RankCoinData.Get().NewRankId);
				if (leaderboardListResponse2 == null)
				{
					return false;
				}
				RankEvent.Invoke(leaderboardListResponse2);
				return true;
			};
			if (!callBack() && !_isDownloading)
			{
				_isDownloading = true;
				LeaderboardRequest leaderboardRequest = new LeaderboardRequest();
				leaderboardRequest.RequestId = RequestId;
				leaderboardRequest.Cmd = 203;
				leaderboardRequest.Platform = Application.platform.ToString();
				leaderboardRequest.AppVersion = Application.version;
				leaderboardRequest.Channel = "Official";
				leaderboardRequest.Group = "Normal";
				leaderboardRequest.PlayerId = SolitaireTripeaksData.Get().GetPlayerId();
				LeaderboardRequest obj = leaderboardRequest;
				UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_leaderBoardApi}/container/leaderboard?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
				UnityEngine.Debug.Log("-------------------获取完整高分榜：" + unityWebRequest.url);
				StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
				{
					_isDownloading = false;
					if (download.isDone)
					{
						LeaderboardListResponse leaderboardListResponse = ProtoDataUtility.Deserialize<LeaderboardListResponse>(download.data);
						if (leaderboardListResponse != null)
						{
							if (leaderboardListResponse.ErrorCode == 1)
							{
								RankCoinData.Get().SetRank(leaderboardListResponse.RankId);
								ChangeRankType(EnumUtility.GetEnumType(leaderboardListResponse.State, RankType.Reward), leaderboardListResponse.StateCountdown);
								UnityEngine.Debug.Log($"获取高分榜成功, RankId: {leaderboardListResponse.RankId}, Stage: {leaderboardListResponse.Stage}, UpgradePosition: {leaderboardListResponse.UpgradePosition}, DowngradePosition: {leaderboardListResponse.DowngradePosition}, TopPlayers: {leaderboardListResponse.TopPlayers.Count}, State: {leaderboardListResponse.State}, StateCountdown: {leaderboardListResponse.StateCountdown}");
								_leaderBoardCaches.Clear();
								_leaderBoardCaches.Add(leaderboardListResponse);
								Rank rank = leaderboardListResponse.TopPlayers.ToList().Find((Rank e) => e.PlayerId == SolitaireTripeaksData.Get().GetPlayerId());
								if (rank != null)
								{
									SingletonData<RankCache>.Get().Put(leaderboardListResponse.RankId, rank.Position);
								}
								callBack();
							}
							else
							{
								UnityEngine.Debug.Log($"获取高分榜失败, 代码：{leaderboardListResponse.ErrorCode}");
							}
						}
					}
				}));
			}
		}

		public void UploadScore(int score, string avater, UnityAction unityAction = null)
		{
			LeaderboardRequest leaderboardRequest = new LeaderboardRequest();
			leaderboardRequest.RequestId = RequestId;
			leaderboardRequest.Cmd = 202;
			leaderboardRequest.Platform = Application.platform.ToString();
			leaderboardRequest.AppVersion = Application.version;
			leaderboardRequest.Channel = "Official";
			leaderboardRequest.Group = "Normal";
			leaderboardRequest.PlayerId = SolitaireTripeaksData.Get().GetPlayerId();
			leaderboardRequest.Rank = new Rank
			{
				PlayerId = SolitaireTripeaksData.Get().GetPlayerId(),
				PlayerName = AuxiliaryData.Get().GetNickName(),
				AvatarId = avater,
				SocialId = SingletonBehaviour<FacebookMananger>.Get().UserId,
				SocialPlatform = 1,
				Score = score
			};
			LeaderboardRequest obj = leaderboardRequest;
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_leaderBoardApi}/container/leaderboard?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
			UnityEngine.Debug.Log("-------------------上传分数：" + unityWebRequest.url);
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				if (download.isDone)
				{
					LeaderboardResponse leaderboardResponse = ProtoDataUtility.Deserialize<LeaderboardResponse>(download.data);
					if (leaderboardResponse != null)
					{
						if (leaderboardResponse.ErrorCode == 1)
						{
							UnityEngine.Debug.Log("更新高分榜成功");
							ChangeRankType(EnumUtility.GetEnumType(leaderboardResponse.State, RankType.Reward), leaderboardResponse.StateCountdown);
							ClearCache();
							if (unityAction != null)
							{
								unityAction();
							}
							if (!RankCoinData.Get().Cleaned)
							{
								RankCoinData.Get().Cleaned = true;
								ClearTopPlayers();
							}
						}
						else if (leaderboardResponse.ErrorCode == 9999)
						{
							UnityEngine.Debug.LogWarning($"检测到作弊行为，事件Id：{leaderboardResponse.ExtraState}");
							VerificationUtility.Get().SetCheatId(leaderboardResponse.ExtraState);
						}
						else
						{
							UnityEngine.Debug.LogWarning($"更新高分榜失败，代码：{leaderboardResponse.ErrorCode}");
						}
					}
				}
			}));
		}

		public void GetReward()
		{
			if (RewardRequest != RequestState.Loading)
			{
				RewardRequest = RequestState.Loading;
				LeaderboardRequest leaderboardRequest = new LeaderboardRequest();
				leaderboardRequest.RequestId = RequestId;
				leaderboardRequest.Cmd = 204;
				leaderboardRequest.Platform = Application.platform.ToString();
				leaderboardRequest.AppVersion = Application.version;
				leaderboardRequest.Channel = "Official";
				leaderboardRequest.Group = "Normal";
				leaderboardRequest.PlayerId = SolitaireTripeaksData.Get().GetPlayerId();
				LeaderboardRequest obj = leaderboardRequest;
				UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_leaderBoardApi}/container/leaderboard?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
				UnityEngine.Debug.Log("-------------------获取奖励：" + unityWebRequest.url);
				StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
				{
					if (download.isDone)
					{
						LeaderboardRewardResponse rewardResponse = ProtoDataUtility.Deserialize<LeaderboardRewardResponse>(download.data);
						if (rewardResponse != null)
						{
							if (rewardResponse.ErrorCode == 1)
							{
								RewardItems = rewardResponse.RewardTable.ToList();
								RewardRequest = RequestState.Success;
								RankType enumType = EnumUtility.GetEnumType(rewardResponse.State, RankType.Reward);
								if (enumType != RankType.Settle)
								{
									if (rewardResponse.RewardPosition == -1)
									{
										if (rewardResponse.CurrentStage > (int)RankCoinData.Get().Staged)
										{
											RankCoinData.Get().Staged = EnumUtility.GetEnumType(rewardResponse.CurrentStage, SegmentType.Bronze);
										}
									}
									else
									{
										UnityEngine.Debug.Log($"领取排行榜奖励成功，RewardRankId: {rewardResponse.RewardRankId}RewardStage：{rewardResponse.RewardStage}, RewardPosition：{rewardResponse.RewardPosition},, RewardTable = {rewardResponse.RewardTable.Count}, CurrentRankId: {rewardResponse.CurrentRankId}, CurrentStage: {rewardResponse.CurrentStage}, State: {rewardResponse.State}, StateCountDown: {rewardResponse.StateCountdown}");
										if (!string.IsNullOrEmpty(rewardResponse.RewardRankId))
										{
											RankCoinData.Get().SetRewardingRank(rewardResponse.RewardRankId, rewardResponse.RewardPosition, GetCommoditys(rewardResponse.RewardStage, rewardResponse.RewardPosition), EnumUtility.GetEnumType(rewardResponse.CurrentStage, SegmentType.Bronze));
											RankCoinData.Get().NewRewardings.RemoveAll((RankRewardData e) => e.reward && !e.id.Equals(rewardResponse.RewardRankId));
											RewardChanged.Invoke();
										}
									}
								}
								RankCoinData.Get().SetRank(rewardResponse.CurrentRankId);
								ChangeRankType(EnumUtility.GetEnumType(rewardResponse.State, RankType.Reward), rewardResponse.StateCountdown);
							}
							else
							{
								UnityEngine.Debug.LogWarning($"获取高分榜奖励失败，代码：{rewardResponse.ErrorCode}");
								RewardRequest = RequestState.Fail;
							}
						}
					}
					else
					{
						RewardRequest = RequestState.Fail;
					}
				}));
			}
		}

		public void GetTopPlayers()
		{
			Func<bool> func = delegate
			{
				string newRankId = RankCoinData.Get().NewRankId;
				if (topplayers != null && !string.IsNullOrEmpty(newRankId) && topplayers.ContainsKey(newRankId))
				{
					TopPlayerEvent.Invoke(topplayers[newRankId]);
					return true;
				}
				return false;
			};
			if (!func() && !isTopDownloading)
			{
				isTopDownloading = true;
				LeaderboardRequest leaderboardRequest = new LeaderboardRequest();
				leaderboardRequest.RequestId = RequestId;
				leaderboardRequest.Cmd = 206;
				leaderboardRequest.Platform = Application.platform.ToString();
				leaderboardRequest.AppVersion = Application.version;
				leaderboardRequest.Channel = "Official";
				leaderboardRequest.Group = "Normal";
				leaderboardRequest.PlayerId = SolitaireTripeaksData.Get().GetPlayerId();
				LeaderboardRequest obj = leaderboardRequest;
				UnityWebRequest uwr = UnityWebRequest.Get($"{_leaderBoardApi}/container/leaderboard?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
				UnityEngine.Debug.Log("-------------------获取名人榜：" + uwr.url);
				StartCoroutine(StartUnityWeb(uwr, delegate(DownloadHandler download)
				{
					isTopDownloading = false;
					if (download.isDone)
					{
						LeaderboardTopResponse leaderboardTopResponse = ProtoDataUtility.Deserialize<LeaderboardTopResponse>(download.data);
						if (leaderboardTopResponse != null)
						{
							if (leaderboardTopResponse.ErrorCode == 1)
							{
								ClearTopPlayers();
								topplayers.Add(leaderboardTopResponse.RankId, leaderboardTopResponse.TopPlayers.ToList());
								GetTopPlayers();
								ChangeRankType(EnumUtility.GetEnumType(leaderboardTopResponse.State, RankType.Reward), leaderboardTopResponse.StateCountdown);
							}
							else
							{
								UnityEngine.Debug.LogWarning($"获取名人排行榜失败，代码：{leaderboardTopResponse.ErrorCode}");
								SingletonBehaviour<StepUtility>.Get().Append("GetTopPlayers", 10f, GetTopPlayers);
							}
						}
					}
					else
					{
						UnityEngine.Debug.LogWarning($"获取名人排行榜失败，{uwr.error} 代码：{uwr.responseCode}");
						SingletonBehaviour<StepUtility>.Get().Append("GetTopPlayers", 10f, GetTopPlayers);
					}
				}));
			}
		}

		public void ClearTopPlayers()
		{
			UnityEngine.Debug.Log("清理本地名人榜信息。");
			topplayers.Clear();
		}

		public void ClearMiniCache()
		{
			_caching = false;
		}

		public void ClearCache()
		{
			ClearMiniCache();
			_leaderBoardCaches.Clear();
		}

		public List<PurchasingCommodity> GetCommoditys(int segment, int position)
		{
			return (from e in RewardItems
				where e.Stage == segment
				where e.MinPosition <= position && e.MaxPosition >= position
				select new PurchasingCommodity
				{
					count = e.Amount,
					boosterType = EnumUtility.GetEnumType(e.ItemId, BoosterType.Coins)
				}).ToList();
		}

		public int GetCoins(int segment, int position)
		{
			List<PurchasingCommodity> commoditys = GetCommoditys(segment, position);
			return commoditys.Find((PurchasingCommodity e) => e.boosterType == BoosterType.Coins)?.count ?? 0;
		}

		public Vector2 GetCoins(int segment)
		{
			IEnumerable<RewardItem> enumerable = from e in GetRewards()
				where e.Stage == segment
				where e.ItemId == 1
				select e;
			if (enumerable == null || enumerable.Count() == 0)
			{
				return Vector2.zero;
			}
			return new Vector2(enumerable.Min((RewardItem e) => e.Amount), enumerable.Max((RewardItem e) => e.Amount));
		}

		public List<RewardItem> GetRewards()
		{
			if (RewardItems == null || RewardItems.Count == 0)
			{
				List<RewardItem> list = new List<RewardItem>();
				list.Add(new RewardItem
				{
					Stage = 1,
					Amount = 2000,
					ItemId = 1,
					MinPosition = 1
				});
				list.Add(new RewardItem
				{
					Stage = 1,
					Amount = 4000,
					ItemId = 1,
					MinPosition = 1
				});
				list.Add(new RewardItem
				{
					Stage = 2,
					Amount = 4000,
					ItemId = 1,
					MinPosition = 1
				});
				list.Add(new RewardItem
				{
					Stage = 2,
					Amount = 10000,
					ItemId = 1,
					MinPosition = 1
				});
				list.Add(new RewardItem
				{
					Stage = 3,
					Amount = 10000,
					ItemId = 1,
					MinPosition = 1
				});
				list.Add(new RewardItem
				{
					Stage = 3,
					Amount = 22000,
					ItemId = 1,
					MinPosition = 1
				});
				list.Add(new RewardItem
				{
					Stage = 4,
					Amount = 22000,
					ItemId = 1,
					MinPosition = 1
				});
				list.Add(new RewardItem
				{
					Stage = 4,
					Amount = 45000,
					ItemId = 1,
					MinPosition = 1
				});
				list.Add(new RewardItem
				{
					Stage = 5,
					Amount = 45000,
					ItemId = 1,
					MinPosition = 1
				});
				list.Add(new RewardItem
				{
					Stage = 5,
					Amount = 90000,
					ItemId = 1,
					MinPosition = 1
				});
				list.Add(new RewardItem
				{
					Stage = 6,
					Amount = 90000,
					ItemId = 1,
					MinPosition = 1
				});
				list.Add(new RewardItem
				{
					Stage = 6,
					Amount = 200000,
					ItemId = 1,
					MinPosition = 1
				});
				list.Add(new RewardItem
				{
					Stage = 7,
					Amount = 200000,
					ItemId = 1,
					MinPosition = 1
				});
				list.Add(new RewardItem
				{
					Stage = 7,
					Amount = 1000000,
					ItemId = 1,
					MinPosition = 1
				});
				return list;
			}
			return RewardItems;
		}

		public void SynOldRankVersion(string api, RankRewardData rewardData)
		{
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{api}/minirank?rankId={rewardData.id}&userId={SolitaireTripeaksData.Get().GetPlayerId()}");
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				if (download.isDone)
				{
					int result = -1;
					if (int.TryParse(download.text, out result))
					{
						rewardData.syn = true;
						rewardData.rank = result;
						RewardChanged.Invoke();
					}
				}
			}));
		}

		private void ChangeRankType(RankType rankType, int stateCountdown)
		{
			if (rankType == RankType.Reward || rankType == RankType.Settle)
			{
				RankCoinData.Get().ClearRank();
				ClearCache();
			}
			if (stateCountdown == 0)
			{
				stateCountdown = 10000;
			}
			StateCountdown = stateCountdown;
			UnityEngine.Debug.Log($"==============当前的排行榜状态{rankType}, 结束倒计时时间{StateCountdown}");
			if (_currentRankType != rankType)
			{
				_lastRankType = _currentRankType;
				_currentRankType = rankType;
				if (_lastRankType != RankType.Reward && _lastRankType != 0 && _currentRankType == RankType.Reward)
				{
					GetReward();
				}
				_connect = true;
				RankChanged.Invoke(rankType);
			}
			else if (GetRankType() != rankType)
			{
				_connect = true;
				RankChanged.Invoke(rankType);
			}
			else
			{
				_connect = true;
			}
		}

		private void Update()
		{
			if (_connect)
			{
				StateCountdown -= Time.unscaledDeltaTime * 1000f;
				if (StateCountdown <= 0.0)
				{
					GetState();
				}
			}
		}
	}
}
