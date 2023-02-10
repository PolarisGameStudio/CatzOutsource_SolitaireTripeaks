using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class BoosterNumberUI : MonoBehaviour
	{
		public BoosterType Type;

		public string StringFormat = "{0}";

		public Text Label;

		private void Awake()
		{
			if (Label == null)
			{
				Label = GetComponent<Text>();
			}
			CommodityChanged(CommoditySource.None);
			PackData.Get().GetCommodity(Type).OnChanged.AddListener(CommodityChanged);
		}

		private void OnDestroy()
		{
			PackData.Get().GetCommodity(Type).OnChanged.RemoveListener(CommodityChanged);
		}

		private void CommodityChanged(CommoditySource source)
		{
			Label.text = string.Format(StringFormat, PackData.Get().GetCommodity(Type).GetTotal());
		}
	}
}
