using SolitaireTripeaks;
using System.Collections.Generic;
using UnityEngine;

namespace TripeaksTools
{
	public class TripeaksContagionCard : TripeaksCard
	{
		private float Index = 2f;

		private float contagionRadius = 3f;

		public TripeaksContagionCard(BaseCard card, TripeaksDesk desk = null)
		{
			baseCard = card;
			base.desk = desk;
			if (desk != null)
			{
				string cardInfo = desk.cardInfo;
				desk.cardInfo = cardInfo + "传染牌：" + GetSuit() + GetNumber() + "\n";
			}
			float.TryParse(baseCard.Config.ExtraContent, out contagionRadius);
			float num = contagionRadius / 100f;
			Vector3 lossyScale = baseCard.transform.lossyScale;
			contagionRadius = num * lossyScale.x;
		}

		public override void Destory(int number)
		{
			desk.SpendTime(1f);
			desk.Step += string.Format("销毁传染牌！！！\n", GetSuit(), GetNumber());
			base.Destory(number);
		}

		public override void SpendTime(float deltaTime)
		{
			Index -= deltaTime;
			if (!(Index <= 0f))
			{
				return;
			}
			Index = 14f;
			desk.Step += $"传染牌 {GetSuit()} {GetNumber()} 开始触发！！！\n";
			Collider[] array = Physics.OverlapSphere(base.baseCard.transform.position, contagionRadius);
			List<BaseCard> list = new List<BaseCard>();
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				BaseCard component = collider.gameObject.GetComponent<BaseCard>();
				if (component != null && desk.GetUppers().Contains(component) && component is NumberCard && component.GetExtra<BaseExtra>() == null && component.Config.Index != base.baseCard.Config.Index)
				{
					list.Add(component);
				}
			}
			if (list.Count > 0)
			{
				BaseCard baseCard = list[Random.Range(0, list.Count - 1)];
				baseCard.SetIndex(base.baseCard.Config.Index);
			}
		}
	}
}
