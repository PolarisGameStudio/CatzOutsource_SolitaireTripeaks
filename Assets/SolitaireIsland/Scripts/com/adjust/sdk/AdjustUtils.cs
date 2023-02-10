using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.adjust.sdk
{
	public class AdjustUtils
	{
		public static string KeyAdid = "adid";

		public static string KeyMessage = "message";

		public static string KeyNetwork = "network";

		public static string KeyAdgroup = "adgroup";

		public static string KeyCampaign = "campaign";

		public static string KeyCreative = "creative";

		public static string KeyWillRetry = "willRetry";

		public static string KeyTimestamp = "timestamp";

		public static string KeyCallbackId = "callbackId";

		public static string KeyEventToken = "eventToken";

		public static string KeyClickLabel = "clickLabel";

		public static string KeyTrackerName = "trackerName";

		public static string KeyTrackerToken = "trackerToken";

		public static string KeyJsonResponse = "jsonResponse";

		public static string KeyTestOptionsBaseUrl = "baseUrl";

		public static string KeyTestOptionsGdprUrl = "gdprUrl";

		public static string KeyTestOptionsBasePath = "basePath";

		public static string KeyTestOptionsGdprPath = "gdprPath";

		public static string KeyTestOptionsDeleteState = "deleteState";

		public static string KeyTestOptionsUseTestConnectionOptions = "useTestConnectionOptions";

		public static string KeyTestOptionsTimerIntervalInMilliseconds = "timerIntervalInMilliseconds";

		public static string KeyTestOptionsTimerStartInMilliseconds = "timerStartInMilliseconds";

		public static string KeyTestOptionsSessionIntervalInMilliseconds = "sessionIntervalInMilliseconds";

		public static string KeyTestOptionsSubsessionIntervalInMilliseconds = "subsessionIntervalInMilliseconds";

		public static string KeyTestOptionsTeardown = "teardown";

		public static string KeyTestOptionsNoBackoffWait = "noBackoffWait";

		public static string KeyTestOptionsiAdFrameworkEnabled = "iAdFrameworkEnabled";

		public static int ConvertLogLevel(AdjustLogLevel? logLevel)
		{
			if (!logLevel.HasValue)
			{
				return -1;
			}
			return (int)logLevel.Value;
		}

		public static int ConvertBool(bool? value)
		{
			if (!value.HasValue)
			{
				return -1;
			}
			if (value.Value)
			{
				return 1;
			}
			return 0;
		}

		public static double ConvertDouble(double? value)
		{
			if (!value.HasValue)
			{
				return -1.0;
			}
			return value.Value;
		}

		public static long ConvertLong(long? value)
		{
			if (!value.HasValue)
			{
				return -1L;
			}
			return value.Value;
		}

		public static string ConvertListToJson(List<string> list)
		{
			if (list == null)
			{
				return null;
			}
			JSONArray jSONArray = new JSONArray();
			foreach (string item in list)
			{
				jSONArray.Add(new JSONData(item));
			}
			return jSONArray.ToString();
		}

		public static string GetJsonResponseCompact(Dictionary<string, object> dictionary)
		{
			string empty = string.Empty;
			if (dictionary == null)
			{
				return empty;
			}
			int num = 0;
			empty += "{";
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				string text = item.Value as string;
				if (text != null)
				{
					if (++num > 1)
					{
						empty += ",";
					}
					if (text.StartsWith("{") && text.EndsWith("}"))
					{
						string text2 = empty;
						empty = text2 + "\"" + item.Key + "\":" + text;
					}
					else
					{
						string text2 = empty;
						empty = text2 + "\"" + item.Key + "\":\"" + text + "\"";
					}
				}
				else
				{
					Dictionary<string, object> dictionary2 = item.Value as Dictionary<string, object>;
					if (++num > 1)
					{
						empty += ",";
					}
					empty = empty + "\"" + item.Key + "\":";
					empty += GetJsonResponseCompact(dictionary2);
				}
			}
			return empty + "}";
		}

		public static string GetJsonString(JSONNode node, string key)
		{
			if (node == null)
			{
				return null;
			}
			JSONData jSONData = node[key] as JSONData;
			if (jSONData == null)
			{
				return null;
			}
			if (jSONData == string.Empty)
			{
				return null;
			}
			return jSONData.Value;
		}

		public static void WriteJsonResponseDictionary(JSONClass jsonObject, Dictionary<string, object> output)
		{
			IEnumerator enumerator = jsonObject.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, JSONNode> keyValuePair = (KeyValuePair<string, JSONNode>)enumerator.Current;
					JSONClass asObject = keyValuePair.Value.AsObject;
					string key = keyValuePair.Key;
					if (asObject == null)
					{
						string value = keyValuePair.Value.Value;
						output.Add(key, value);
					}
					else
					{
						Dictionary<string, object> dictionary = new Dictionary<string, object>();
						output.Add(key, dictionary);
						WriteJsonResponseDictionary(asObject, dictionary);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		public static string TryGetValue(Dictionary<string, string> dictionary, string key)
		{
			if (dictionary.TryGetValue(key, out string value))
			{
				if (value == string.Empty)
				{
					return null;
				}
				return value;
			}
			return null;
		}

		public static AndroidJavaObject TestOptionsMap2AndroidJavaObject(Dictionary<string, string> testOptionsMap, AndroidJavaObject ajoCurrentActivity)
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.adjust.sdk.AdjustTestOptions");
			androidJavaObject.Set("baseUrl", testOptionsMap[KeyTestOptionsBaseUrl]);
			androidJavaObject.Set("gdprUrl", testOptionsMap[KeyTestOptionsGdprUrl]);
			if (testOptionsMap.ContainsKey(KeyTestOptionsBasePath) && !string.IsNullOrEmpty(testOptionsMap[KeyTestOptionsBasePath]))
			{
				androidJavaObject.Set("basePath", testOptionsMap[KeyTestOptionsBasePath]);
			}
			if (testOptionsMap.ContainsKey(KeyTestOptionsGdprPath) && !string.IsNullOrEmpty(testOptionsMap[KeyTestOptionsGdprPath]))
			{
				androidJavaObject.Set("gdprPath", testOptionsMap[KeyTestOptionsGdprPath]);
			}
			if (testOptionsMap.ContainsKey(KeyTestOptionsDeleteState) && ajoCurrentActivity != null)
			{
				androidJavaObject.Set("context", ajoCurrentActivity);
			}
			if (testOptionsMap.ContainsKey(KeyTestOptionsUseTestConnectionOptions))
			{
				bool flag = testOptionsMap[KeyTestOptionsUseTestConnectionOptions].ToLower() == "true";
				AndroidJavaObject val = new AndroidJavaObject("java.lang.Boolean", flag);
				androidJavaObject.Set("useTestConnectionOptions", val);
			}
			if (testOptionsMap.ContainsKey(KeyTestOptionsTimerIntervalInMilliseconds))
			{
				long num = long.Parse(testOptionsMap[KeyTestOptionsTimerIntervalInMilliseconds]);
				AndroidJavaObject val2 = new AndroidJavaObject("java.lang.Long", num);
				androidJavaObject.Set("timerIntervalInMilliseconds", val2);
			}
			if (testOptionsMap.ContainsKey(KeyTestOptionsTimerStartInMilliseconds))
			{
				long num2 = long.Parse(testOptionsMap[KeyTestOptionsTimerStartInMilliseconds]);
				AndroidJavaObject val3 = new AndroidJavaObject("java.lang.Long", num2);
				androidJavaObject.Set("timerStartInMilliseconds", val3);
			}
			if (testOptionsMap.ContainsKey(KeyTestOptionsSessionIntervalInMilliseconds))
			{
				long num3 = long.Parse(testOptionsMap[KeyTestOptionsSessionIntervalInMilliseconds]);
				AndroidJavaObject val4 = new AndroidJavaObject("java.lang.Long", num3);
				androidJavaObject.Set("sessionIntervalInMilliseconds", val4);
			}
			if (testOptionsMap.ContainsKey(KeyTestOptionsSubsessionIntervalInMilliseconds))
			{
				long num4 = long.Parse(testOptionsMap[KeyTestOptionsSubsessionIntervalInMilliseconds]);
				AndroidJavaObject val5 = new AndroidJavaObject("java.lang.Long", num4);
				androidJavaObject.Set("subsessionIntervalInMilliseconds", val5);
			}
			if (testOptionsMap.ContainsKey(KeyTestOptionsTeardown))
			{
				bool flag2 = testOptionsMap[KeyTestOptionsTeardown].ToLower() == "true";
				AndroidJavaObject val6 = new AndroidJavaObject("java.lang.Boolean", flag2);
				androidJavaObject.Set("teardown", val6);
			}
			if (testOptionsMap.ContainsKey(KeyTestOptionsNoBackoffWait))
			{
				bool flag3 = testOptionsMap[KeyTestOptionsNoBackoffWait].ToLower() == "true";
				AndroidJavaObject val7 = new AndroidJavaObject("java.lang.Boolean", flag3);
				androidJavaObject.Set("noBackoffWait", val7);
			}
			return androidJavaObject;
		}
	}
}
