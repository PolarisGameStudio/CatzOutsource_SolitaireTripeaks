using Nightingale.U2D;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class SellPokerUI : MonoBehaviour
	{
		public Text Label;

		public DoubleSpriteButton SelectButton;

		public void OnStart(GameObject Prefab)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Prefab);
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
		}

		public void OnSelect(bool visable)
		{
		}
	}
}
