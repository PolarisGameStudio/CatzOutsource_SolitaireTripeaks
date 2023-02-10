using System.Collections.Generic;

namespace com.adjust.sdk
{
	public class AdjustSessionSuccess
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

		public Dictionary<string, object> JsonResponse
		{
			get;
			set;
		}

		public AdjustSessionSuccess()
		{
		}

		public AdjustSessionSuccess(Dictionary<string, string> sessionSuccessDataMap)
		{
			if (sessionSuccessDataMap != null)
			{
				Adid = AdjustUtils.TryGetValue(sessionSuccessDataMap, AdjustUtils.KeyAdid);
				Message = AdjustUtils.TryGetValue(sessionSuccessDataMap, AdjustUtils.KeyMessage);
				Timestamp = AdjustUtils.TryGetValue(sessionSuccessDataMap, AdjustUtils.KeyTimestamp);
				string aJSON = AdjustUtils.TryGetValue(sessionSuccessDataMap, AdjustUtils.KeyJsonResponse);
				JSONNode jSONNode = JSON.Parse(aJSON);
				if (jSONNode != null && jSONNode.AsObject != null)
				{
					JsonResponse = new Dictionary<string, object>();
					AdjustUtils.WriteJsonResponseDictionary(jSONNode.AsObject, JsonResponse);
				}
			}
		}

		public AdjustSessionSuccess(string jsonString)
		{
			JSONNode jSONNode = JSON.Parse(jsonString);
			if (!(jSONNode == null))
			{
				Adid = AdjustUtils.GetJsonString(jSONNode, AdjustUtils.KeyAdid);
				Message = AdjustUtils.GetJsonString(jSONNode, AdjustUtils.KeyMessage);
				Timestamp = AdjustUtils.GetJsonString(jSONNode, AdjustUtils.KeyTimestamp);
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
