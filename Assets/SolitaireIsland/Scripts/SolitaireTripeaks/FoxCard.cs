using DG.Tweening;
using Nightingale.Extensions;
using Nightingale.Localization;
using Nightingale.Utilitys;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class FoxCard : BaseCard
	{
		protected override string GetFont()
		{
			return "Prefabs/Pokers/FoxPoker";
		}

		public override void CollectedToRightHand()
		{
			AudioUtility.GetSound().Play("Audios/Fox.mp3");
			if (Object.FindObjectOfType<BellaBlessingBooster>() != null)
			{
				SingletonBehaviour<Effect2DUtility>.Get().CreateBoosterUseEffectUI(BoosterType.BellaBlessing);
			}
			else
			{
				SingletonBehaviour<Effect2DUtility>.Get().CreateTextTipsUI(LocalizationUtility.Get().GetString("Fox Tips"));
			}
			OperatingHelper.Get().ClearStep();
			PlayDesk.Get().AppendBusyTime(0.5f);
			this.DelayDo(new WaitForSeconds(0.5f), delegate
			{
				if (PlayDesk.Get() != null && !PlayDesk.Get().IsGameOver)
				{
					HandCardSystem.Get().AppendLeftCards(3, delegate
					{
						if (HandCardSystem.Get()._RightHandGroup.GetTop() is FoxCard)
						{
							BaseCard flyCard = HandCardSystem.Get().FlyRightCard();
							Vector3 position = base.transform.position;
							position.y -= 3f;
							flyCard.UpdateOrderLayer(32667);
							flyCard.transform.DOMove(position, 0.5f);
							flyCard.transform.DORotate(new Vector3(30f, 0f, 30f), 0.5f).OnComplete(delegate
							{
								UnityEngine.Object.Destroy(flyCard.gameObject);
								PlayDesk.Get().DestopChanged();
							});
						}
					});
				}
			});
			base.CollectedToRightHand();
		}

		public override bool StayInTop()
		{
			return false;
		}

		public override bool CalcClickMatch(BaseCard baseCard)
		{
			return true;
		}
	}
}
