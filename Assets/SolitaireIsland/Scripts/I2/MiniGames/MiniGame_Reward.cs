using UnityEngine;

namespace I2.MiniGames
{
	[AddComponentMenu("I2/MiniGames/TreasureHunt/Reward")]
	public class MiniGame_Reward : MonoBehaviour
	{
		public float Probability = 1f;

		public bool _AttachToCaller = true;

		public bool _EndGame;

		public UnityEventTreasureHunt _OnRewarded = new UnityEventTreasureHunt();

		public virtual void Execute(MiniGame game, Transform parent)
		{
			Show(parent);
			_OnRewarded.Invoke(this);
		}

		public virtual void Show(Transform parent)
		{
			base.gameObject.SetActive(value: true);
			if (_AttachToCaller)
			{
				base.transform.position = parent.position;
			}
		}

		public virtual void Hide()
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
