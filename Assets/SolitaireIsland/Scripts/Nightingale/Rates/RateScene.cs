using Nightingale.Azure;
using Nightingale.ScenesManager;
using Nightingale.UIExtensions;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nightingale.Rates
{
	public class RateScene : BaseRateScene
	{
		[Header("描述文本")]
		public Text _DesLabel;

		[Header("关闭按钮")]
		public Button _CloseButton;

		[Header("评价按钮")]
		public Button _RateButton;

		[Header("评星")]
		public ToggleButton[] stars;

		[Header("评星 对应文字")]
		public GameObject[] _Labels;

		private int _StarCount;

		public override void OnStart(UnityAction<RateType> unityAction)
		{
			for (int i = 0; i < stars.Length; i++)
			{
				int temp = i;
				stars[i].onChanged.AddListener(delegate
				{
					UpdateStar(temp);
				});
			}
			_CloseButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Close(null, delegate
				{
					if (unityAction != null)
					{
						unityAction(RateType.Close);
					}
				});
			});
			_RateButton.interactable = false;
			_RateButton.onClick.AddListener(delegate
			{
				RateData data = new RateData
				{
					Star = _StarCount + 1
				};
				if (_StarCount + 1 >= 5)
				{
					SingletonBehaviour<AppActiveUtility>.Get().Append(delegate(float time)
					{
						if (!(time < 4f))
						{
							SingletonClass<MySceneManager>.Get().Close(null, delegate
							{
								if (unityAction != null)
								{
									unityAction(RateType.Rate);
								}
							});
							AzureTableStorage.GetOld().InsertOrReplaceEntity("NightingaleAppRate", NightingaleConfig.Get().GetAppIdAndVersion(), SingletonClass<NightingaleSystemInfo>.Get().DeviceUniqueIdentifier, JsonUtility.ToJson(data));
						}
					});
					PlatformUtility.OnRateUs();
				}
				else
				{
					SingletonClass<MySceneManager>.Get().Close(new NavigationEffect());
					SingletonClass<MySceneManager>.Get().Popup<FeedbackScene>("FeedbackScene", new NavigationEffect()).OnStart(data, unityAction);
				}
			});
		}

		private void UpdateStar(int star)
		{
			_StarCount = star;
			for (int i = 0; i < stars.Length; i++)
			{
				stars[i].SetState(i <= star);
				_Labels[i].SetActive(i == star);
			}
			_RateButton.interactable = true;
			_DesLabel.gameObject.SetActive(value: false);
		}
	}
}
