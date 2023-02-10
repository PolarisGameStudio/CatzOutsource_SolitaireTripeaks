using DG.Tweening;
using Nightingale.Utilitys;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	[RequireComponent(typeof(Button))]
	public class MiniCard : MonoBehaviour
	{
		public class CommodityConfig
		{
			public PurchasingCommodity Commodity;

			public int Weight;

			public CommodityConfig(PurchasingCommodity commodity, int weight)
			{
				Commodity = commodity;
				Weight = weight;
			}
		}

		public ImageUI BoosterImageUI;

		public Sprite BombSprite;

		public Sprite CoinSprite;

		public bool Opening
		{
			get;
			private set;
		}

		public PurchasingCommodity Commodity
		{
			get;
			private set;
		}

		public static CommodityConfig RandomObjectConfig(List<CommodityConfig> configs)
		{
			if (configs == null)
			{
				return null;
			}
			if (configs.Count == 0)
			{
				return null;
			}
			int num = UnityEngine.Random.Range(0, configs.Sum((CommodityConfig e) => e.Weight));
			for (int i = 0; i < configs.Count; i++)
			{
				num -= configs[i].Weight;
				if (num <= 0)
				{
					return configs[i];
				}
			}
			return configs[0];
		}

		private void Awake()
		{
			Opening = false;
			BoosterImageUI.gameObject.SetActive(value: false);
			base.transform.GetChild(0).GetComponent<Image>().sprite = PokerThemeGroup.Get().GetSpriteManager().GetSprite("back");
			base.transform.GetChild(1).GetComponent<Image>().sprite = PokerThemeGroup.Get().GetSpriteManager().GetSprite("front");
			base.transform.GetChild(1).transform.localScale = new Vector3(0f, 1f, 1f);
		}

		public void DOCollect(bool opening = true)
		{
			if (Opening)
			{
				return;
			}
			Opening = opening;
			MiniCard[] componentsInChildren = base.transform.parent.GetComponentsInChildren<MiniCard>();
			int num = componentsInChildren.Count((MiniCard e) => e.Opening);
			LevelRetrunCoinConfig levelRetrunCoinConfig = LevelRetrunCoinConfig.Read(SingletonClass<AAOConfig>.Get().GetPlaySchedule());
			int count = Mathf.Min(levelRetrunCoinConfig.LevelTicketCoins / 4, 500);
			int count2 = Mathf.Min(levelRetrunCoinConfig.LevelTicketCoins / 2 + 500, 1500);
			List<CommodityConfig> configs;
			if (opening)
			{
				switch (num)
				{
				default:
				{
					List<CommodityConfig> list = new List<CommodityConfig>();
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Coins,
						count = count
					}, 50));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Coins,
						count = count2
					}, 40));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Wild,
						count = 1
					}, 0));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = AppearNodeConfig.Get().GetRandomBooster(),
						count = 1
					}, 5));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.None
					}, 0));
					configs = list;
					break;
				}
				case 2:
				{
					List<CommodityConfig> list = new List<CommodityConfig>();
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Coins,
						count = count
					}, 60));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Coins,
						count = count2
					}, 50));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Wild,
						count = 1
					}, 3));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = AppearNodeConfig.Get().GetRandomBooster(),
						count = 1
					}, 5));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.None
					}, 0));
					configs = list;
					break;
				}
				case 3:
				{
					List<CommodityConfig> list = new List<CommodityConfig>();
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Coins,
						count = count
					}, 80));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Coins,
						count = count2
					}, 40));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Wild,
						count = 1
					}, 5));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = AppearNodeConfig.Get().GetRandomBooster(),
						count = 1
					}, 10));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.None
					}, 0));
					configs = list;
					break;
				}
				case 4:
				{
					List<CommodityConfig> list = new List<CommodityConfig>();
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Coins,
						count = count
					}, 80));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Coins,
						count = count2
					}, 40));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Wild,
						count = 1
					}, 5));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = AppearNodeConfig.Get().GetRandomBooster(),
						count = 1
					}, 10));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.None
					}, 90));
					configs = list;
					break;
				}
				case 5:
				{
					List<CommodityConfig> list = new List<CommodityConfig>();
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Coins,
						count = count
					}, 80));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Coins,
						count = count2
					}, 40));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Wild,
						count = 1
					}, 10));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = AppearNodeConfig.Get().GetRandomBooster(),
						count = 1
					}, 15));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.None
					}, 180));
					configs = list;
					break;
				}
				case 6:
				{
					List<CommodityConfig> list = new List<CommodityConfig>();
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Coins,
						count = count
					}, 5));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Coins,
						count = count2
					}, 5));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Wild,
						count = 1
					}, 10));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = AppearNodeConfig.Get().GetRandomBooster(),
						count = 1
					}, 20));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.None
					}, 150));
					configs = list;
					break;
				}
				case 7:
				{
					List<CommodityConfig> list = new List<CommodityConfig>();
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Coins,
						count = count
					}, 5));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Coins,
						count = count2
					}, 5));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Wild,
						count = 1
					}, 10));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = AppearNodeConfig.Get().GetRandomBooster(),
						count = 1
					}, 20));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.None
					}, 300));
					configs = list;
					break;
				}
				case 8:
				{
					List<CommodityConfig> list = new List<CommodityConfig>();
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Coins,
						count = count
					}, 5));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Coins,
						count = count2
					}, 5));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Wild,
						count = 1
					}, 10));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = AppearNodeConfig.Get().GetRandomBooster(),
						count = 1
					}, 20));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.None
					}, 400));
					configs = list;
					break;
				}
				case 9:
				{
					List<CommodityConfig> list = new List<CommodityConfig>();
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Coins,
						count = count
					}, 5));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Coins,
						count = count2
					}, 5));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Wild,
						count = 1
					}, 10));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = AppearNodeConfig.Get().GetRandomBooster(),
						count = 1
					}, 20));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.None
					}, 500));
					configs = list;
					break;
				}
				case 10:
				{
					List<CommodityConfig> list = new List<CommodityConfig>();
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Coins,
						count = count
					}, 5));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Coins,
						count = count2
					}, 5));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.Wild,
						count = 1
					}, 10));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = AppearNodeConfig.Get().GetRandomBooster(),
						count = 1
					}, 20));
					list.Add(new CommodityConfig(new PurchasingCommodity
					{
						boosterType = BoosterType.None
					}, 800));
					configs = list;
					break;
				}
				}
			}
			else if (base.transform.parent.GetComponentsInChildren<MiniCard>().Count((MiniCard e) => e.Commodity != null && e.Commodity.boosterType == BoosterType.None) < 3)
			{
				List<CommodityConfig> list = new List<CommodityConfig>();
				list.Add(new CommodityConfig(new PurchasingCommodity
				{
					boosterType = BoosterType.None,
					count = 1
				}, 10));
				configs = list;
			}
			else
			{
				List<CommodityConfig> list = new List<CommodityConfig>();
				list.Add(new CommodityConfig(new PurchasingCommodity
				{
					boosterType = BoosterType.Coins,
					count = count
				}, 50));
				list.Add(new CommodityConfig(new PurchasingCommodity
				{
					boosterType = BoosterType.Coins,
					count = count2
				}, 40));
				list.Add(new CommodityConfig(new PurchasingCommodity
				{
					boosterType = BoosterType.Wild,
					count = 1
				}, 50));
				list.Add(new CommodityConfig(new PurchasingCommodity
				{
					boosterType = AppearNodeConfig.Get().GetRandomBooster(),
					count = 1
				}, 50));
				configs = list;
			}
			Commodity = RandomObjectConfig(configs).Commodity;
			BoosterImageUI.transform.localScale = new Vector3(0f, 1f, 1f);
			BoosterImageUI.transform.localPosition = Vector3.zero;
			Sequence sequence = DOTween.Sequence();
			sequence.Append(base.transform.GetChild(0).DOScaleX(0f, 0.1f).SetEase(Ease.Linear));
			sequence.AppendCallback(delegate
			{
				if (Commodity.boosterType == BoosterType.None)
				{
					base.transform.GetChild(1).GetComponent<Image>().sprite = BombSprite;
				}
				else
				{
					BoosterImageUI.gameObject.SetActive(value: true);
					BoosterImageUI.SetLabel($"+{Commodity.count}");
					if (Commodity.boosterType == BoosterType.Wild || Commodity.boosterType == BoosterType.Rocket)
					{
						BoosterImageUI.transform.GetChild(0).localPosition = Vector3.zero;
						BoosterImageUI.SetImage(AppearNodeConfig.Get().GetBoosterSprite(Commodity.boosterType));
					}
					else if (Commodity.boosterType == BoosterType.Coins)
					{
						BoosterImageUI.SetNoNativeSizeImage(CoinSprite);
					}
					else
					{
						BoosterImageUI.SetNoNativeSizeImage(AppearNodeConfig.Get().GetBoosterSprite(Commodity.boosterType));
					}
				}
			});
			sequence.Append(base.transform.GetChild(1).DOScaleX(1f, 0.1f).SetEase(Ease.Linear));
			sequence.Join(BoosterImageUI.transform.DOScale(1f, 0.1f).SetEase(Ease.Linear));
			sequence.OnComplete(delegate
			{
				base.transform.localEulerAngles = Vector3.zero;
				if (opening && Commodity.boosterType == BoosterType.None)
				{
					AudioUtility.GetSound().Play("Audios/skeleton.mp3");
					GameObject gameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(GuessGame).Name, "Particles/SkullDeadParticle"));
					Transform transform = gameObject.transform;
					Vector3 position = base.transform.position;
					float x = position.x;
					Vector3 position2 = base.transform.position;
					float y = position2.y;
					Vector3 position3 = base.transform.position;
					transform.position = new Vector3(x, y, position3.z);
					UnityEngine.Object.Destroy(gameObject, 5f);
				}
			});
			sequence.SetEase(Ease.Linear);
		}

		public void Fade()
		{
			if (!Opening)
			{
				base.transform.Find("Booster/Light").gameObject.SetActive(value: false);
				Graphic[] componentsInChildren = GetComponentsInChildren<Graphic>();
				Graphic[] array = componentsInChildren;
				foreach (Graphic graphic in array)
				{
					graphic.color = Color.gray;
				}
				BoosterImageUI._Image.color = Color.gray;
				BoosterImageUI._Label.color = Color.gray;
			}
		}
	}
}
