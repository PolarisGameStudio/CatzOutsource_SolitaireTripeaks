using DG.Tweening;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ButtonLightMask : MonoBehaviour
	{
		public Image LightImage;

		private Mask LightMask;

		public float Speed = 1.5f;

		private void Awake()
		{
			if (LightImage == null)
			{
				GameObject gameObject = new GameObject("Mask");
				gameObject.transform.SetParent(base.transform, worldPositionStays: false);
				LightImage = gameObject.AddComponent<Image>();
				LightImage.raycastTarget = false;
			}
			LightImage.transform.localScale = new Vector3(-1f, 1f, 1f);
			LightImage.sprite = SingletonBehaviour<LoaderUtility>.Get().GetAsset<Sprite>("Sprites/LightMask");
			LightImage.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 128);
			LightImage.SetNativeSize();
			LightMask = base.gameObject.GetComponent<Mask>();
			if (LightMask == null)
			{
				LightMask = base.gameObject.AddComponent<Mask>();
			}
			LightMask.showMaskGraphic = true;
			Vector2 sizeDelta = (base.transform as RectTransform).sizeDelta;
			float sizeDeltaW = sizeDelta.x * 1.2f;
			LightImage.rectTransform.anchoredPosition = new Vector2(0f - sizeDeltaW, 0f);
			Sequence sequence = DOTween.Sequence();
			sequence.Append(LightImage.rectTransform.DOAnchorPosX(sizeDeltaW, Speed));
			sequence.AppendCallback(delegate
			{
				LightImage.rectTransform.anchoredPosition = new Vector2(0f - sizeDeltaW, 0f);
				LightImage.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 128);
			});
			sequence.AppendInterval(5f);
			sequence.SetEase(Ease.Linear);
			sequence.SetLoops(-1);
		}

		public void SetActive(bool active)
		{
			if (!(LightImage == null))
			{
				LightImage.gameObject.SetActive(active);
				if (!(LightMask == null))
				{
					LightMask.enabled = active;
				}
			}
		}
	}
}
