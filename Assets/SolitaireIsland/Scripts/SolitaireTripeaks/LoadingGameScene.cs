using DG.Tweening;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class LoadingGameScene : GlobalLoadingAnimation
	{
		public const string LoadingGameScenePath = "Scenes/LoadingGameScene";

		private const float WallDuration = 0.6f;

		private const float GrossDuration = 0.8f;

		public RectTransform LeftTransform;

		public RectTransform RightTransform;

		public RectTransform TopTransform;

		public RectTransform BottomTransform;

		[SerializeField]
		private Image _betweenBG;

		protected override void OnOpenAnimation(UnityAction unityAction)
		{
            AudioUtility.GetSound().Play("Audios/loading_open.mp3");
#if DENIS_VERSION
			//_betweenBG.color = new Color(1, 1, 1, 1);
			Sequence sequence = DOTween.Sequence();
			sequence.Append(_betweenBG.rectTransform.DOAnchorPos(new Vector2(0, 0), 0.6f));
			sequence.SetEase(Ease.Linear);
			sequence.OnComplete(delegate
			{
				//_betweenBG.gameObject.SetActive(false);
				unityAction?.Invoke();
			});
#else
            Sequence sequence = DOTween.Sequence();
			sequence.Append(LeftTransform.DOAnchorPos(new Vector2(105f, 0f), 0.6f));
			sequence.Join(RightTransform.DOAnchorPos(new Vector2(-341f, 0f), 0.6f));
			sequence.Join(TopTransform.DOAnchorPos(Vector2.zero, 0.8f));
			sequence.Join(BottomTransform.DOAnchorPos(Vector2.zero, 0.8f));
			sequence.SetEase(Ease.Linear);
			sequence.OnComplete(delegate
			{
				unityAction?.Invoke();
			});
#endif
        }

        protected override void OnClosedAnimation(UnityAction unityAction)
		{
            AudioUtility.GetSound().Play("Audios/loading_close.mp3");
#if DENIS_VERSION
            //_betweenBG.gameObject.SetActive(true);
			//_betweenBG.color = new Color(1, 1, 1, 0);
            Sequence sequence = DOTween.Sequence();
			sequence.Append(_betweenBG.rectTransform.DOAnchorPos(new Vector2(0, 2000), 0.6f));
            sequence.SetEase(Ease.Linear);
            sequence.OnComplete(delegate
            {
                unityAction?.Invoke();
            });
#else
            Sequence sequence = DOTween.Sequence();
			sequence.Append(LeftTransform.DOAnchorPos(new Vector2(-960f, 0f), 0.6f));
			sequence.Join(RightTransform.DOAnchorPos(new Vector2(960f, 0f), 0.6f));
			sequence.Join(TopTransform.DOAnchorPos(new Vector2(0f, 380f), 0.8f));
			sequence.Join(BottomTransform.DOAnchorPos(new Vector2(0f, -380f), 0.8f));
			sequence.SetEase(Ease.Linear);
			sequence.OnComplete(delegate
			{
				unityAction?.Invoke();
			});
#endif
        }
    }
}
