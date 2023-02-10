using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nightingale.Rates
{
	public class FeatureRateScene : BaseRateScene
	{
		[Header("关闭按钮")]
		public Button _CloseButton;

		[Header("评价按钮")]
		public Button _RateButton;

		public override void OnStart(UnityAction<RateType> unityAction)
		{
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
			_RateButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Close(null, delegate
				{
					if (unityAction != null)
					{
						unityAction(RateType.Close);
					}
				});
				PlatformUtility.OnRateUs();
			});
		}
	}
}
