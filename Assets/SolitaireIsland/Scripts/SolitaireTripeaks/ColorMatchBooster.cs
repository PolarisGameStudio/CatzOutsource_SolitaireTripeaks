using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class ColorMatchBooster : NormalBooster
	{
		private List<BaseCard> freeTops;

		protected override void Init(int number)
		{
			TipPokerSystem.Get().IsRuning = false;
			OperatingHelper.Get().ClearStep();
			freeTops = (from baseCard in PlayDesk.Get().Uppers
				where IsMatch(baseCard)
				select baseCard).ToList();
			Invoke("AutoRemove", 0.5f);
		}

		protected override bool IsMatch(BaseCard baseCard)
		{
			if (baseCard is NumberCard)
			{
				return baseCard.GetColor() == base.Number;
			}
			return false;
		}

		private void AutoRemove()
		{
			if (PlayDesk.Get() != null && PlayDesk.Get().IsPlaying)
			{
				if (freeTops.Count > 0)
				{
					BaseCard baseCard = freeTops.FirstOrDefault((BaseCard e) => e.HasExtras(ExtraType.Skeleton));
					if (baseCard == null)
					{
						baseCard = freeTops[Random.Range(0, freeTops.Count)];
					}
					freeTops.Remove(baseCard);
					baseCard.DestoryByColor();
					Invoke("AutoRemove", 0.5f);
					return;
				}
				TipPokerSystem.Get().IsRuning = true;
				PlayDesk.Get().DestopChanged();
				HandCardSystem.Get().CheckRightHandCard();
				OperatingHelper.Get().ClearStep();
			}
			UnityEngine.Object.Destroy(base.gameObject);
			CancelInvoke("AutoRemove");
		}
	}
}
