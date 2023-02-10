using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class GlobalBooster : MonoBehaviour
	{
		public BoosterType boosterType;

		private bool showing;

		public virtual void OnStart(UnityAction unityAction)
		{
			unityAction?.Invoke();
		}

		public virtual bool OpenPoker()
		{
			return false;
		}

		public virtual void ShowEffectEveryOnce(UnityAction unityAction)
		{
			if (!showing)
			{
				showing = true;
				SingletonBehaviour<Effect2DUtility>.Get().CreateBoosterUseEffectUI(boosterType, delegate
				{
					showing = false;
					if (unityAction != null)
					{
						unityAction();
					}
				});
				AudioUtility.GetSound().Play("Audios/Booster.mp3");
			}
		}

		public virtual int MultipleStreaks()
		{
			return 1;
		}

		public virtual void ClosePoker()
		{
		}
	}
}
