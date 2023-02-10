using Nightingale.Utilitys;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class BaseExtra : MonoBehaviour
	{
		protected BaseCard baseCard;

		public int Index
		{
			get;
			protected set;
		}

		protected List<SpriteRenderer> spriteRenderers
		{
			get;
			private set;
		}

		protected BaseSpine PokerSpine
		{
			get;
			private set;
		}

		public virtual bool DestoryByMatch(BaseCard card)
		{
			return DestoryByRocket();
		}

		public virtual bool DestoryByGolden()
		{
			return DestoryByRocket();
		}

		public virtual bool DestoryByRocket()
		{
			OperatingHelper.Get().ClearStep();
			baseCard.RemoveExtra(this);
			RemoveAnimtor(delegate
			{
				UnityEngine.Object.Destroy(base.gameObject);
			});
			baseCard.DestoryCollect(step: false);
			return true;
		}

		public virtual bool DestoryByColor()
		{
			return DestoryByRocket();
		}

		public virtual void DestoryByBooster()
		{
			baseCard.RemoveExtra(this);
			RemoveAnimtor(delegate
			{
				UnityEngine.Object.Destroy(base.gameObject);
				PlayDesk.Get().CalcTopCard();
				PlayDesk.Get().DestopChanged();
			});
			OperatingHelper.Get().ClearStep();
		}

		protected virtual string PokerPrefab()
		{
			return string.Empty;
		}

		public void OnStart(BaseCard baseCard, int index)
		{
			spriteRenderers = new List<SpriteRenderer>();
			this.baseCard = baseCard;
			Index = index;
			string text = PokerPrefab();
			if (!string.IsNullOrEmpty(text))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, text));
				PokerSpine = gameObject.GetComponent<BaseSpine>();
				PokerSpine.transform.SetParent(base.transform, worldPositionStays: false);
			}
			StartInitialized();
		}

		protected virtual void StartInitialized()
		{
		}

		public virtual bool IsFree()
		{
			return true;
		}

		public virtual void UpdateOrderLayer(int zIndex, int index)
		{
			int num = 1;
			foreach (SpriteRenderer spriteRenderer in spriteRenderers)
			{
				num++;
				spriteRenderer.sortingOrder = zIndex + index + num;
			}
			if (PokerSpine != null)
			{
				PokerSpine.UpdateOrderLayer(zIndex, index);
			}
		}

		public virtual void OnHandChange()
		{
		}

		public virtual void OnUndo(bool match)
		{
		}

		public virtual void OnRemoveCard()
		{
		}

		public virtual void SetState(bool state)
		{
			base.gameObject.SetActive(state);
		}

		public virtual void RemoveAnimtor(UnityAction unityAction)
		{
			if (PokerSpine != null)
			{
				PokerSpine.PlayDestroy(unityAction);
			}
			else
			{
				unityAction?.Invoke();
			}
		}

		protected SpriteRenderer CreateSpriteRenderer(string name, Transform parent = null)
		{
			if (parent == null)
			{
				parent = base.transform;
			}
			GameObject gameObject = new GameObject(name);
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.material = SingletonBehaviour<LoaderUtility>.Get().GetAsset<Material>("Materials/PokerMaterial");
			spriteRenderers.Add(spriteRenderer);
			return spriteRenderer;
		}

		public virtual void UpdateColor(bool white)
		{
			if (spriteRenderers != null)
			{
				foreach (SpriteRenderer spriteRenderer in spriteRenderers)
				{
					spriteRenderer.color = ((!white) ? Color.gray : Color.white);
				}
			}
			if (PokerSpine != null)
			{
				PokerSpine.UpdateColor(white);
			}
		}
	}
}
