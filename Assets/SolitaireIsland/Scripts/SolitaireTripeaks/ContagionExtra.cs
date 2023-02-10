using System.Collections.Generic;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class ContagionExtra : TimeExtra
	{
		private float contagionRadius = 3f;

		protected override void StartInitialized()
		{
			base.StartInitialized();
			base.Index = 14;
			float.TryParse(baseCard.Config.ExtraContent, out contagionRadius);
			float num = contagionRadius / 100f;
			Vector3 lossyScale = base.transform.lossyScale;
			contagionRadius = num * lossyScale.x;
			RestTime(2);
		}

		protected override void LifeUpdate()
		{
			base.LifeUpdate();
		}

		protected override void LifeOver()
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position, contagionRadius);
			List<BaseCard> list = new List<BaseCard>();
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				BaseCard component = collider.gameObject.GetComponent<BaseCard>();
				if (component != null && PlayDesk.Get().Uppers.Contains(component) && component is NumberCard && component.GetExtra<BaseExtra>() == null && component.Config.Index != baseCard.Config.Index)
				{
					list.Add(component);
				}
			}
			if (list.Count > 0)
			{
				BaseCard poker = list[Random.Range(0, list.Count - 1)];
				base.PokerSpine.PlayTransform(poker.transform, delegate
				{
					if (!PlayDesk.Get().IsGameOver)
					{
						poker.Config.Index = baseCard.Config.Index;
						poker.SetIndex(poker.Config.Index);
					}
				});
				OperatingHelper.Get().ClearStep();
				PlayDesk.Get().AppendBusyTime(1.2f);
			}
			RestTime();
		}

		protected override string PokerPrefab()
		{
			return "Prefabs/Extras/ContagionSpine";
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(base.transform.position, contagionRadius);
		}
	}
}
