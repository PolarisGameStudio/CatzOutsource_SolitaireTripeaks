using DG.Tweening;
using Nightingale.Extensions;
using Nightingale.Inputs;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class HandCardSystem : MonoBehaviour
	{
		private static HandCardSystem _HandCardSystem;

		private List<int> numbers = new List<int>();

		public LeftHandGroup _LeftHandGroup;

		public RightHandGroup _RightHandGroup;

		public IStorageHandGroup StorageHand;

		public bool IsClickEnable = true;

		public bool IsStorageHand = true;

		private List<int> hands = new List<int>();

		private List<int> viewedPokers = new List<int>();

		private bool needHasCard;

		public static HandCardSystem Get()
		{
			if (_HandCardSystem == null)
			{
				_HandCardSystem = UnityEngine.Object.FindObjectOfType<HandCardSystem>();
			}
			return _HandCardSystem;
		}

		private void Awake()
		{
			_HandCardSystem = this;
		}

		private void OnDestroy()
		{
			_HandCardSystem = null;
			FindObjectsWithClick.Get().Remove(InsertClick);
		}

		public void OnStart(ScheduleData playSchedule, int handCount, UnityAction unityAction)
		{
			handCount -= WorldHandConfig.Get().GetHand(playSchedule);
			if (handCount < 3)
			{
				UnityEngine.Debug.LogWarningFormat("{0}-{1}-{2},初始手牌数目:{3}", playSchedule.world + 1, playSchedule.chapter + 1, playSchedule.level + 1, handCount);
			}
			handCount = Mathf.Max(handCount, 3);
			List<BaseCard> list = new List<BaseCard>();
			List<CardProbability> list2 = new List<CardProbability>();
			string text = string.Empty;
			if (playSchedule.world == 1)
			{
				text = "Prefabs/StorageHandGroup.prefab";
			}
			if (!string.IsNullOrEmpty(text))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(text));
				gameObject.transform.SetParent(base.transform, worldPositionStays: false);
				StorageHand = gameObject.GetComponent<IStorageHandGroup>();
				gameObject.transform.localPosition = new Vector3(3.8f, -7.1f, 0f);
				gameObject.transform.DOLocalMoveY(-4.1f, 0.5f);
				Transform transform = base.transform;
				Vector3 localPosition = base.transform.localPosition;
				float x = localPosition.x - 2.1f;
				Vector3 localPosition2 = base.transform.localPosition;
				transform.localPosition = new Vector3(x, localPosition2.y, 0f);
			}
			if (!SingletonClass<OnceGameData>.Get().IsTutorial())
			{
				foreach (CardProbability cardProbability in HandConfig.GetNormal().CardProbabilitys)
				{
					if (MathUtility.Probability(cardProbability.probability))
					{
						list2.Add(cardProbability);
					}
				}
			}
			for (int i = 0; i < handCount - list2.Count; i++)
			{
				list.Add(GetBaseCard(new CardConfig
				{
					Index = GetHandRandom()
				}));
			}
			if (PackData.Get().ContainsBooster(BoosterType.BellaBlessing))
			{
				list2.Add(HandConfig.GetBella().Random());
			}
			foreach (CardProbability item in list2)
			{
				BaseCard baseCard = GetBaseCard(new CardConfig
				{
					CardType = item.GetCardType(),
					Index = item.Index
				});
				if (list.Count == 0)
				{
					list.Add(baseCard);
				}
				else
				{
					list.Insert(UnityEngine.Random.Range(1, list.Count), baseCard);
				}
			}
			Sequence sequence = null;
			for (int j = 0; j < list.Count; j++)
			{
				_LeftHandGroup.AppendCard(list[j]);
				Vector3 vector = new Vector3(_LeftHandGroup.GetPositionX(list.Count - j - 1), 0f, 0f);
				list[j].transform.localPosition = vector - new Vector3(0f, 3f, 0f);
				sequence = DOTween.Sequence();
				sequence.PrependInterval((float)j * 0.04f);
				sequence.Append(list[j].transform.DOLocalMove(vector, 0.3f));
			}
			for (int k = 0; k < list.Count; k++)
			{
				list[k].UpdateOrderLayer(k);
			}
			sequence.OnComplete(delegate
			{
				if (unityAction != null)
				{
					unityAction();
				}
				_LeftHandGroup.UpdatePosition();
			});
			FindObjectsWithClick.Get().Append(InsertClick);
		}

		private bool InsertClick(Transform[] transforms)
		{
			if (PlayDesk.Get().IsAnimionBusy)
			{
				return false;
			}
			if (IsClickEnable)
			{
				BaseCard top = _LeftHandGroup.GetTop();
				if ((bool)top)
				{
					foreach (Transform transform in transforms)
					{
						BaseCard component = transform.gameObject.GetComponent<BaseCard>();
						if (component == top)
						{
							DoNextCardForce();
							return true;
						}
					}
				}
			}
			return false;
		}

		private BaseCard GetBaseCard(CardConfig config)
		{
			BaseCard baseCard = null;
			Type stringType = EnumUtility.GetStringType(config.CardType);
			GameObject gameObject = new GameObject(stringType.Name);
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			baseCard = (BaseCard)gameObject.AddComponent(stringType);
			baseCard.OnStart(config);
			return baseCard;
		}

		public void PutHandNumber(List<int> numbers)
		{
			hands.AddRange(numbers);
		}

		public int GetHandRandom()
		{
			if (hands.Count > 0)
			{
				int result = hands[0];
				hands.RemoveAt(0);
				return result;
			}
			return GetRandom();
		}

		public int GetRandom()
		{
			if (numbers.Count == 0)
			{
				for (int i = 1; i <= 52; i++)
				{
					numbers.Add(i);
				}
			}
			int num = numbers[UnityEngine.Random.Range(0, numbers.Count)];
			numbers.Remove(num);
			return num;
		}

		public void CheckRightHandCard(UnityAction unityAction = null)
		{
			BaseCard top = _RightHandGroup.GetTop();
			if (top == null || !top.StayInTop())
			{
				PlayDesk.Get().AppendBusyTime(0.5f);
				this.DelayDo(new WaitForSeconds(0.5f), delegate
				{
					DoNextCardForce();
					if (unityAction != null)
					{
						unityAction();
					}
				});
			}
			else if (unityAction != null)
			{
				unityAction();
			}
		}

		public void FromDeskToRightHandCard(BaseCard baseCard)
		{
			_RightHandGroup.AppendCard(baseCard);
			baseCard.UpdateOrderLayer(1010);
			baseCard.transform.SetParent(_RightHandGroup.transform, worldPositionStays: true);
			Sequence sequence = DOTween.Sequence();
			sequence.Append(baseCard.transform.DORotate(Vector3.forward * 360f * UnityEngine.Random.Range(3, 5), 0.7f, RotateMode.FastBeyond360));
			sequence.Join(baseCard.transform.DOLocalJump(Vector3.zero, 4f, 1, 0.7f));
			sequence.Join(baseCard.transform.DOScale(1f, 0.7f));
			sequence.SetEase(Ease.Linear);
			sequence.SetId($"MoveToHandCard_{baseCard.GetInstanceID()}");
			sequence.OnComplete(delegate
			{
				_RightHandGroup.UpdatePosition();
				if (_RightHandGroup.IsDestory)
				{
					baseCard.transform.DOMoveY(-7f, 0.2f);
				}
			});
			PlayDesk.Get().AppendStepBusyTime(sequence.Duration());
		}

		public void AppendRightCardNormal(BaseCard baseCard)
		{
			_RightHandGroup.AppendCard(baseCard);
			baseCard.UpdateOrderLayer(1010);
			baseCard.transform.SetParent(_RightHandGroup.transform, worldPositionStays: true);
			Sequence sequence = DOTween.Sequence();
			sequence.Append(baseCard.transform.DOLocalMove(Vector3.zero, 0.5f));
			sequence.Join(baseCard.transform.DOScale(1f, 0.5f));
			sequence.SetEase(Ease.Linear);
			sequence.SetId($"MoveToHandCard_{baseCard.GetInstanceID()}");
			sequence.OnComplete(delegate
			{
				PlayDesk.Get().DestopChanged();
				_RightHandGroup.UpdatePosition();
			});
			PlayDesk.Get().AppendBusyTime(sequence.Duration());
		}

		public BaseCard FlyRightCard()
		{
			BaseCard result = _RightHandGroup.FlyCard();
			CheckRightHandCard();
			return result;
		}

		public BaseCard FlyLeftCard()
		{
			BaseCard baseCard = _LeftHandGroup.FlyCard();
			if (baseCard == null)
			{
				return null;
			}
			OperatingHelper.Get().ClearStep();
			PlayScene.Get().SetOverButtons(_LeftHandGroup.GetTop() == null);
			return baseCard;
		}

		public void UpdateNeedHasCard()
		{
			if (AuxiliaryData.Get().HelpHand && AuxiliaryData.Get().BuyHandNumber < 5)
			{
				needHasCard = true;
				AuxiliaryData.Get().BuyHandNumber++;
			}
		}

		public void AppendLeftCard(int index)
		{
			BaseCard baseCard = GetBaseCard(new CardConfig
			{
				Index = index
			});
			_LeftHandGroup.AppendCard(baseCard);
			_LeftHandGroup.UpdatePosition();
			_LeftHandGroup.UpdateMagicEye();
		}

		public void AppendLeftCards(int count, UnityAction unityAction = null)
		{
			BaseCard[] array = new BaseCard[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = GetBaseCard(new CardConfig
				{
					Index = GetRandom()
				});
			}
			float num = 0.2f;
			if (_LeftHandGroup.baseCards.Count > 0)
			{
				num = _LeftHandGroup.baseCards.Min(delegate(BaseCard e)
				{
					Vector3 localPosition = e.transform.localPosition;
					return localPosition.x;
				});
			}
			Sequence sequence = null;
			for (int j = 0; j < array.Length; j++)
			{
				_LeftHandGroup.InsertCard(array[j]);
				Vector3 vector = new Vector3(num - (float)(j + 1) * 0.2f, 0f, 0f);
				array[j].transform.localPosition = vector + new Vector3(0f, -3f, 0f);
				sequence = DOTween.Sequence();
				sequence.PrependInterval((float)j * 0.1f);
				sequence.Append(array[j].transform.DOLocalMove(vector, 0.5f));
				sequence.Join(array[j].transform.DOLocalRotate(Vector3.zero, 0.5f));
			}
			PlayDesk.Get().AppendBusyTime(0.5f + (float)(count - 1) * 0.1f);
			for (int k = 0; k < array.Length; k++)
			{
				array[k].UpdateOrderLayer(count - k);
			}
			sequence.OnComplete(delegate
			{
				if (unityAction != null)
				{
					unityAction();
				}
				_LeftHandGroup.UpdatePosition();
				_LeftHandGroup.UpdateMagicEye();
				PlayDesk.Get().DestopChanged();
			});
			PlayScene.Get().SetOverButtons(visable: false);
		}

		public void FlyRightToLeft(UnityAction unityAction)
		{
			BaseCard baseCard = _RightHandGroup.FlyCard();
			_LeftHandGroup.AppendCard(baseCard);
			baseCard.UpdateOrderLayer(1010);
			baseCard.transform.DOLocalMove(Vector3.zero, 0.5f).OnComplete(delegate
			{
				PlayDesk.Get().DestopChanged();
				_LeftHandGroup.UpdatePosition();
				_LeftHandGroup.UpdateMagicEye();
				if (unityAction != null)
				{
					unityAction();
				}
			});
			PlayDesk.Get().AppendBusyTime(0.5f);
			PlayScene.Get().SetOverButtons(visable: false);
		}

		public void DoNextCardForce(bool thinkMiss = true)
		{
			if (!(PlayDesk.Get() != null) || PlayDesk.Get().IsGameOver)
			{
				return;
			}
			BaseCard baseCard = _LeftHandGroup.FlyCard();
			if (baseCard == null)
			{
				return;
			}
			if (baseCard is NumberCard && !viewedPokers.Contains(baseCard.GetInstanceID()))
			{
				viewedPokers.Add(baseCard.GetInstanceID());
				if (needHasCard && UnityEngine.Random.Range(0, _LeftHandGroup.baseCards.Count) == 0)
				{
					BaseCard baseCard2 = (from e in PlayDesk.Get().Uppers
						where e is NumberCard
						select e).FirstOrDefault();
					if ((bool)baseCard2)
					{
						needHasCard = false;
						baseCard.Config.Index = ((baseCard2.Config.Index <= 10) ? (baseCard2.Config.Index + 1) : (baseCard2.Config.Index - 1));
						baseCard.UpdateFaceWithConfig();
						UnityEngine.Debug.LogWarning("给你加手牌必出有用牌的机会了。");
					}
				}
			}
			AudioUtility.GetSound().Play("Audios/open_Poker.mp3");
			PlayDesk.Get().OnClickCardChanged.Invoke(arg0: false);
			if (_RightHandGroup.GetTop() != null && thinkMiss)
			{
				OperatingHelper.Get().AppendStep(new CardStep());
				BaseCard top = _RightHandGroup.GetTop();
				bool flag = false;
				BaseCard[] array = PlayDesk.Get().Uppers.ToArray();
				BaseCard[] array2 = array;
				foreach (BaseCard baseCard3 in array2)
				{
					if (baseCard3.DoCheckMiss(top))
					{
						flag = true;
					}
				}
				if (top is NumberCard && Totem.DoCheckMiss(top.GetSuit()))
				{
					flag = true;
				}
				if (flag)
				{
					AudioUtility.GetSound().Play("Audios/Miss_Card.mp3");
					PlayScene.Get().HasMissCard = true;
				}
				foreach (BaseCard poker in PlayDesk.Get().Pokers)
				{
					poker.OnHandChange();
				}
				StorageHand.HasVaule(delegate(IStorageHandGroup e)
				{
					e.DONext();
				});
			}
			PlayStreaksSystem.Get().ChangeHand();
			OperatingHelper.Get().ClearLinkCount();
			AppendRightCardNormal(baseCard);
			PlayDesk.Get().DestopChanged();
			if (_LeftHandGroup.GetTop() == null)
			{
				PlayScene.Get().SetOverButtons(visable: true);
				OperatingHelper.Get().ClearStepOnlyOne();
			}
			else
			{
				PlayScene.Get().SetOverButtons(visable: false);
			}
		}
	}
}
