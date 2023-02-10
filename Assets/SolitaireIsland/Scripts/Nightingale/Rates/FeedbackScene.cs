using Nightingale.Azure;
using Nightingale.ScenesManager;
using Nightingale.UIExtensions;
using Nightingale.Utilitys;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nightingale.Rates
{
	public class FeedbackScene : BaseScene
	{
		[Header("取消按钮")]
		public Button _CancelButton;

		[Header("提交按钮")]
		public Button _SubmitButton;

		[Header("反馈细节")]
		public ToggleButton[] Feedbacks;

		public void OnStart(RateData rateData, UnityAction<RateType> unityAction)
		{
			_CancelButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Close(new NavigationEffect());
				SingletonClass<MySceneManager>.Get().Popup<RateScene>("RateScene", new NavigationEffect()).OnStart(unityAction);
			});
			_SubmitButton.onClick.AddListener(delegate
			{
				List<string> list = new List<string>();
				for (int i = 0; i < Feedbacks.Length; i++)
				{
					if (Feedbacks[i].IsOn)
					{
						list.Add(i.ToString());
					}
				}
				rateData.Reason = string.Join(",", list.ToArray());
				AzureTableStorage.GetOld().InsertOrMergeEntity("NightingaleAppRate", NightingaleConfig.Get().GetAppIdAndVersion(), SingletonClass<NightingaleSystemInfo>.Get().DeviceUniqueIdentifier, JsonUtility.ToJson(rateData));
				SingletonClass<MySceneManager>.Get().Close(null, delegate
				{
					if (unityAction != null)
					{
						unityAction(RateType.Submit);
					}
				});
			});
		}

		private Text GetButtonText(Button button)
		{
			return button.GetComponent<Text>();
		}
	}
}
