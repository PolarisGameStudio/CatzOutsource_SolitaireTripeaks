using DG.Tweening;
using Nightingale.Localization;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class CommonGuideUtility : MonoBehaviour
	{
		public RectTransform RoleTransform;

		public Text DescLabel;

		public void CloseGuide(UnityAction unityAction = null)
		{
			base.transform.DOScaleY(0f, 0.2f).OnComplete(delegate
			{
				SingletonBehaviour<GlobalConfig>.Get().TimeScale = 1f;
				UnityEngine.Object.Destroy(base.gameObject);
				if (unityAction != null)
				{
					unityAction();
				}
			});
		}

		public void ChangeGuide(string text, float showTime = 0f, UnityAction unityAction = null)
		{
			SingletonBehaviour<GlobalConfig>.Get().TimeScale = 0f;
			Sequence sequence = DOTween.Sequence();
			sequence.Append(DescLabel.rectTransform.DOAnchorPosX(960f, 0.4f));
			sequence.AppendCallback(delegate
			{
				DescLabel.text = text;
			});
			sequence.Append(DescLabel.rectTransform.DOAnchorPosX(-600f, 0.4f));
			sequence.AppendInterval(showTime);
			sequence.OnComplete(delegate
			{
				if (unityAction != null)
				{
					unityAction();
				}
			});
		}

		public void CreateGuide(string text, float showTime, UnityAction unityAction)
		{
			SingletonBehaviour<GlobalConfig>.Get().TimeScale = 0f;
			DescLabel.text = text;
			DescLabel.rectTransform.anchoredPosition = new Vector2(960f, 0f);
			base.transform.localScale = new Vector3(1f, 0f, 1f);
			Sequence sequence = DOTween.Sequence();
			sequence.Append(base.transform.DOScaleY(1f, 0.2f));
			sequence.Append(DescLabel.rectTransform.DOAnchorPosX(-600f, 0.4f));
			sequence.AppendInterval(showTime);
			sequence.OnComplete(delegate
			{
				if (unityAction != null)
				{
					unityAction();
				}
			});
		}

		public static CommonGuideUtility CreateCommonGuideUtility(Transform transform)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, "HelpGuide/CommonGuide"));
			gameObject.transform.SetParent(transform.Find("Canvas"), worldPositionStays: false);
			return gameObject.GetComponent<CommonGuideUtility>();
		}

		public static CommonGuideUtility CreateCommonGuideUtilitySnake(Transform transform)
		{
			GameObject gameObject = null;
			SystemLanguage language = LocalizationUtility.GetLanguage();
			gameObject = ((language != SystemLanguage.French && language != SystemLanguage.German && language != SystemLanguage.Japanese && language != SystemLanguage.Korean && language != SystemLanguage.Spanish) ? UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, "HelpGuide/SnakeGuide")) : UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, "HelpGuide/SnakeGuideDefault")));
			gameObject.transform.SetParent(transform.Find("Canvas"), worldPositionStays: false);
			return gameObject.GetComponent<CommonGuideUtility>();
		}

		public static CommonGuideUtility CreateCommonGuideUtilityBy(Transform transform)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, "HelpGuide/CommonGuide"));
			gameObject.transform.SetParent(transform, worldPositionStays: false);
			return gameObject.GetComponent<CommonGuideUtility>();
		}
	}
}
