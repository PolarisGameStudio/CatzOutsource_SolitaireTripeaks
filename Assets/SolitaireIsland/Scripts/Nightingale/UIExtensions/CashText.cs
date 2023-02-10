using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Nightingale.UIExtensions
{
	[ExecuteInEditMode]
	public class CashText : MonoBehaviour
	{
		public Text Text;

		private long Number;

		private bool destroy;

		private void OnDestroy()
		{
			destroy = true;
		}

		public void SetCash(long number, bool animator = false)
		{
			DOTween.Kill(base.gameObject.GetInstanceID());
			if (number >= 1000000000)
			{
				Text.text = $"{(float)number / 1E+09f:N2}B";
			}
			else if (number >= 10000000)
			{
				Text.text = $"{(float)number / 1000000f:N2}M";
			}
			else if (animator)
			{
				float duration = 1.8f;
				if (Number > number)
				{
					duration = 1f;
				}
				DOTween.To(() => Number, delegate(long coin)
				{
					if (!destroy)
					{
						Text.text = $"{coin:N0}";
						Number = number;
					}
				}, number, duration).SetId(base.gameObject.GetInstanceID());
			}
			else
			{
				Number = number;
				Text.text = $"{number:N0}";
			}
		}

		public void SetSmallCash(long number)
		{
			if (number >= 1000000000)
			{
				Text.text = $"{(float)number / 1E+09f:N1}B";
			}
			else if (number >= 1000000)
			{
				Text.text = $"{(float)number / 1000000f:N1}M";
			}
			else if (number >= 10000)
			{
				Text.text = $"{(float)number / 1000f:N1}k";
			}
			else
			{
				Text.text = $"{number:N0}";
			}
		}

		public void SetLeaderBoardCash(long number)
		{
			if (number >= 1000000000)
			{
				Text.text = $"{(float)number / 1E+09f:N1}B";
			}
			else if (number >= 1000000)
			{
				Text.text = $"{(float)number / 1000000f:N1}M";
			}
			else
			{
				Text.text = $"{number:N0}";
			}
		}
	}
}
