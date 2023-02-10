using Nightingale.Azure;
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
	public class TripeaksLogUtility : SingletonBehaviour<TripeaksLogUtility>
	{
		private const string TripeaksError = "Error";

		private const string Session = "Session";

		private const string ButtonStatistics = "ButtonStatistics";

		private const string Purchasing = "PurchasingV2";

		private const string UsePurchasing = "UsePurchasing";

		private const string Event = "Event";

		private const string Sale = "Sale";

		private const string SpecialEvent = "SpecialEvent";

		private const string Player = "Player";

		private const string Detection = "Detection";

		private const string SynchronizeOdd = "SynchronizeOdd";

		private const string FacebookPlayer = "FacebookPlayer";

		private const string MakeAChoice = "MakeAChoice";

		public void Init()
		{
			Application.logMessageReceived -= LogMessageReceived;
			Application.logMessageReceived += LogMessageReceived;
			Login();
		}

		public void LogMessageReceived(string condition, string stackTrace, LogType type)
		{
			if (type != 0 && type != LogType.Exception)
			{
				return;
			}
			UnityEngine.Debug.Log("开启错误上传逻辑。");
			SessionData sassionData = SingletonData<SessionGroup>.Get().GetSassionData();
			string text = "SassionId";
			if (sassionData != null)
			{
				text = sassionData.SassionId;
			}
			if (!string.IsNullOrEmpty(stackTrace))
			{
				List<string> list = stackTrace.Split('\n').ToList();
				if (list.Count > 5)
				{
					list.RemoveRange(5, list.Count - 5);
				}
				stackTrace = string.Join("\n", list.ToArray());
			}
			Upload("_____Error", DateTime.UtcNow.ToString("yyyyMMddHHmmss"), SingletonClass<NightingaleSystemInfo>.Get().DeviceUniqueIdentifier, new Dictionary<string, object>
			{
				{
					"stackTrace",
					stackTrace
				},
				{
					"condition",
					condition
				},
				{
					"type",
					type.ToString()
				}
			});
		}

		public void Login()
		{
			UploadPlayer();
			UploadSpecialEvent();
			UploadSassion();
			SingletonData<SessionGroup>.Get().NewSession();
		}

		private void UploadSassion()
		{
			try
			{
				SessionData sassionData = SingletonData<SessionGroup>.Get().GetSassionData();
				if (sassionData != null)
				{
					Upload("Session", new DateTime(sassionData.StartTime, DateTimeKind.Local).ToString("yyyyMMddHHmmss"), SingletonClass<NightingaleSystemInfo>.Get().DeviceUniqueIdentifier, new Dictionary<string, object>
					{
						{
							"OnlineHours",
							new DateTime(sassionData.EndTime).Subtract(new DateTime(sassionData.StartTime)).TotalHours
						},
						{
							"UseNumbers",
							sassionData.ToUseJson()
						},
						{
							"SoureNumbers",
							sassionData.ToSoureJson()
						},
						{
							"RemainNumbers",
							sassionData.RemainNumbers
						},
						{
							"Level",
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

		public void Logout()
		{
			try
			{
				SingletonData<SessionGroup>.Get().SaveSassion();
			}
			catch (Exception)
			{
			}
		}

		public void UploadMinSynchronize()
		{
			try
			{
				DateTime utcNow = DateTime.UtcNow;
				Upload("SynchronizeOdd", utcNow.ToString("yyyyMMddHH"), utcNow.ToString("mmssfffff"), new Dictionary<string, object>());
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
		}

		public void UploadPurchasingPackage(string orderId, PurchasingPackage package)
		{
			try
			{
				string value = "None";
				QuestInfo questInfo = QuestData.Get().quests.Find((QuestInfo e) => e.QuestStyle == QuestStyle.Event);
				if (questInfo != null)
				{
					value = questInfo.GetId();
				}
				UnityEngine.Debug.LogWarningFormat("上传购买数据信息是：{0}。", package.Content);
				Upload("PurchasingV2", DateTime.UtcNow.ToString("yyyyMMddHH"), orderId, new Dictionary<string, object>
				{
					{
						"Id",
						package.id
					},
					{
						"Content",
						package.Content
					},
					{
						"Commoditys",
						string.Join("_", (from e in package.commoditys
							select $"{e.boosterType.ToString()}_{e.count}").ToArray())
					},
					{
						"EventId",
						value
					},
					{
						"Stage",
						RankCoinData.Get().Staged.ToString()
					},
					{
						"State",
						SingletonBehaviour<LeaderBoardUtility>.Get().GetRankTypeOffline().ToString()
					},
					{
						"Country",
						PlatformUtility.GetCountry()
					},
					{
						"CoinsAfterBuy",
						PackData.Get().GetCommodity(BoosterType.Coins).GetTotal()
					}
				});
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
		}

		public void UploadUsePurchasingPackage(string orderId, string type)
		{
			try
			{
				Upload("UsePurchasing", DateTime.UtcNow.ToString("yyyyMMddHH"), orderId, new Dictionary<string, object>
				{
					{
						"Type",
						type
					}
				});
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
		}

		public void UploadEvent(QuestInfo questInfo)
		{
			try
			{
				if (questInfo.QuestStyle == QuestStyle.Event)
				{
					Upload("Event", DateTime.UtcNow.ToString("yyyyMMddHHmmss"), SingletonClass<NightingaleSystemInfo>.Get().DeviceUniqueIdentifier, new Dictionary<string, object>
					{
						{
							"Id",
							questInfo.GetId()
						},
						{
							"IsComplete",
							questInfo.IsComplete().ToString()
						},
						{
							"TimeCost",
							SaleData.Get().hours - questInfo.ReceiveTime
						}
					});
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
		}

		public void UploadMakeAChoice(string choice, string eventId)
		{
			Upload("MakeAChoice", DateTime.UtcNow.ToString("yyyyMMddHHmmss"), SingletonClass<NightingaleSystemInfo>.Get().DeviceUniqueIdentifier, new Dictionary<string, object>
			{
				{
					"Content",
					choice
				},
				{
					"Id",
					eventId
				}
			});
		}

		public void UploadSale(SaleInfo info)
		{
			try
			{
				Upload("Sale", DateTime.UtcNow.ToString("yyyyMMddHHmmssfffff"), SingletonClass<NightingaleSystemInfo>.Get().DeviceUniqueIdentifier, new Dictionary<string, object>
				{
					{
						"Id",
						info.GetId()
					},
					{
						"ShowNumber",
						info.ShowNumber
					},
					{
						"PurchaseInfo",
						info.GetBuyInfo()
					}
				});
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
		}

		private void OnApplicationPause(bool pause)
		{
			if (pause)
			{
				Logout();
			}
			else
			{
				Login();
			}
		}

		private void OnApplicationQuit()
		{
			Logout();
		}

		private void UploadSpecialEvent()
		{
			try
			{
				if (SingletonBehaviour<SpecialActivityUtility>.Get().IsActive())
				{
					string text = string.Empty;
					foreach (PurchasingCommodity exchange in SpecialActivityData.Get().Exchanges)
					{
						string text2 = text;
						text = text2 + exchange.boosterType + "_" + exchange.count + ",";
					}
					text = (string.IsNullOrEmpty(text) ? "None" : text.Substring(0, text.Length - 1));
					Upload("SpecialEvent", SpecialActivityConfig.Get().GetSpecialId(), SingletonClass<NightingaleSystemInfo>.Get().DeviceUniqueIdentifier, new Dictionary<string, object>
					{
						{
							"CollectionCount",
							SpecialActivityData.Get().Numbers
						},
						{
							"CollectionTotalCount",
							SpecialActivityData.Get().TotalNumbers
						},
						{
							"SaleShowCount",
							SpecialActivityData.Get().OpenSpecialSaleNumbers
						},
						{
							"SaleBuyCount",
							SpecialActivityData.Get().SpecialSaleNumbers
						},
						{
							"ActivityShowCount",
							SpecialActivityData.Get().OpenSpecialNumbers
						},
						{
							"ActivityExchangeInfo",
							text
						}
					});
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
		}

		private void UploadPlayer()
		{
			try
			{
				Upload("Player", SolitaireTripeaksData.Get().GetPlayerId(), SingletonClass<NightingaleSystemInfo>.Get().DeviceUniqueIdentifier, new Dictionary<string, object>
				{
					{
						"FacebookId",
						SingletonBehaviour<FacebookMananger>.Get().UserId
					},
					{
						"InstallTime",
						new DateTime(StatisticsData.Get().InstallTicks).ToString("yyyyMMddHHmmss")
					},
					{
						"LoginFacebookTime",
						new DateTime(StatisticsData.Get().LoginTicks).ToString("yyyyMMddHHmmss")
					},
					{
						"InstallSurce",
						NtgNativeAgent.GetInstallReferrer()
					},
					{
						"Country",
						PlatformUtility.GetCountry()
					},
					{
						"Avatar",
						AuxiliaryData.Get().AvaterFileName
					},
					{
						"NickName",
						AuxiliaryData.Get().GetNickName()
					},
					{
						"Coins",
						PackData.Get().GetCommodity(BoosterType.Coins).GetTotal()
					},
					{
						"Shell",
						PackData.Get().GetCommodity(BoosterType.RandomBooster).GetTotal()
					},
					{
						"FreePlay",
						PackData.Get().GetCommodity(BoosterType.FreePlay).GetTotal()
					},
					{
						"ExpiredPlay",
						PackData.Get().GetCommodity(BoosterType.ExpiredPlay).GetTotal()
					},
					{
						"UnlimitedPlay",
						Mathf.Max(0f, (float)TimeSpan.FromSeconds(AuxiliaryData.Get().GetUnlimitedPlayRemain()).TotalHours)
					},
					{
						"Level",
						PlayData.Get().GetMax()
					},
					{
						"MasterLevel",
						PlayData.Get().GetMaxMasterLevels()
					}
				});
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
			try
			{
				Upload("Detection", SolitaireTripeaksData.Get().GetPlayerId(), SingletonClass<NightingaleSystemInfo>.Get().DeviceUniqueIdentifier, new Dictionary<string, object>
				{
					{
						"UnlimitedPlay",
						AuxiliaryData.Get().GetUnlimitedPlayRemain()
					},
					{
						"DoubleBlueStar",
						RankCoinData.Get().GetRemainTime().TotalSeconds
					}
				});
			}
			catch (Exception ex2)
			{
				UnityEngine.Debug.Log(ex2.Message);
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
	}
}
