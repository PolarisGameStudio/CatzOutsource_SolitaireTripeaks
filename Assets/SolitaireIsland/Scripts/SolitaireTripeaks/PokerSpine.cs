using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class PokerSpine : BaseSpine
	{
		public Animation _PokerAnimator;

		[SerializeField]
		protected List<SpriteRenderer> Renderers;

		public override void PlayActivation(UnityAction unityAction)
		{
			string text = "Activation";
			AnimationClip clip = _PokerAnimator.GetClip(text);
			if (!(clip == null))
			{
				_PokerAnimator.Play(text);
				DelayDo(new WaitForSeconds(clip.length), unityAction);
			}
		}

		public override void PlayDestroy(UnityAction unityAction)
		{
			string text = "Destroy";
			AnimationClip clip = _PokerAnimator.GetClip(text);
			if (!(clip == null))
			{
				_PokerAnimator.Play(text);
				DelayDo(new WaitForSeconds(clip.length), unityAction);
				PlayDesk.Get().AppendBusyTime(clip.length);
			}
		}

		public override void PlayIndex(float index)
		{
		}

		public override void UpdateOrderLayer(int zIndex, int index)
		{
			int num = 1;
			foreach (SpriteRenderer renderer in Renderers)
			{
				num++;
				renderer.sortingOrder = zIndex + index + num;
			}
		}

		public override void UpdateColor(bool white)
		{
			foreach (SpriteRenderer renderer in Renderers)
			{
				renderer.color = ((!white) ? Color.gray : Color.white);
			}
		}
	}
}
