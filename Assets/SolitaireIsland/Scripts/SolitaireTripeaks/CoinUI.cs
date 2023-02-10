using Nightingale.UIExtensions;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class CoinUI : MonoBehaviour
	{
		public CashText Label;

		private void Start()
		{
			Label.SetCash(PackData.Get().GetCommodity(BoosterType.Coins).GetTotal());
			PackData.Get().GetCommodity(BoosterType.Coins).OnChanged.AddListener(UpdateUI);
		}

		private void OnDestroy()
		{
			PackData.Get().GetCommodity(BoosterType.Coins).OnChanged.RemoveListener(UpdateUI);
		}

		private void UpdateUI(CommoditySource commoditySource)
		{
			Label.SetCash(PackData.Get().GetCommodity(BoosterType.Coins).GetTotal(), animator: true);
		}
	}
}
