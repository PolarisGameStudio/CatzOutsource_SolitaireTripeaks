using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class BoosterUseEffectUI : MonoBehaviour
	{
		public Image BackgroundImage;

		public Image BackgroundCloneImage;

		public Image IconImage;

		public Text TitleLabel;

		public void OnStart(BoosterType boosterType, UnityAction unityAction)
		{
			TitleLabel.text = AppearNodeConfig.Get().GetBoosterTitle(boosterType).ToUpper() + "!!";
			IconImage.sprite = AppearNodeConfig.Get().GetBoosterSprite(boosterType);
			IconImage.SetNativeSize();
			BackgroundCloneImage.DOFade(1f, 0.3f).SetEase(Ease.Linear);
			BackgroundImage.transform.localPosition = new Vector3(-1920f, 0f, 0f);
			BackgroundImage.transform.DOLocalMoveX(1920f, 0.5f).SetEase(Ease.Linear);
			IconImage.transform.localPosition = new Vector3(-525f, 0f, 0f);
			IconImage.transform.DOLocalMoveX(-250f, 1f).SetEase(Ease.Linear);
			TitleLabel.transform.localPosition = new Vector3(-140f, 0f, 0f);
			TitleLabel.transform.DOLocalMoveX(-90f, 1f).OnComplete(delegate
			{
				if (unityAction != null)
				{
					unityAction();
				}
				UnityEngine.Object.Destroy(base.gameObject);
			}).SetEase(Ease.Linear);
		}
	}
}
