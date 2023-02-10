using DG.Tweening;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nightingale.Extensions
{
	public class SelectableEffect : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		public SelectableEffectType selectableEffectType;

		public string AudioClipFileName = "Audios/button.mp3";

		public void OnPointerDown(PointerEventData eventData)
		{
			Button component = base.transform.GetComponent<Button>();
			if (!(component != null) || component.interactable)
			{
				Sequence s = DOTween.Sequence();
				switch (selectableEffectType)
				{
				case SelectableEffectType.ScaleY:
					s.Append(base.transform.DOScaleY(1.1f, 0.12f));
					s.Append(base.transform.DOScaleY(0.85f, 0.09f));
					s.Append(base.transform.DOScaleY(1f, 0.09f));
					break;
				case SelectableEffectType.ScaleXY:
					s.Append(base.transform.DOScaleX(0.95f, 0.1f));
					s.Join(base.transform.DOScaleY(1.05f, 0.1f));
					s.Append(base.transform.DOScaleX(1.05f, 0.1f));
					s.Join(base.transform.DOScaleY(0.95f, 0.1f));
					s.Append(base.transform.DOScaleX(1f, 0.1f));
					s.Join(base.transform.DOScaleY(1f, 0.1f));
					break;
				}
				AudioUtility.GetSound().Play(AudioClipFileName);
			}
		}
	}
}
