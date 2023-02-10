using Nightingale.Extensions;
using Nightingale.Utilitys;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class BagBoosterScene : SoundScene
	{
		public Transform Content;

		public Transform NoPackage;

		private void Awake()
		{
			GameObject asset = SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(BagBoosterScene).Name, "UI/BagBoosterItem");
			foreach (BoosterType item in from e in AppearNodeConfig.Get().GetAllPackBoosters()
				orderby PackData.Get().GetCommodity(e).GetTotal() descending
				select e)
			{
				GameObject gameObject = Object.Instantiate(asset, Content);
				gameObject.transform.Find("Icon").GetComponent<Image>().sprite = AppearNodeConfig.Get().GetBoosterMiniSprite(item);
				gameObject.transform.Find("Account/Text").GetComponent<Text>().text = PackData.Get().GetCommodity(item).GetTotal()
					.ToString();
				if (AppearNodeConfig.Get().IsUsable(item))
				{
					gameObject.transform.Find("lock").gameObject.SetActive(value: false);
				}
				else
				{
					gameObject.transform.GetComponentsInChildren<Image>().ForEach(delegate(Image e)
					{
						e.color = Color.gray;
					});
					gameObject.transform.GetComponentsInChildren<Text>().ForEach(delegate(Text e)
					{
						e.color = Color.gray;
					});
					gameObject.transform.Find("lock").gameObject.SetActive(value: true);
				}
			}
			NoPackage.gameObject.SetActive(Content.childCount == 0);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			SingletonBehaviour<LoaderUtility>.Get().UnLoadScene(typeof(BagBoosterScene).Name);
		}
	}
}
