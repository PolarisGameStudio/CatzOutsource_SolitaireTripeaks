using DG.Tweening;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class TextTipsUI : MonoBehaviour
	{
		public Text MessageLabel;

		public Image Background;

		public Image BackgroundClone;

		private void Awake()
		{
			AudioUtility.GetSound().Play("Audios/Booster.mp3");
		}

		public void SetNewText(string message, float delayTime = 0f, UnityAction unityAction = null)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.PrependInterval(delayTime);
			sequence.AppendCallback(delegate
			{
				MessageLabel.text = message;
			});
			sequence.Append(MessageLabel.transform.DOLocalMoveX(0f, 0.2f));
			sequence.OnComplete(delegate
			{
				if (unityAction != null)
				{
					unityAction();
				}
			});
		}

		public void SetDestory(float duration, UnityAction unityAction = null)
		{
			UnityEngine.Object.Destroy(base.gameObject, duration);
		}
	}
}
