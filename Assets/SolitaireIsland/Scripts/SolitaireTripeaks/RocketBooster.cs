using DG.Tweening;
using Nightingale.Extensions;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class RocketBooster : NormalBooster
	{
		private int LaserCount;

		protected override void Init(int number)
		{
			TipPokerSystem.Get().IsRuning = false;
			OperatingHelper.Get().ClearStep();
			LaserCount = 7;
			this.DelayDo(new WaitForSeconds(0.5f), AutoRemove);
		}

		protected override bool IsMatch(BaseCard baseCard)
		{
			if (baseCard is ForkCard)
			{
				return PlayDesk.Get().Uppers.Count((BaseCard e) => e is SnakeCard) >= PlayDesk.Get().Uppers.Count((BaseCard e) => e is ForkCard);
			}
			if (baseCard is SnakeCard)
			{
				return PlayDesk.Get().Uppers.Count((BaseCard e) => e is ForkCard) >= PlayDesk.Get().Uppers.Count((BaseCard e) => e is SnakeCard);
			}
			return baseCard is NumberCard;
		}

		private void AutoRemove()
		{
			if (SingletonClass<MySceneManager>.Get().Count() > 1)
			{
				this.DelayDo(new WaitForSeconds(0.5f), AutoRemove);
			}
			else
			{
				if (!(PlayDesk.Get() != null) || !PlayDesk.Get().IsPlaying)
				{
					return;
				}
				BaseCard[] array = (from baseCard in PlayDesk.Get().Uppers
					where IsMatch(baseCard)
					select baseCard).ToArray();
				if (LaserCount > 0 && array.Length > 0)
				{
					Vector3 position = HandCardSystem.Get()._RightHandGroup.transform.position;
					BaseCard baseCard2 = array.FirstOrDefault((BaseCard e) => e.HasExtras(ExtraType.Skeleton));
					if (baseCard2 == null)
					{
						baseCard2 = array[Random.Range(0, array.Length)];
					}
					if (baseCard2 is ForkCard || baseCard2 is SnakeCard)
					{
						baseCard2.DestoryByRocket();
						this.DelayDo(new WaitForSeconds(0.5f), AutoRemove);
						return;
					}
					GameObject g = Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, "Prefabs/RocketEffect"));
					g.transform.position = position;
					Vector3 up = Vector3.up;
					Vector3 toDirection = baseCard2.transform.position - g.transform.position;
					g.transform.rotation = Quaternion.FromToRotation(up, toDirection);
					AudioUtility.GetSound().Play("Audios/Rocket_Fly.mp3");
					Sequence sequence = DOTween.Sequence();
					sequence.Append(g.transform.DOMove(baseCard2.transform.position, 0.6f));
					sequence.OnComplete(delegate
					{
						UnityEngine.Object.Destroy(g);
						AudioUtility.GetSound().Play("Audios/Rock_Bomb.mp3");
						GameObject gameObject = Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, "Particles/RocketBombParticle"));
						gameObject.transform.position = baseCard2.transform.position;
						UnityEngine.Object.Destroy(gameObject, 2f);
						baseCard2.DestoryByRocket();
						AutoRemove();
					});
					sequence.SetEase(Ease.Linear);
					LaserCount--;
				}
				else
				{
					TipPokerSystem.Get().IsRuning = true;
					PlayDesk.Get().DestopChanged();
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}
	}
}
