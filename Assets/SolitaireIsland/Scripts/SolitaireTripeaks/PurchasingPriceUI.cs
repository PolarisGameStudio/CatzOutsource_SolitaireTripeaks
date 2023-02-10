using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class PurchasingPriceUI : MonoBehaviour
	{
		public string id;

		public string Format;

		private void Awake()
		{
			Repeating();
		}

		private void Repeating()
		{
			if (SingletonBehaviour<UnityPurchasingHelper>.Get().IsInited)
			{
				if (string.IsNullOrEmpty(Format))
				{
					Format = "{0}";
				}
				GetComponent<Text>().text = string.Format(Format, UnityPurchasingConfig.Get().GetLocalizedPriceString(id));
			}
			else
			{
				Invoke("Repeating", 1f);
			}
		}
	}
}
