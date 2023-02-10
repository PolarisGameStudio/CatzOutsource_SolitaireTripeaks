using Nightingale.Utilitys;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Nightingale.Azure
{
	public class AzureTableStorage : MonoBehaviour
	{
		private string StorageAccount;

		private string StorageKey;

		private static AzureTableStorage _oldStorage;

		private static AzureTableStorage _newStorage;

		public static AzureTableStorage GetOld()
		{
			if (_oldStorage == null)
			{
				GameObject gameObject = new GameObject();
				_oldStorage = gameObject.AddComponent<AzureTableStorage>();
				_oldStorage.StorageAccount = NightingaleConfig.Get().StorageAccount;
				_oldStorage.StorageKey = NightingaleConfig.Get().StorageKey;
				UnityEngine.Object.DontDestroyOnLoad(_oldStorage);
			}
			return _oldStorage;
		}

		public static AzureTableStorage GetNew()
		{
			if (_newStorage == null)
			{
				GameObject gameObject = new GameObject();
				_newStorage = gameObject.AddComponent<AzureTableStorage>();
				_newStorage.StorageAccount = NightingaleConfig.Get().NewStorageAccount;
				_newStorage.StorageKey = NightingaleConfig.Get().NewStorageKey;
				UnityEngine.Object.DontDestroyOnLoad(_newStorage);
			}
			return _newStorage;
		}

		public UnityWebRequest CreateTable(string tableName, UnityAction<DownloadHandler> unityAction = null)
		{
			string requestBody = "{'TableName':'" + tableName + "'}";
			string url = $"https://{StorageAccount}.table.core.windows.net/Tables";
			string method = "POST";
			UnityWebRequest unityWebRequest = CreateUnityWebRequest(url, method, requestBody);
			StartCoroutine(StartUnityWeb(unityWebRequest, unityAction));
			return unityWebRequest;
		}

		public UnityWebRequest DeleteTable(string tableName, UnityAction<DownloadHandler> unityAction = null)
		{
			string url = $"https://{StorageAccount}.table.core.windows.net/Tables('{tableName}')";
			string method = "DELETE";
			UnityWebRequest unityWebRequest = CreateUnityWebRequest(url, method);
			StartCoroutine(StartUnityWeb(unityWebRequest, unityAction));
			return unityWebRequest;
		}

		public UnityWebRequest QueryEntities(string tableName, string filter, string[] selects = null, UnityAction<DownloadHandler> unityAction = null)
		{
			string text = $"https://{StorageAccount}.table.core.windows.net/{tableName}()?$filter={filter}";
			if (selects != null)
			{
				text = text + "&$select=" + string.Join(", ", selects);
			}
			string method = "GET";
			UnityWebRequest unityWebRequest = CreateUnityWebRequest(text, method);
			StartCoroutine(StartUnityWeb(unityWebRequest, unityAction));
			return unityWebRequest;
		}

		public UnityWebRequest QueryEntities(string tableName, string filter, string[] selects = null, int top = 100, UnityAction<DownloadHandler> unityAction = null)
		{
			string text = $"https://{StorageAccount}.table.core.windows.net/{tableName}()?$filter={filter}";
			if (selects != null)
			{
				text = text + "&$select=" + string.Join(", ", selects);
			}
			text = text + "&$top=" + top;
			string method = "GET";
			UnityWebRequest unityWebRequest = CreateUnityWebRequest(text, method);
			StartCoroutine(StartUnityWeb(unityWebRequest, unityAction));
			return unityWebRequest;
		}

		public UnityWebRequest QueryEntities(string tableName, string[] partitionKeys, string rowKey, string[] selects = null, UnityAction<DownloadHandler> unityAction = null)
		{
			string empty = string.Empty;
			empty = $"((PartitionKey%20eq%20'{partitionKeys[0]}')";
			for (int i = 1; i < partitionKeys.Length; i++)
			{
				empty += $"%20or%20(PartitionKey%20eq%20'{partitionKeys[i]}')";
			}
			empty += $")%20and%20(RowKey%20eq%20'{rowKey}')";
			string text = $"https://{StorageAccount}.table.core.windows.net/{tableName}()?$filter={empty}";
			if (selects != null)
			{
				text = text + "&$select=" + string.Join(",", selects);
			}
			string method = "GET";
			UnityWebRequest unityWebRequest = CreateUnityWebRequest(text, method);
			StartCoroutine(StartUnityWeb(unityWebRequest, unityAction));
			return unityWebRequest;
		}

		public UnityWebRequest QueryEntities(string tableName, string[] partitionKeys, string[] selects = null, UnityAction<DownloadHandler> unityAction = null)
		{
			string empty = string.Empty;
			empty = $"((PartitionKey%20eq%20'{partitionKeys[0]}')";
			for (int i = 1; i < partitionKeys.Length; i++)
			{
				empty += $"%20or%20(PartitionKey%20eq%20'{partitionKeys[i]}')";
			}
			empty += ")";
			string text = $"https://{StorageAccount}.table.core.windows.net/{tableName}()?$filter={empty}";
			if (selects != null)
			{
				text = text + "&$select=" + string.Join(",", selects);
			}
			string method = "GET";
			UnityWebRequest unityWebRequest = CreateUnityWebRequest(text, method);
			StartCoroutine(StartUnityWeb(unityWebRequest, unityAction));
			return unityWebRequest;
		}

		public UnityWebRequest GetEntities(string tableName, string partitionKey, string[] selects = null, UnityAction<DownloadHandler> unityAction = null)
		{
			string empty = string.Empty;
			empty = $"(PartitionKey%20eq%20'{partitionKey}')";
			string text = $"https://{StorageAccount}.table.core.windows.net/{tableName}()?$filter={empty}";
			if (selects != null)
			{
				text = text + "&$select=" + string.Join(",", selects);
			}
			string method = "GET";
			UnityWebRequest unityWebRequest = CreateUnityWebRequest(text, method);
			StartCoroutine(StartUnityWeb(unityWebRequest, unityAction));
			return unityWebRequest;
		}

		public UnityWebRequest GetEntity(string tableName, string partitionKey, string rowKey, string[] selects = null, UnityAction<DownloadHandler> unityAction = null)
		{
			string text = $"https://{StorageAccount}.table.core.windows.net/{tableName}(PartitionKey='{partitionKey}',RowKey='{rowKey}')";
			if (selects != null)
			{
				text = text + "?$select=" + string.Join(",", selects);
			}
			string method = "GET";
			UnityWebRequest unityWebRequest = CreateUnityWebRequest(text, method);
			StartCoroutine(StartUnityWeb(unityWebRequest, unityAction));
			return unityWebRequest;
		}

		public UnityWebRequest InsertOrReplaceEntity(string tableName, string partitionKey, string rowKey, string jsonBody, UnityAction<DownloadHandler> unityAction = null)
		{
			string url = $"https://{StorageAccount}.table.core.windows.net/{tableName}(PartitionKey='{partitionKey}',RowKey='{rowKey}')";
			string method = "PUT";
			UnityWebRequest unityWebRequest = CreateUnityWebRequest(url, method, jsonBody);
			StartCoroutine(StartUnityWeb(unityWebRequest, unityAction));
			return unityWebRequest;
		}

		public UnityWebRequest InsertOrMergeEntity(string tableName, string partitionKey, string rowKey, string jsonBody, UnityAction<DownloadHandler> unityAction = null)
		{
			string url = $"https://{StorageAccount}.table.core.windows.net/{tableName}(PartitionKey='{partitionKey}',RowKey='{rowKey}')";
			string method = "POST";
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("X-HTTP-Method", "MERGE");
			UnityWebRequest unityWebRequest = CreateUnityWebRequest(url, method, jsonBody, dictionary);
			StartCoroutine(StartUnityWeb(unityWebRequest, unityAction));
			return unityWebRequest;
		}

		public UnityWebRequest InsertOrMergeEntity(string tableName, string partitionKey, string rowKey, Dictionary<string, object> bodys, UnityAction<DownloadHandler> unityAction = null)
		{
			string text = "{";
			foreach (string key in bodys.Keys)
			{
				text = ((!(bodys[key] is string)) ? (text + $"\"{key}\":{bodys[key]},") : (text + $"\"{key}\":\"{bodys[key].ToString()}\","));
			}
			text = text.Substring(0, text.Length - 1);
			text += "}";
			string url = $"https://{StorageAccount}.table.core.windows.net/{tableName}(PartitionKey='{partitionKey}',RowKey='{rowKey}')";
			string method = "POST";
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("X-HTTP-Method", "MERGE");
			UnityWebRequest unityWebRequest = CreateUnityWebRequest(url, method, text, dictionary);
			StartCoroutine(StartUnityWeb(unityWebRequest, unityAction));
			return unityWebRequest;
		}

		public UnityWebRequest DeleteEntity(string tableName, string partitionKey, string rowKey, UnityAction<DownloadHandler> unityAction = null)
		{
			string url = $"https://{StorageAccount}.table.core.windows.net/{tableName}(PartitionKey='{partitionKey}',RowKey='{rowKey}')";
			string method = "DELETE";
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("If-Match", "*");
			UnityWebRequest unityWebRequest = CreateUnityWebRequest(url, method, null, dictionary);
			StartCoroutine(StartUnityWeb(unityWebRequest, unityAction));
			return unityWebRequest;
		}

		private IEnumerator StartUnityWeb(UnityWebRequest unityWebRequest, UnityAction<DownloadHandler> unityAction)
		{
			yield return unityWebRequest.SendWebRequest();
			unityAction?.Invoke(unityWebRequest.downloadHandler);
		}

		private UnityWebRequest CreateUnityWebRequest(string url, string method, string requestBody = null, Dictionary<string, string> headers = null)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(url, method);
			if (headers == null)
			{
				headers = new Dictionary<string, string>();
			}
			headers.Add("x-ms-date", SystemTime.Now.ToString("R", CultureInfo.InvariantCulture));
			headers.Add("x-ms-version", "2015-12-11");
			headers.Add("Accept", "application/json;odata=nometadata");
			foreach (KeyValuePair<string, string> header in headers)
			{
				unityWebRequest.SetRequestHeader(header.Key, header.Value);
			}
			string stringToSign = string.Format("{0}\n\n{1}\n{2}\n{3}", method, "application/json", SystemTime.Now.ToString("R", CultureInfo.InvariantCulture), GetCanonicalizedResource(new Uri(url), StorageAccount));
			stringToSign = "SharedKey " + StorageAccount + ":" + Sign(Convert.FromBase64String(StorageKey), stringToSign);
			unityWebRequest.SetRequestHeader("Authorization", stringToSign);
			byte[] data = null;
			if (!string.IsNullOrEmpty(requestBody))
			{
				data = Encoding.UTF8.GetBytes(requestBody);
			}
			unityWebRequest.uploadHandler = new UploadHandlerRaw(data);
			unityWebRequest.uploadHandler.contentType = "application/json";
			unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
			return unityWebRequest;
		}

		private string GetCanonicalizedResource(Uri address, string accountName)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder("/");
			stringBuilder2.Append(accountName);
			stringBuilder2.Append(address.AbsolutePath);
			stringBuilder.Append(stringBuilder2.ToString());
			return stringBuilder.ToString();
		}

		private string Sign(byte[] key, string stringToSign)
		{
			HMACSHA256 hMACSHA = new HMACSHA256();
			hMACSHA.Key = key;
			byte[] inArray = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
			return Convert.ToBase64String(inArray);
		}
	}
}
