using Nightingale.Utilitys;
using System.Collections.Generic;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class BatterExtra : DebuffExtra
	{
		private const int HighMax = 5;

		private const int LowMax = 3;

		private List<int> Links = new List<int>();

		protected int LinkIndex;

		protected override void StartInitialized()
		{
			base.StartInitialized();
			int num2 = LinkIndex = (base.Index = 5);
		}

		public override void OnRemoveCard()
		{
			Links.Add(LinkIndex);
			if (LinkIndex > 0)
			{
				LinkIndex--;
				AudioUtility.GetSound().Play("Audios/batter_destory.ogg");
				if (LinkIndex <= 3)
				{
					base.Index = 3;
				}
			}
			Link(LinkIndex);
		}

		public override void OnHandChange()
		{
			base.OnHandChange();
			Links.Add(LinkIndex);
			if (LinkIndex < base.Index)
			{
				LinkIndex++;
				AudioUtility.GetSound().Play("Audios/vine_grow.ogg");
			}
			Link(LinkIndex);
		}

		public override void OnUndo(bool match)
		{
			base.OnUndo(match);
			LinkIndex = Links[Links.Count - 1];
			Links.RemoveAt(Links.Count - 1);
			Link(LinkIndex);
		}

		public override bool IsFree()
		{
			return LinkIndex <= 0;
		}

		protected override string PokerPrefab()
		{
			return "Prefabs/Extras/BatterSpine";
		}

		protected void Link(int linkCount)
		{
			UnityEngine.Debug.LogWarning("当前龟壳牌的值是：" + linkCount);
			base.PokerSpine.PlayIndex(linkCount);
		}

		public override void DestoryByBooster()
		{
			OperatingHelper.Get().ClearStep();
			baseCard.RemoveExtra(this);
			RemoveAnimtor(delegate
			{
				UnityEngine.Object.Destroy(base.gameObject);
				PlayDesk.Get().CalcTopCard();
				PlayDesk.Get().DestopChanged();
			});
		}

		public override bool DestoryByColor()
		{
			return DestoryByRocket();
		}

		public override bool DestoryByGolden()
		{
			return DestoryByRocket();
		}

		public override bool DestoryByMatch(BaseCard card)
		{
			if (IsFree())
			{
				baseCard.RemoveExtra(this);
				UnityEngine.Object.Destroy(base.gameObject);
				OperatingHelper.Get().ClearStep();
				baseCard.DestoryCollect(step: false);
			}
			return true;
		}

		public override bool DestoryByRocket()
		{
			if (IsFree())
			{
				OperatingHelper.Get().ClearStep();
				baseCard.DestoryCollect(step: false);
				baseCard.RemoveExtra(this);
				RemoveAnimtor(delegate
				{
					UnityEngine.Object.Destroy(base.gameObject);
				});
			}
			else
			{
				OperatingHelper.Get().ClearStep();
				baseCard.RemoveExtra(this);
				UnityEngine.Object.Destroy(base.gameObject);
				baseCard.DestoryCollect(step: false);
			}
			return true;
		}
	}
}
