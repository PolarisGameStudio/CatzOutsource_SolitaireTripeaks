using System.Collections.Generic;

namespace com.adjust.sdk
{
	public class AdjustEventSuccess
	{
		public string Adid
		{
			get;
			set;
		}

		public string Message
		{
			get;
			set;
		}

		public string Timestamp
		{
			get;
			set;
		}

		public string EventToken
		{
			get;
			set;
		}

		public string CallbackId
		{
			get;
			set;
		}

		public Dictionary<string, object> JsonResponse
		{
			get;
			set;
		}

		public AdjustEventSuccess()
		{
		}

		public AdjustEventSuccess(Dictionary<string, string> eventSuccessDataMap)
		{
			if (eventSuccessDataMap != null)
			{
				Adid = AdjustUtils.TryGetValue(eventSuccessDataMap, AdjustUtils.KeyAdid);
				Message = AdjustUtils.TryGetValue(eventSuccessDataMap, AdjustUtils.KeyMessage);
				Timestamp = AdjustUtils.TryGetValue(eventSuccessDataMap, AdjustUtils.KeyTimestamp);
				EventToken = AdjustUtils.TryGetValue(eventSuccessDataMap, AdjustUtils.KeyEventToken);
				CallbackId = AdjustUtils.TryGetValue(eventSuccessDataMap, AdjustUtils.KeyCallbackId);
				string aJSON = AdjustUtils.TryGetValue(eventSuccessDataMap, AdjustUtils.KeyJsonResponse);
				JSONNode jSONNode = JSON.Parse(aJSON);
				if (jSONNode != null && jSONNode.AsObject != null)
				{
					JsonResponse = new Dictionary<string, object>();
					AdjustUtils.WriteJsonResponseDictionary(jSONNode.AsObject, JsonResponse);
				}
			}
		}

		public AdjustEventSuccess(string jsonString)
		{
			JSONNode jSONNode = JSON.Parse(jsonString);
			if (!(jSONNode == null))
			{
				Adid = AdjustUtils.GetJsonString(jSONNode, AdjustUtils.KeyAdid);
				Message = AdjustUtils.GetJsonString(jSONNode, AdjustUtils.KeyMessage);
				Timestamp = AdjustUtils.GetJsonString(jSONNode, AdjustUtils.KeyTimestamp);
				EventToken = AdjustUtils.GetJsonString(jSONNode, AdjustUtils.KeyEventToken);
				CallbackId = AdjustUtils.GetJsonString(jSONNode, AdjustUtils.KeyCallbackId);
				JSONNode jSONNode2 = jSONNode[AdjustUtils.KeyJsonResponse];
				if (!(jSONNode2 == null) && !(jSONNode2.AsObject == null))
				{
					JsonResponse = new Dictionary<string, object>();
					AdjustUtils.WriteJsonResponseDictionary(jSONNode2.AsObject, JsonResponse);
				}
			}
		}

		public void BuildJsonResponseFromString(string jsonResponseString)
		{
			JSONNode jSONNode = JSON.Parse(jsonResponseString);
			if (!(jSONNode == null))
			{
				JsonResponse = new Dictionary<string, object>();
				AdjustUtils.WriteJsonResponseDictionary(jSONNode.AsObject, JsonResponse);
			}
		}

		public string GetJsonResponse()
		{
			return AdjustUtils.GetJsonResponseCompact(JsonResponse);
		}
	}
}
