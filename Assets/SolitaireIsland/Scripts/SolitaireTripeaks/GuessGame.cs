using DG.Tweening;
using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class GuessGame : BaseScene
	{
		private bool isOveing;

		private List<MiniCard> miniCards;

		private void Awake()
		{
			base.IsStay = true;
			MenuUITopLeft.CreateMenuUITopLeft(base.transform);
			Transform child = base.transform.GetChild(0).GetChild(1);
			List<Transform> list = new List<Transform>();
			for (int i = 0; i < child.childCount; i++)
			{
				list.Add(child.GetChild(i));
			}
			int num = UnityEngine.Random.Range(0, list.Count);
			for (int j = 0; j < list.Count; j++)
			{
				list[j].gameObject.SetActive(value: true);
				if (j != num)
				{
					UnityEngine.Object.Destroy(list[j].gameObject);
				}
			}
			miniCards = (from e in list[num].GetComponentsInChildren<MiniCard>()
				orderby e.transform.GetSiblingIndex()
				select e).ToList();
			foreach (MiniCard miniCard in miniCards)
			{
				miniCard.GetComponent<Button>().onClick.AddListener(delegate
				{
					if (!isOveing)
					{
						miniCard.GetComponent<Button>().onClick.RemoveAllListeners();
						AudioUtility.GetSound().Play("Audios/guess_click.ogg");
						miniCard.DOCollect();
						CheckOver();
					}
				});
			}
			DelayDo(delegate
			{
				TipPopupNoIconScene.ShowTitleDescription(LocalizationUtility.Get("Localization_mini_game.json").GetString("Treasure_Title"), LocalizationUtility.Get("Localization_mini_game.json").GetString("Pick Cards for Treasure"));
			});
		}

		public void CheckOver()
		{
			if (!isOveing && (miniCards.Count((MiniCard e) => !e.Opening) == 0 || miniCards.Count((MiniCard e) => e.Commodity != null && e.Commodity.boosterType == BoosterType.None) > 0))
			{
				isOveing = true;
				miniCards.ForEach(delegate(MiniCard miniCard)
				{
					miniCard.GetComponent<Button>().onClick.RemoveAllListeners();
				});
				DOCompleted((from e in miniCards
					where e.Opening && e.Commodity.boosterType != BoosterType.None
					select e).ToList());
			}
		}

		private void DOCompleted(List<MiniCard> cards, float seconds = 1f)
		{
			SetCanvasGraphicRaycaster(enabled: false);
			DelayDo(new WaitForSeconds(seconds), delegate
			{
				(from e in miniCards
					where !e.Opening
					orderby Guid.NewGuid()
					select e).ToList().ForEach(delegate(MiniCard e)
				{
					e.DOCollect(opening: false);
				});
				DelayDo(new WaitForSeconds(1f), delegate
				{
					(from e in miniCards
						where !e.Opening
						select e).ToList().ForEach(delegate(MiniCard miniCard)
					{
						Vector3 position = miniCard.transform.position;
						position.x += 1.5f;
						position.y -= 6f;
						miniCard.transform.DOMove(position, 0.8f).OnComplete(delegate
						{
							UnityEngine.Object.Destroy(miniCard.gameObject);
						});
						miniCard.transform.DORotate(UnityEngine.Random.insideUnitSphere * 360f * 3f, 4f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
					});
					DelayDo(new WaitForSeconds(1f), delegate
					{
						if (SingletonClass<DayActivityHelper>.Get().HasDayActivty(DayActivityType.DoubleTreause))
						{
							SingletonBehaviour<Effect2DUtility>.Get().CreateTextTipsUI("Lucky Zoe, Double Reward", 80f, delegate
							{
								cards.ForEach(delegate(MiniCard miniCard)
								{
									Transform child = miniCard.transform.GetChild(2).GetChild(2);
									child.gameObject.SetActive(value: true);
									child.transform.localScale = Vector3.one * 5f;
									child.transform.DOScale(0.7f, 0.2f);
								});
								DelayDo(new WaitForSeconds(1f), delegate
								{
									CollectMiniCards(cards);
								});
							});
						}
						else
						{
							CollectMiniCards(cards);
						}
					});
				});
			});
		}

		private void CollectMiniCards(List<MiniCard> cards)
		{
			DelayDo(new WaitForSeconds(0.5f), delegate
			{
				List<PurchasingCommodity> list = new List<PurchasingCommodity>();
				foreach (MiniCard card in cards)
				{
					if (SingletonClass<DayActivityHelper>.Get().HasDayActivty(DayActivityType.DoubleTreause))
					{
						card.Commodity.count *= 2;
					}
					PurchasingCommodity purchasingCommodity = list.Find((PurchasingCommodity e) => e.boosterType == card.Commodity.boosterType);
					if (purchasingCommodity == null)
					{
						purchasingCommodity = new PurchasingCommodity
						{
							boosterType = card.Commodity.boosterType
						};
						list.Add(purchasingCommodity);
					}
					purchasingCommodity.count += card.Commodity.count;
					SessionData.Get().PutCommodity(card.Commodity.boosterType, CommoditySource.Free, card.Commodity.count, changed: false);
				}
				PurchasSuccessPopup.ShowPurchasSuccessPopup(list.ToArray(), delegate
				{
					ScheduleData scheduleData = SingletonClass<AAOConfig>.Get().GetPlaySchedule();
					GlobalLoadingAnimation.Show("Scenes/LoadingGameScene", delegate
					{
						IslandScene scene = IslandScene.Create(scheduleData.world);
						scene.OnStart(scheduleData.world, scheduleData.chapter);
						scene.AddLoadListener(delegate(bool success)
						{
							if (!success)
							{
								scene.JumpTo(scheduleData);
							}
						});
						return scene;
					});
				});
			});
		}
	}
}
