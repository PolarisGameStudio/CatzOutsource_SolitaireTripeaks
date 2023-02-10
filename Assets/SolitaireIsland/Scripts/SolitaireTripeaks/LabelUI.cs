using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class LabelUI : MonoBehaviour
	{
		public Text Label;

		private bool isdestory;

		private void OnDestroy()
		{
			isdestory = true;
		}

		public void SetActive(bool visable)
		{
			if (base.gameObject.activeSelf != visable)
			{
				base.gameObject.SetActive(visable);
			}
		}

		public void SetString(string text)
		{
			Label.text = text;
		}

		public void CrossFadeAlpha(float duration)
		{
			Graphic[] componentsInChildren = base.transform.GetComponentsInChildren<Graphic>();
			Graphic[] array = componentsInChildren;
			foreach (Graphic graphic in array)
			{
				graphic.CrossFadeAlpha(0f, duration, ignoreTimeScale: true);
			}
			Shadow[] componentsInChildren2 = base.transform.GetComponentsInChildren<Shadow>();
			Shadow[] array2 = componentsInChildren2;
			foreach (Shadow item in array2)
			{
				Color effectColor = item.effectColor;
				float alpha = effectColor.a;
				DOTween.To(() => alpha, delegate(float vaule)
				{
					if (!isdestory)
					{
						Color effectColor2 = item.effectColor;
						effectColor2.a = vaule;
						item.effectColor = effectColor2;
					}
				}, 0f, duration);
			}
		}
	}
}
