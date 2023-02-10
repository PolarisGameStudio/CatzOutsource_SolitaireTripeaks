using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ProgressBarUI : MonoBehaviour
	{
		public Image _Image;

		private Tween tween;

		public void SetFillAmount(float amount)
		{
			_Image.fillAmount = amount;
		}

		public void UpdateFillAmount(float amount, float time = 0.5f, TweenCallback tweenCallback = null)
		{
			if (tween != null)
			{
				tween.Kill();
			}
			amount = Mathf.Min(amount, 1f);
			float fillAmount = _Image.fillAmount;
			tween = DOTween.To(() => fillAmount, delegate(float vaule)
			{
				_Image.fillAmount = vaule;
			}, amount, time).OnComplete(tweenCallback);
		}
	}
}
