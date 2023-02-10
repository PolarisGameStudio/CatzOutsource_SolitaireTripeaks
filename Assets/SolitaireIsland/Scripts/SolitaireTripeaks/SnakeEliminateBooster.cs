using DG.Tweening;
using System.Linq;

namespace SolitaireTripeaks
{
	public class SnakeEliminateBooster : GlobalBooster
	{
		public override bool OpenPoker()
		{
			bool flag = PlayDesk.Get().Uppers.Count((BaseCard e) => e is SnakeCard || e is ForkCard) > 0;
			if (flag)
			{
				Sequence sequence = DOTween.Sequence();
				sequence.PrependInterval(1f);
				sequence.OnComplete(delegate
				{
					BaseCard[] array = (from e in PlayDesk.Get().Uppers
						where e is SnakeCard || e is ForkCard
						select e).ToArray();
					BaseCard[] array2 = array;
					foreach (BaseCard baseCard in array2)
					{
						baseCard.DestoryByBooster();
					}
				});
			}
			return flag;
		}
	}
}
