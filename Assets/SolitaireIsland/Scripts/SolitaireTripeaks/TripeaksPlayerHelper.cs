using Nightingale.Azure;
using Nightingale.JSONUtilitys;
using Nightingale.Socials;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace SolitaireTripeaks
{
	public class TripeaksPlayerHelper : SingletonBehaviour<TripeaksPlayerHelper>
	{
		private bool isRunning;

		private const string FacebookPlayer = "FacebookPlayer";

		private List<TripeaksPlayer> players = new List<TripeaksPlayer>();

		private TripeaksPlayerEvent playerUpdate = new TripeaksPlayerEvent();

		private TripeaksPlayer player;

		private List<TripeaksPlayer> Players = new List<TripeaksPlayer>();

		private void Awake()
		{
			UploadTripeaksPlayer();
			UpdatePlayers();
			SingletonBehaviour<FacebookMananger>.Get().LoginChanged.AddListener(delegate(bool login)
			{
				isRunning = false;
				if (login)
				{
					UpdatePlayers();
				}
				else
				{
					players = new List<TripeaksPlayer>();
				}
				player.UpdateAvatar();
				player.UpdateNickName();
			});
			player = new TripeaksPlayer();
			player.UpdateBySelf();
		}

		private void UpdatePlayers()
		{
			isRunning = true;
			string facebookId = SingletonBehaviour<FacebookMananger>.Get().UserId;
			Dictionary<string, object> userData;
			SingletonBehaviour<FacebookMananger>.Get().GetFriendsInGame(delegate(List<FacebookUser> users)
			{
				UnityEngine.Debug.Log(string.Join(";", (from e in users
					select $"{e.id} {e.name}").ToArray()));
				if (facebookId != SingletonBehaviour<FacebookMananger>.Get().UserId)
				{
					isRunning = false;
				}
				else
				{
					players = new List<TripeaksPlayer>();
					if (users == null || users.Count == 0)
					{
						isRunning = false;
						playerUpdate.Invoke(players);
					}
					else
					{
						List<List<string>> arrays = PList((from e in users
							select e.id).ToList(), 10);
						foreach (List<string> ids in arrays.ToList())
						{
							AzureTableStorage.GetNew().QueryEntities("FacebookPlayer", ids.ToArray(), new string[4]
							{
								"PartitionKey",
								"RowKey",
								"MaxLevel",
								"Avatar"
							}, delegate(DownloadHandler download)
							{
								if (facebookId != SingletonBehaviour<FacebookMananger>.Get().UserId)
								{
									isRunning = false;
								}
								else
								{
									arrays.Remove(ids);
									if (arrays.Count == 0)
									{
										isRunning = false;
									}
									if (download.isDone)
									{
										try
										{
											Dictionary<string, object> dictionary = Json.Deserialize(download.text) as Dictionary<string, object>;
											if (dictionary.ContainsKey("value"))
											{
												List<object> list = dictionary["value"] as List<object>;
												foreach (object item in list)
												{
													userData = (item as Dictionary<string, object>);
													int level = 0;
													if (userData.ContainsKey("MaxLevel"))
													{
														level = int.Parse(userData["MaxLevel"].ToString());
													}
													string avatar = string.Empty;
													if (userData.ContainsKey("Avatar"))
													{
														avatar = userData["Avatar"].ToString();
													}
													players.Add(CreatePlayer(users.Find((FacebookUser e) => e.id == userData["PartitionKey"].ToString()), userData["RowKey"].ToString(), avatar, level));
												}
											}
										}
										catch (Exception ex)
										{
											UnityEngine.Debug.Log(ex.Message);
										}
										if (arrays.Count == 0)
										{
											(from e in users
												where players.Find((TripeaksPlayer p) => p.id == e.id) == null
												select e).ToList().ForEach(delegate(FacebookUser e)
											{
												players.Add(CreatePlayer(e));
											});
											playerUpdate.Invoke(players);
										}
									}
								}
							});
						}
					}
				}
			}, null, string.Empty);
		}

		private List<List<string>> PList(List<string> arrays, int max)
		{
			List<List<string>> list = new List<List<string>>();
			while (arrays.Count > max)
			{
				list.Add(arrays.GetRange(0, max));
				arrays.RemoveRange(0, max);
			}
			if (arrays.Count > 0)
			{
				list.Add(arrays);
			}
			return list;
		}

		public void AddListener(UnityAction<List<TripeaksPlayer>> unityAction, bool forced = false)
		{
			if (unityAction != null)
			{
				playerUpdate.AddListener(unityAction);
				DelayDo(delegate
				{
					if (!forced && players != null && players.Count > 0)
					{
						unityAction(players);
					}
					else if (!isRunning)
					{
						UpdatePlayers();
					}
				});
			}
		}

		public void RemoveListener(UnityAction<List<TripeaksPlayer>> unityAction)
		{
			if (unityAction != null)
			{
				playerUpdate.RemoveListener(unityAction);
			}
		}

		public void UploadTripeaksPlayer()
		{
			try
			{
				if (SingletonBehaviour<FacebookMananger>.Get().IsLogin())
				{
					Upload("FacebookPlayer", SingletonBehaviour<FacebookMananger>.Get().UserId, SolitaireTripeaksData.Get().GetPlayerId(), new Dictionary<string, object>
					{
						{
							"Avatar",
							AuxiliaryData.Get().AvaterFileName
						},
						{
							"MaxLevel",
							PlayData.Get().GetMax()
						}
					});
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
		}

		private void Upload(string tableName, string partitionKey, string rowKey, Dictionary<string, object> pairs, UnityAction<bool> unityAction = null)
		{
			try
			{
				string text = "{";
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("Platform", Application.platform.ToString());
				dictionary.Add("Version", Application.version.ToString());
				dictionary.Add("Group", string.Empty);
				dictionary.Add("DeviceId", SingletonClass<NightingaleSystemInfo>.Get().DeviceUniqueIdentifier);
				dictionary.Add("PlayerId", SolitaireTripeaksData.Get().GetPlayerId());
				dictionary.Add("Channel", Application.installerName);
				Dictionary<string, object> dictionary2 = dictionary;
				foreach (string key in dictionary2.Keys)
				{
					if (!pairs.ContainsKey(key))
					{
						pairs.Add(key, dictionary2[key]);
					}
				}
				foreach (string key2 in pairs.Keys)
				{
					text = ((!(pairs[key2] is string)) ? (text + $"\"{key2}\":{pairs[key2]},") : (text + $"\"{key2}\":\"{WWW.EscapeURL(pairs[key2].ToString())}\","));
				}
				text = text.Substring(0, text.Length - 1);
				text += "}";
				UnityEngine.Debug.LogFormat("开始{0} Table提交！", tableName);
				AzureTableStorage.GetNew().InsertOrMergeEntity(tableName, WWW.EscapeURL(partitionKey), WWW.EscapeURL(rowKey), text, delegate(DownloadHandler download)
				{
					UnityEngine.Debug.LogFormat("{0} Table提交{1}！", tableName, (!download.isDone) ? "失败" : "成功");
					if (unityAction != null)
					{
						unityAction(download.isDone);
					}
				});
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
		}

		public TripeaksPlayer CreatePlayer(FacebookUser user, string playerId, string avatar, int level)
		{
			TripeaksPlayer tripeaksPlayer = Players.Find((TripeaksPlayer e) => e.id == user.id && e.GetPlayerId() == playerId);
			if (tripeaksPlayer == null)
			{
				tripeaksPlayer = Players.Find((TripeaksPlayer e) => e.id == user.id && string.IsNullOrEmpty(e.GetPlayerId()));
				if (tripeaksPlayer == null)
				{
					tripeaksPlayer = new TripeaksPlayer();
					Players.Add(tripeaksPlayer);
				}
			}
			tripeaksPlayer.UpdateInfo(user, playerId, avatar, level);
			return tripeaksPlayer;
		}

		public TripeaksPlayer CreatePlayer(string socailId, string avatar)
		{
			TripeaksPlayer tripeaksPlayer = Players.Find((TripeaksPlayer e) => e.id == socailId && !string.IsNullOrEmpty(socailId));
			if (tripeaksPlayer == null)
			{
				tripeaksPlayer = new TripeaksPlayer(socailId, avatar);
				Players.Add(tripeaksPlayer);
			}
			else
			{
				tripeaksPlayer.SetAvatar(avatar);
			}
			return tripeaksPlayer;
		}

		public TripeaksPlayer CreatePlayer(FacebookUser user)
		{
			TripeaksPlayer tripeaksPlayer = Players.Find((TripeaksPlayer e) => e.id == user.id);
			if (tripeaksPlayer == null)
			{
				tripeaksPlayer = new TripeaksPlayer(user);
				Players.Add(tripeaksPlayer);
			}
			return tripeaksPlayer;
		}

		public TripeaksPlayer GetSelf()
		{
			return player;
		}
	}
}
