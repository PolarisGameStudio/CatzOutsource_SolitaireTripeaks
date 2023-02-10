using DG.Tweening;
using Nightingale.Utilitys;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class BombSpine : PokerSpine
	{
		public List<ParticleSystemRenderer> Particles;

		public Canvas Canvas;

		public Text Label;

		public GameObject SparksParticle;

		private float localScale = 1f;

		private void Awake()
		{
			Vector3 vector = base.transform.localScale;
			localScale = vector.x;
		}

		public override void PlayDestroy(UnityAction unityAction)
		{
			if (SparksParticle != null)
			{
				UnityEngine.Object.Destroy(SparksParticle);
				SparksParticle = null;
			}
		}

		public override void PlayIndex(float index)
		{
			Label.text = $"{(int)index}";
			if (base.gameObject.activeInHierarchy)
			{
				Sequence s = DOTween.Sequence();
				float duration = 0.2f;
				int num = 1;
				if (index <= 4f)
				{
					num = 2;
					duration = 0.1f;
				}
				for (int i = 0; i < num; i++)
				{
					s.Append(base.transform.DOScale(1.1f * localScale, duration));
					s.AppendCallback(delegate
					{
						AudioUtility.GetSound().Play("Audios/Bomb_countdown.mp3");
					});
					s.Append(base.transform.DOScale(1f * localScale, duration));
					s.Append(base.transform.DOScale(1.05f * localScale, duration));
					s.AppendCallback(delegate
					{
						AudioUtility.GetSound().Play("Audios/Bomb_countdown.mp3");
					});
					s.Append(base.transform.DOScale(1f * localScale, duration));
					s.AppendInterval(0.1f);
				}
			}
		}

		public override void UpdateOrderLayer(int zIndex, int index)
		{
			base.UpdateOrderLayer(zIndex, index);
			Particles.ForEach(delegate(ParticleSystemRenderer e)
			{
				e.sortingOrder = zIndex + 6;
			});
			Canvas.sortingOrder = zIndex + 5;
		}

		public override void UpdateColor(bool white)
		{
			foreach (SpriteRenderer renderer in Renderers)
			{
				renderer.color = ((!white) ? Color.gray : Color.white);
			}
			if (SparksParticle != null)
			{
				SparksParticle.SetActive(white);
			}
		}
	}
}
