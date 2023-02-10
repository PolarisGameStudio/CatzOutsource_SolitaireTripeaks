using DG.Tweening;
using Nightingale.Utilitys;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class HelpScene : SoundScene
	{
		public Transform[] viewTransforms;

		private int currentPageIndex;

		private bool isAnimtor;

		private const float moveDurtion = 0.5f;

		protected override void OnDestroy()
		{
			base.OnDestroy();
			SingletonBehaviour<LoaderUtility>.Get().UnLoadScene(typeof(HelpScene).Name);
		}

		private void Start()
		{
			viewTransforms[0].gameObject.SetActive(value: true);
			for (int i = 1; i < viewTransforms.Length; i++)
			{
				viewTransforms[i].gameObject.SetActive(value: false);
			}
		}

		public void OnNextClick()
		{
			if (!isAnimtor)
			{
				isAnimtor = true;
				Transform current = viewTransforms[currentPageIndex];
				current.DOLocalMoveX(-1300f, 0.5f).OnComplete(delegate
				{
					current.gameObject.SetActive(value: false);
					isAnimtor = false;
				});
				currentPageIndex++;
				currentPageIndex %= viewTransforms.Length;
				Transform transform = viewTransforms[currentPageIndex];
				transform.gameObject.SetActive(value: true);
				transform.localPosition = new Vector3(1300f, 0f, 0f);
				transform.DOLocalMoveX(0f, 0.5f);
			}
		}

		public void OnPreClick()
		{
			if (!isAnimtor)
			{
				isAnimtor = true;
				Transform current = viewTransforms[currentPageIndex];
				current.DOLocalMoveX(1300f, 0.5f).OnComplete(delegate
				{
					current.gameObject.SetActive(value: false);
					isAnimtor = false;
				});
				currentPageIndex--;
				currentPageIndex += viewTransforms.Length;
				currentPageIndex %= viewTransforms.Length;
				Transform transform = viewTransforms[currentPageIndex];
				transform.gameObject.SetActive(value: true);
				transform.localPosition = new Vector3(-1300f, 0f, 0f);
				transform.DOLocalMoveX(0f, 0.5f);
			}
		}
	}
}
