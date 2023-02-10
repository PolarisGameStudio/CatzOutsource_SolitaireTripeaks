using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class NumberCard : BaseCard
	{
		public override int GetColor()
		{
			return (Config.Index - 1) / 13 % 2;
		}

		public override int GetNumber()
		{
			return (Config.Index - 1) % 13;
		}

		public override int GetSuit()
		{
			return (Config.Index - 1) / 13 % 4;
		}

		public override bool CalcClickMatch(BaseCard baseCard)
		{
			if (baseCard is NumberCard)
			{
				int number = baseCard.GetNumber();
				int number2 = GetNumber();
				if (Mathf.Abs(number - number2) == 1)
				{
					return true;
				}
				if ((number == 0 && number2 == 12) || (number == 12 && number2 == 0))
				{
					return true;
				}
			}
			return false;
		}

		public override void DestoryCollect(bool step)
		{
			PlayDesk.Get().RemoveCard(this);
			PlayDesk.Get().ClearCard();
			HandCardSystem.Get().FromDeskToRightHandCard(this);
			List<BaseCard> list2 = new List<BaseCard>();
			if (!PlayDesk.Get().CalcTopCard(list2) && step)
			{
				if (list2.Count > 0)
				{
					Collider[] source = Physics.OverlapBox(base.transform.position, GetHalfExtents(), base.transform.rotation);
					List<BaseCard> list = (from e in source
						select e.gameObject.GetComponent<BaseCard>()).ToList();
					list2.RemoveAll((BaseCard e) => !list.Contains(e));
				}
				OperatingHelper.Get().AppendStep(new MatchStep(list2));
			}
			PlayDesk.Get().DestopChanged();
			PlayDesk.Get().LinkOnce(base.transform.position);
		}
	}
}
