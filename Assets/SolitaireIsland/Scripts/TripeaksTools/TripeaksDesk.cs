using Nightingale.Utilitys;
using SolitaireTripeaks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TripeaksTools
{
	public class TripeaksDesk
	{
		public class TripeaksNode
		{
			public TripeaksCard tripeaksCard;

			public int MaxDepth;

			public List<TripeaksCard> tripeaksCards;

			public List<TripeaksNode> tripeaksNodes;

			public TripeaksNode tripeaksNode;

			public TripeaksNode(TripeaksNode tripeaksNode, TripeaksCard tripeaksCard, List<TripeaksCard> tripeaksCards)
			{
				this.tripeaksCard = tripeaksCard;
				this.tripeaksNode = tripeaksNode;
				tripeaksCards = tripeaksCards.ToList();
				tripeaksCards.Remove(tripeaksCard);
				this.tripeaksCards = tripeaksCards;
				tripeaksNodes = new List<TripeaksNode>();
				List<TripeaksCard> list = (from e in tripeaksCards
					where IsMatch(e.baseCard.GetNumber() + 1, tripeaksCard.baseCard.GetNumber() + 1)
					select e).ToList();
				foreach (TripeaksCard item in list)
				{
					tripeaksNodes.Add(new TripeaksNode(this, item, tripeaksCards));
				}
				if (list.Count == 0)
				{
					int num = 0;
					for (TripeaksNode tripeaksNode2 = tripeaksNode; tripeaksNode2 != null; tripeaksNode2 = tripeaksNode2.tripeaksNode)
					{
						tripeaksNode2.SetMaxDepth(++num);
					}
				}
			}

			public void SetMaxDepth(int depth)
			{
				if (MaxDepth < depth)
				{
					MaxDepth = depth;
				}
			}
		}

		private List<TripeaksCard> cards = new List<TripeaksCard>();

		private List<int> leftHands = new List<int>();

		private List<int> rightHands = new List<int>();

		private int stageCard;

		private List<TripeaksCard> uppers = new List<TripeaksCard>();

		private List<TotemCard> totemCards = new List<TotemCard>();

		private float PlayTime;

		private int linkCoins;

		private int linkCount;

		private int starNumber;

		private int inStarLink;

		private bool over;

		public string cardInfo;

		public string Step = "\n";

		private int world;

		private LevelRetrunCoinConfig returnConfig;

		private List<int> numbers = new List<int>();

		public TripeaksDesk(int world, int handNumber, LevelConfig levelConfig, LevelRetrunCoinConfig returnConfig, Transform transform)
		{
			this.returnConfig = returnConfig;
			this.world = world;
			cardInfo = "本次手牌信息:\n";
			List<BaseCard> list = new List<BaseCard>();
			levelConfig.Cards = (from e in levelConfig.Cards
				orderby e.zIndex
				select e).ToList();
			foreach (CardConfig card in levelConfig.Cards)
			{
				BaseCard baseCard = GetBaseCard(card);
				baseCard.transform.SetParent(transform, worldPositionStays: false);
				baseCard.transform.localPosition = baseCard.Config.GetPosition();
				baseCard.transform.eulerAngles = Vector3.forward * baseCard.Config.EulerAngles;
				list.Add(baseCard);
			}
			cards = ToCards(list, this);
			if (levelConfig.Objects != null)
			{
				foreach (ObjectConfig @object in levelConfig.Objects)
				{
					switch (@object.Type)
					{
					case "totem_spade":
						totemCards.Add(new TotemCard(0));
						break;
					case "totem_heart":
						totemCards.Add(new TotemCard(1));
						break;
					case "totem_club":
						totemCards.Add(new TotemCard(2));
						break;
					case "totem_diamond":
						totemCards.Add(new TotemCard(3));
						break;
					}
				}
			}
			cardInfo += "手牌信息：";
			for (int i = 0; i < levelConfig.HandCount - handNumber; i++)
			{
				int random = GetRandom();
				leftHands.Add(random);
				cardInfo += $"{GetSuit((random - 1) / 13)}_{(random - 1) % 13 + 1},";
			}
			DONext();
			CalcTopCard();
		}

		public List<BaseCard> GetUppers()
		{
			return (from e in uppers
				select e.baseCard).ToList();
		}

		public void RemoveLeftHandCard()
		{
			if (leftHands.Count > 0)
			{
				leftHands.RemoveAt(leftHands.Count - 1);
			}
		}

		public void AppendToRightHand(TripeaksCard card)
		{
			rightHands.Add(card.baseCard.Config.Index);
			RemoveCard(card);
		}

		public void AddHand(int total)
		{
			for (int i = 0; i < total; i++)
			{
				int random = GetRandom();
				leftHands.Add(random);
				cardInfo += $"{GetSuit((random - 1) / 13)}_{(random - 1) % 13 + 1},";
			}
		}

		public void FlyCard()
		{
			if (rightHands.Count > 0)
			{
				rightHands.RemoveAt(rightHands.Count - 1);
			}
			if (rightHands.Count == 0)
			{
				DONext();
			}
		}

		public bool DONext()
		{
			if (leftHands.Count > 0)
			{
				if (rightHands.Count > 0)
				{
					if (world == 2)
					{
						int num = stageCard;
						stageCard = rightHands[rightHands.Count - 1];
						if (num > 0)
						{
							rightHands.Add(num);
						}
						Step += "存手牌\n";
					}
					else if (world == 3 && stageCard == 0)
					{
						stageCard = rightHands[rightHands.Count - 1];
						rightHands.RemoveAt(rightHands.Count - 1);
						Step += "存手牌\n";
					}
				}
				if (world == 3 && stageCard > 0)
				{
					int num2 = (stageCard - 1) / 13 % 4;
					int num3 = (stageCard - 1) % 13;
					num3 = (num3 + 13 + 1) % 13 + 1;
					stageCard = num2 * 13 + num3;
					Step += "手牌数字+1\n";
				}
				linkCount = 0;
				inStarLink = 0;
				int num4 = leftHands[leftHands.Count - 1];
				leftHands.RemoveAt(leftHands.Count - 1);
				rightHands.Add(num4);
				foreach (TripeaksCard card in cards)
				{
					card.DONext(uppers.Contains(card));
				}
				Step += $"翻手牌：{GetSuit((num4 - 1) / 13)}_{(num4 - 1) % 13 + 1}\n";
				SpendTime(1f);
				return true;
			}
			return false;
		}

		public void CalcTopCard()
		{
			uppers.Clear();
			foreach (TripeaksCard card in cards)
			{
				if (IsTop(card))
				{
					uppers.Add(card);
				}
			}
			uppers = (from e in uppers
				orderby Guid.NewGuid()
				select e).ToList();
		}

		public void RemoveCard(TripeaksCard card)
		{
			if (cards.Contains(card))
			{
				cards.Remove(card);
			}
			if (uppers.Contains(card))
			{
				uppers.Remove(card);
			}
			UnityEngine.Object.Destroy(card.baseCard.gameObject);
			foreach (TripeaksCard card2 in cards)
			{
				card2.LinkOnce();
			}
			foreach (TripeaksCard upper in uppers)
			{
				upper.RemoveOnce();
			}
		}

		public bool FindCard()
		{
			if (over)
			{
				return false;
			}
			if (cards.Count == 0)
			{
				return false;
			}
			CalcTopCard();
			List<TripeaksCard> list = new List<TripeaksCard>();
			if (uppers.Count > 0)
			{
				TripeaksCard tripeaksCard = uppers.Find((TripeaksCard e) => e.baseCard is SeagullCard);
				if (tripeaksCard != null)
				{
					tripeaksCard.Destory(0);
					return true;
				}
			}
			if (rightHands.Count > 0)
			{
				int handNumber = (rightHands[rightHands.Count - 1] - 1) % 13 + 1;
				list = (from e in uppers
					where e.IsFree()
					where IsMatch(e.baseCard.GetNumber() + 1, handNumber)
					select e).ToList();
				if (list.Count == 0)
				{
					if (stageCard > 0)
					{
						handNumber = (stageCard - 1) % 13 + 1;
						list = (from e in uppers
							where e.IsFree()
							where IsMatch(e.baseCard.GetNumber() + 1, handNumber)
							select e).ToList();
						if (list.Count > 0)
						{
							if (world == 2)
							{
								rightHands.Add(stageCard);
								stageCard = 0;
								Step += "取手牌\n";
							}
							else if (world == 3)
							{
								int num = rightHands[rightHands.Count - 1];
								rightHands.RemoveAt(rightHands.Count - 1);
								stageCard = num;
								Step += "取手牌\n";
							}
						}
					}
					else if ((world == 2 || world == 3) && rightHands.Count > 1)
					{
						handNumber = rightHands[rightHands.Count - 2] % 13 + 1;
						list = (from e in uppers
							where e.IsFree()
							where IsMatch(e.baseCard.GetNumber() + 1, handNumber)
							select e).ToList();
						if (list.Count > 0)
						{
							stageCard = rightHands[rightHands.Count - 1];
							rightHands.RemoveAt(rightHands.Count - 1);
							Step += "存手牌\n";
						}
					}
				}
				if (list.Count > 0)
				{
					TripeaksCard tripeaksCard2 = list.Find((TripeaksCard e) => e.baseCard is ScarecrowCard);
					if (tripeaksCard2 != null)
					{
						for (int i = 0; i < cards.Count; i++)
						{
							LinkCard();
							UnityEngine.Object.Destroy(cards[i].baseCard.gameObject);
						}
						cards.Clear();
						return false;
					}
				}
			}
			TripeaksCard tripeaksCard3 = uppers.Find((TripeaksCard e) => e.baseCard is ForkCard);
			TripeaksCard tripeaksCard4 = uppers.Find((TripeaksCard e) => e.baseCard is SnakeCard);
			if (tripeaksCard3 != null && tripeaksCard4 != null)
			{
				tripeaksCard3.Destory(0);
				cards.Remove(tripeaksCard4);
				UnityEngine.Object.Destroy(tripeaksCard4.baseCard.gameObject);
				return true;
			}
			if (list.Count == 0)
			{
				if (rightHands.Count > 0)
				{
					foreach (TotemCard totemCard in totemCards)
					{
						if (totemCard.Click(this, rightHands[rightHands.Count - 1]))
						{
							return true;
						}
					}
				}
				return DONext();
			}
			TripeaksCard bestLink = GetBestLink(list, uppers);
			bestLink.Destory(rightHands[rightHands.Count - 1]);
			return true;
		}

		public void LinkCard()
		{
			linkCoins += GetNumber(linkCount);
			linkCount++;
			inStarLink++;
			if (inStarLink >= 5)
			{
				starNumber++;
				inStarLink = 0;
			}
		}

		public void AddLinkCoins(int coins)
		{
			linkCoins += coins;
		}

		public void Over()
		{
			over = true;
		}

		public void SpendTime(float time)
		{
			PlayTime += time;
			foreach (TripeaksCard upper in uppers)
			{
				upper.SpendTime(time);
			}
		}

		public TripeaksOnceReslut GetTripeaksOnceReslut()
		{
			TripeaksOnceReslut tripeaksOnceReslut = new TripeaksOnceReslut();
			tripeaksOnceReslut.Step = Step;
			TripeaksOnceReslut tripeaksOnceReslut2 = tripeaksOnceReslut;
			int num = 0;
			tripeaksOnceReslut2.StarNumber = Mathf.Min(starNumber, 3);
			for (int i = 0; i < returnConfig.StreaksNodeRewardCoin.Count; i++)
			{
				if (starNumber > i)
				{
					num += returnConfig.StreaksNodeRewardCoin[i];
				}
			}
			if (cards.Count == 0)
			{
				int a = Mathf.FloorToInt(((float)returnConfig.LimitTime - PlayTime) / (float)returnConfig.LimitTime * (float)returnConfig.TimeRewardReferenceCoin);
				a = Mathf.Max(Mathf.Min(a, returnConfig.TimeRewardMaxLimitCoin), 0);
				int num2 = 0;
				for (int j = 0; j < leftHands.Count; j++)
				{
					num2 += GetNumber(j);
				}
				tripeaksOnceReslut2.Success = true;
				tripeaksOnceReslut2.TotamCoins = ((totemCards.Count > 0 && totemCards.Find((TotemCard e) => !e.IsCompleted) == null) ? 1000 : 0);
				tripeaksOnceReslut2.WonCoins += linkCoins + returnConfig.CompletionCoin + a + num + num2 - returnConfig.LevelTicketCoins + tripeaksOnceReslut2.TotamCoins;
				tripeaksOnceReslut2.WonPokerCoins += linkCoins;
				tripeaksOnceReslut2.WonSteaksCoins += num;
				tripeaksOnceReslut2.RemainHandCoins += num2;
				tripeaksOnceReslut2.TimeCoins = a;
			}
			else
			{
				tripeaksOnceReslut2.WonCoins += linkCoins + num - returnConfig.LevelTicketCoins;
				tripeaksOnceReslut2.WonPokerCoins += linkCoins;
				tripeaksOnceReslut2.WonSteaksCoins += num;
			}
			tripeaksOnceReslut2.CardInfo = cardInfo;
			foreach (TripeaksCard card in cards)
			{
				UnityEngine.Object.Destroy(card.baseCard.gameObject);
			}
			return tripeaksOnceReslut2;
		}

		public string GetSuit(int index)
		{
			switch (index)
			{
			default:
				return "黑桃";
			case 1:
				return "红桃";
			case 2:
				return "梅花";
			case 3:
				return "方片";
			}
		}

		private int GetNumber(int link)
		{
			int num = 0;
			if (link < returnConfig.StreaksRewardCoin.Count)
			{
				return returnConfig.StreaksRewardCoin[link];
			}
			int num2 = returnConfig.StreaksFormulaParameters[0];
			int num3 = returnConfig.StreaksFormulaParameters[1];
			int num4 = returnConfig.StreaksFormulaParameters[2];
			int num5 = returnConfig.StreaksFormulaParameters[3];
			return GetNumber(link - 1) + (link - num2 + 1) / 2 * num3 + (link - 1) * num4 + num5;
		}

		private bool IsTop(TripeaksCard tripeaksCard)
		{
			Collider[] array = Physics.OverlapBox(tripeaksCard.baseCard.transform.position, tripeaksCard.baseCard.GetHalfExtents(), tripeaksCard.baseCard.transform.rotation);
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				BaseCard finder = collider.gameObject.GetComponent<BaseCard>();
				if (finder != null && collider.gameObject.activeSelf && finder != tripeaksCard.baseCard && cards.Find((TripeaksCard e) => e.baseCard == finder) != null && finder.Config.zIndex > tripeaksCard.baseCard.Config.zIndex)
				{
					return false;
				}
			}
			return true;
		}

		private BaseCard GetBaseCard(CardConfig config)
		{
			if (config.CardType == CardType.Number || config.CardType == CardType.Swallowed)
			{
				config.Index = GetRandom();
			}
			else
			{
				config.Index = UnityEngine.Random.Range(0, 53);
			}
			BaseCard baseCard = null;
			Type stringType = EnumUtility.GetStringType(config.CardType);
			GameObject gameObject = new GameObject(stringType.Name);
			baseCard = (BaseCard)gameObject.AddComponent(stringType);
			baseCard.OnStart(config);
			baseCard.ExtraInitialized();
			return baseCard;
		}

		private int GetRandom()
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

		public TripeaksCard[] GetCard<T>() where T : TripeaksCard
		{
			return (from e in uppers
				where e is T
				select e).ToArray();
		}

		public TripeaksCard[] GetAllCard<T>() where T : TripeaksCard
		{
			return (from e in cards
				where e is T
				select e).ToArray();
		}

		private static bool IsMatch(int number1, int number2)
		{
			if (Mathf.Abs(number1 - number2) == 1)
			{
				return true;
			}
			if (number1 == 1 && number2 == 13)
			{
				return true;
			}
			if (number1 == 13 && number2 == 1)
			{
				return true;
			}
			return false;
		}

		public static List<TripeaksCard> ToCards(List<BaseCard> tripeaks, TripeaksDesk desk = null)
		{
			List<TripeaksCard> list = new List<TripeaksCard>();
			foreach (BaseCard tripeak in tripeaks)
			{
				if (tripeak.GetExtra<RopeExtra>() != null)
				{
					list.Add(new TripeaksRopeCard(tripeak, desk));
				}
				else if (tripeak.GetExtra<SkeletonExtra>() != null)
				{
					list.Add(new TripeaksSkeletonCard(tripeak, desk));
				}
				else if (tripeak.GetExtra<BombExtra>() != null)
				{
					list.Add(new TripeaksBombCard(tripeak, desk));
				}
				else if (tripeak.GetExtra<NumberGrowExtra>() != null)
				{
					list.Add(new TripeaksNumberGrowCard(tripeak, desk));
				}
				else if (tripeak is ForkCard || tripeak is SnakeCard)
				{
					list.Add(new TripeaksForkCard(tripeak, desk));
				}
				else if (tripeak is ScarecrowCard)
				{
					list.Add(new TripeaksScarecrowCard(tripeak, desk));
				}
				else if (tripeak.GetExtra<SwallowedExtra>() != null)
				{
					list.Add(new TripeaksMouseCard(tripeak, desk));
				}
				else if (tripeak.GetExtra<VineExtra>() != null)
				{
					list.Add(new TripeaksVineCard(tripeak, desk));
				}
				else if (tripeak.GetExtra<BatterExtra>() != null)
				{
					list.Add(new TripeaksBatterCard(tripeak, desk));
				}
				else if (tripeak.GetExtra<KeyExtra>() != null)
				{
					list.Add(new TripeaksKeyCard(tripeak, desk));
				}
				else if (tripeak.GetExtra<LockExtra>() != null)
				{
					list.Add(new TripeaksLockCard(tripeak, desk));
				}
				else if (tripeak.GetExtra<ContagionExtra>() != null)
				{
					list.Add(new TripeaksContagionCard(tripeak, desk));
				}
				else if (tripeak.GetExtra<ColorExtra>() != null)
				{
					list.Add(new TripeaksSameColorCard(tripeak, desk));
				}
				else if (tripeak is SeagullCard)
				{
					list.Add(new TripeaksSeagullCard(tripeak, desk));
				}
				else
				{
					list.Add(new TripeaksCard(tripeak, desk));
				}
			}
			return list;
		}

		public static TripeaksCard GetBestLink(List<TripeaksCard> cards, List<TripeaksCard> uppers)
		{
			TripeaksCard tripeaksCard = cards.Find((TripeaksCard e) => e is TripeaksScarecrowCard);
			if (tripeaksCard != null)
			{
				return tripeaksCard;
			}
			tripeaksCard = cards.Find((TripeaksCard e) => e is TripeaksBatterCard);
			if (tripeaksCard != null)
			{
				return tripeaksCard;
			}
			tripeaksCard = cards.Find((TripeaksCard e) => e is TripeaksContagionCard);
			if (tripeaksCard != null)
			{
				return tripeaksCard;
			}
			tripeaksCard = cards.Find((TripeaksCard e) => e is TripeaksNumberGrowCard);
			if (tripeaksCard != null)
			{
				return tripeaksCard;
			}
			tripeaksCard = cards.Find((TripeaksCard e) => e is TripeaksSkeletonCard);
			if (tripeaksCard != null)
			{
				return tripeaksCard;
			}
			List<TripeaksNode> list = new List<TripeaksNode>();
			foreach (TripeaksCard card in cards)
			{
				list.Add(new TripeaksNode(null, card, uppers));
			}
			int max = list.Max((TripeaksNode e) => e.MaxDepth);
			list.RemoveAll((TripeaksNode e) => e.MaxDepth != max);
			list = (from e in list
				orderby FindBack(e.tripeaksCard) descending
				select e).ToList();
			return list[0].tripeaksCard;
		}

		private static int FindBack(TripeaksCard tripeaksCard)
		{
			Collider[] source = Physics.OverlapBox(tripeaksCard.baseCard.transform.position, tripeaksCard.baseCard.GetHalfExtents(), tripeaksCard.baseCard.transform.rotation);
			return source.Count();
		}
	}
}
