using Nightingale.JSONUtilitys;
using Nightingale.Utilitys;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Nightingale.Socials
{
	public class FacebookMananger : SingletonBehaviour<FacebookMananger>
	{
		private FacebookUser user;

		public FacebookRequestEvent LoginChanged = new FacebookRequestEvent();

		public UnityEvent LoginFaild = new UnityEvent();

		private IBaseSocial _social;

		public string UserId => GetSocialPlatform().UserId();

		public string TokenString => GetSocialPlatform().TokenString();

		private IBaseSocial GetSocialPlatform()
		{
			if (_social == null)
			{
				_social = new UnityFacebook();
			}
			return _social;
		}

		public void Initial(UnityAction callBack)
		{
			GetSocialPlatform().Init(callBack);
		}

		public void Login(UnityAction callBack = null)
		{
			if (IsInitialized())
			{
				GetSocialPlatform().Login(delegate
				{
					if (callBack != null)
					{
						callBack();
					}
					if (IsLogin())
					{
						LoginChanged.Invoke(arg0: true);
					}
					else
					{
						LoginFaild.Invoke();
					}
				});
			}
		}

		public void GetAppLink(UnityAction<IDictionary<string, object>> unityAction)
		{
			GetSocialPlatform().GetAppLink(unityAction);
		}

		public void Logout()
		{
			GetSocialPlatform().Logout();
			user = null;
			LoginChanged.Invoke(arg0: false);
		}

		public bool IsLogin()
		{
			return GetSocialPlatform().IsLogin();
		}

		public SocialPlatform GetPlatform()
		{
			return GetSocialPlatform().GetPlatform();
		}

		public bool IsInitialized()
		{
			return GetSocialPlatform().IsInitialized();
		}

		public void GetPlayerInfo(UnityAction<FacebookUser> unityAction)
		{
			if (unityAction != null && IsLogin())
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("fields", "id,name,picture.width(80).height(80)");
				Dictionary<string, string> parameters = dictionary;
				GetSocialPlatform().API("me", 0, parameters, delegate(string result)
				{
					if (!string.IsNullOrEmpty(result))
					{
						try
						{
							unityAction(FacebookUser.Parse(Json.Deserialize(result)));
						}
						catch (Exception ex)
						{
							UnityEngine.Debug.Log(ex.Message);
						}
					}
				});
			}
		}

		public void AppRequest(UnityAction<int> callBack, string message, IEnumerable<string> to, IEnumerable<object> filters = null, string data = "", string title = "")
		{
			if (IsInitialized() && IsLogin())
			{
				GetSocialPlatform().AppRequest(message, to, filters, data, title, callBack);
			}
		}

		public void AppRequest(UnityAction<int> callBack, string message, IEnumerable<string> to, string data = "", string title = "")
		{
			AppRequest(callBack, message, to, null, data, title);
		}

		public void AppInvite(UnityAction<int> callBack, string message, IEnumerable<string> to, string data = "", string title = "")
		{
			AppRequest(callBack, message, null, new string[1]
			{
				"app_non_users"
			}, data, title);
		}

		public IEnumerator GetAppRequests(UnityAction<object, float> unityAction)
		{
			yield return new WaitForSeconds(0.1f);
			if (IsLogin())
			{
				GetSocialPlatform().API("me/apprequests", 0, null, delegate(string result)
				{
					List<FacebookRequestData> list = new List<FacebookRequestData>();
					try
					{
						if (!string.IsNullOrEmpty(result))
						{
							Dictionary<string, object> dictionary = Json.Deserialize(result) as Dictionary<string, object>;
							if (dictionary.ContainsKey("data"))
							{
								List<object> list2 = dictionary["data"] as List<object>;
								foreach (object item in list2)
								{
									FacebookRequestData facebookRequestData = new FacebookRequestData();
									Dictionary<string, object> dictionary2 = item as Dictionary<string, object>;
									if (dictionary2.ContainsKey("id") && dictionary2.ContainsKey("message") && dictionary2.ContainsKey("from"))
									{
										facebookRequestData.id = dictionary2["id"].ToString();
										if (!SingletonData<FacebookRequestDataCache>.Get().Contains(facebookRequestData.id))
										{
											facebookRequestData.message = dictionary2["message"].ToString();
											if (dictionary2.ContainsKey("data") && dictionary2["data"] != null)
											{
												facebookRequestData.data = dictionary2["data"].ToString();
											}
											facebookRequestData.fromUser = FacebookUser.Parse(dictionary2["from"]);
											list.Add(facebookRequestData);
										}
									}
								}
							}
						}
					}
					catch (Exception ex)
					{
						UnityEngine.Debug.Log(ex.Message);
					}
					unityAction(list, 1f);
				});
			}
			else if (unityAction != null)
			{
				unityAction(new List<FacebookRequestData>(), 1f);
			}
		}

		public void ClearAppRequest(string requestId)
		{
			SingletonData<FacebookRequestDataCache>.Get().PutId(requestId);
			SingletonData<FacebookRequestDataCache>.Get().FlushData();
			GetSocialPlatform().API(requestId, 2);
		}

		public void GetFriendsInGame(UnityAction<List<FacebookUser>> unityAction, List<FacebookUser> users = null, string next = "")
		{
			try
			{
				if (unityAction != null)
				{
					if (users == null)
					{
						users = new List<FacebookUser>();
					}
					if (IsLogin())
					{
						Dictionary<string, string> dictionary = new Dictionary<string, string>();
						dictionary.Add("fields", "id,name,picture.width(80).height(80)");
						dictionary.Add("limit", "50");
						Dictionary<string, string> dictionary2 = dictionary;
						if (!string.IsNullOrEmpty(next))
						{
							dictionary2.Add("after", next);
						}
						GetSocialPlatform().API("me/friends", 0, dictionary2, delegate(string result)
						{
							try
							{
								Dictionary<string, object> dictionary3 = Json.Deserialize(result) as Dictionary<string, object>;
								List<object> list = dictionary3["data"] as List<object>;
								foreach (object item in list)
								{
									users.Add(FacebookUser.Parse(item));
								}
								if (!dictionary3.ContainsKey("paging") || list.Count <= 0)
								{
									goto IL_0102;
								}
								Dictionary<string, object> dictionary4 = dictionary3["paging"] as Dictionary<string, object>;
								dictionary4 = (dictionary4["cursors"] as Dictionary<string, object>);
								if (!dictionary4.ContainsKey("after"))
								{
									goto IL_0102;
								}
								string next2 = string.Format("{0}", dictionary4["after"]);
								string text = string.Format("{0}", dictionary4["before"]);
								GetFriendsInGame(unityAction, users, next2);
								goto end_IL_0000;
								IL_0102:
								unityAction(users);
								end_IL_0000:;
							}
							catch (Exception ex2)
							{
								UnityEngine.Debug.Log(ex2.Message);
							}
						});
					}
					else
					{
						unityAction(users);
					}
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
		}

		public void LogAppEvent(string logEvent, float? valueToSum = default(float?), Dictionary<string, object> parameters = null)
		{
			GetSocialPlatform().LogAppEvent(logEvent, valueToSum, parameters);
		}
	}
}
