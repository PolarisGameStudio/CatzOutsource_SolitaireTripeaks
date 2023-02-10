using DG.Tweening;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class BaseCard : MonoBehaviour
	{
		private const float PokerWidth = 1.67f;

		private const float PokerHeight = 2.46f;

		public CardConfig Config;

		protected List<BaseExtra> extras = new List<BaseExtra>();

		private bool isAnimtoring;

		private bool opening;

		private BoxCollider boxCollider;

		private bool destorying;

		public FrontPoker FrontPoker
		{
			get;
			private set;
		}

		public BackgroundPoker BackgroundPoker
		{
			get;
			private set;
		}

		public Transform PokerTransform
		{
			get;
			private set;
		}

		public Vector3 GetHalfExtents()
		{
			Vector3 size = boxCollider.size;
			Vector3 lossyScale = base.transform.lossyScale;
			return size * lossyScale.x * 0.5f;
		}

		public void HideAndRemoveAllExtra()
		{
			foreach (BaseExtra extra in extras)
			{
				extra.gameObject.SetActive(value: false);
			}
			extras.Clear();
		}

		public void OnStart(CardConfig config)
		{
			Config = config;
			PokerTransform = new GameObject("Poker Transform").transform;
			PokerTransform.SetParent(base.transform, worldPositionStays: false);
			boxCollider = base.gameObject.AddComponent<BoxCollider>();
			boxCollider.size = new Vector3(1.67f, 2.46f, 0.01f);
			GameObject gameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(GetFont()));
			gameObject.transform.SetParent(PokerTransform, worldPositionStays: false);
			gameObject.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
			FrontPoker = gameObject.GetComponent<FrontPoker>();
			gameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(GetBackground()));
			gameObject.transform.SetParent(PokerTransform, worldPositionStays: false);
			BackgroundPoker = gameObject.GetComponent<BackgroundPoker>();
			StartInitialized();
			Initialized();
		}

		public void ExtraInitialized()
		{
			if (Config.ExtraConfigs != null)
			{
				ExtraConfig[] extraConfigs = Config.ExtraConfigs;
				for (int i = 0; i < extraConfigs.Length; i++)
				{
					ExtraConfig extraConfig = extraConfigs[i];
					Type stringType = EnumUtility.GetStringType(extraConfig.ClassType);
					GameObject gameObject = new GameObject(stringType.Name);
					gameObject.transform.SetParent(FrontPoker.transform, worldPositionStays: false);
					BaseExtra baseExtra = (BaseExtra)gameObject.AddComponent(stringType);
					baseExtra.OnStart(this, extraConfig.Index);
					extras.Add(baseExtra);
				}
			}
			UpdateOrderLayer(Config.zIndex);
			UpdateFaceWithConfig();
		}

		public void Initialized()
		{
			destorying = false;
			base.gameObject.SetActive(value: true);
			SetBoxCollider(enable: true);
			SetSprite(Config.IsOpen, Config.Index);
			UpdateOrderLayer(Config.zIndex);
		}

		public void UpdateOrderLayer(int zIndex)
		{
			FrontPoker.UpdateLayer(zIndex);
			BackgroundPoker.UpdateLayer(zIndex);
			for (int i = 0; i < extras.Count; i++)
			{
				extras[i].UpdateOrderLayer(zIndex, i + 3);
			}
		}

		public void UpdateFaceWithConfig()
		{
			FrontPoker.gameObject.SetActive(Config.IsOpen);
			BackgroundPoker.gameObject.SetActive(!Config.IsOpen);
			PokerTransform.localEulerAngles = new Vector3(0f, Config.IsOpen ? 180 : 0, 0f);
		}

		public void SetMagicEye()
		{
			FrontPoker.gameObject.SetActive(value: true);
			BackgroundPoker.gameObject.SetActive(value: false);
			PokerTransform.localEulerAngles = new Vector3(0f, 180f, 0f);
		}

		public Sequence PlayOpenAnimtor(bool isOpen)
		{
			if (opening == isOpen)
			{
				FrontPoker.gameObject.SetActive(isOpen);
				BackgroundPoker.gameObject.SetActive(!isOpen);
				PokerTransform.localEulerAngles = new Vector3(0f, isOpen ? 180 : 0, 0f);
				return null;
			}
			SetPokerFaceAllShow();
			DOTween.Kill(PokerTransform.GetInstanceID());
			isAnimtoring = true;
			Sequence sequence = DOTween.Sequence();
			sequence.Append(PokerTransform.DOLocalRotate(new Vector3(0f, isOpen ? 180 : 0, 0f), 0.4f));
			sequence.AppendCallback(delegate
			{
				SetSprite(isOpen, Config.Index);
			});
			sequence.OnComplete(delegate
			{
				isAnimtoring = false;
			});
			sequence.SetId(PokerTransform.GetInstanceID());
			return sequence;
		}

		public void SetPokerFaceAllShow()
		{
			FrontPoker.gameObject.SetActive(value: true);
			BackgroundPoker.gameObject.SetActive(value: true);
			foreach (BaseExtra extra in extras)
			{
				extra.SetState(state: true);
			}
		}

		public bool TryOpenCard(bool open = true)
		{
			PlayOpenAnimtor(open);
			if (open)
			{
				FrontPoker.UpdateColor(!PlayDesk.Get().Pokers.Contains(this) || PlayDesk.Get().Uppers.Contains(this));
				foreach (BaseExtra extra in extras)
				{
					extra.UpdateColor(!PlayDesk.Get().Pokers.Contains(this) || PlayDesk.Get().Uppers.Contains(this));
				}
			}
			if (Config.IsOpen != open)
			{
				Config.IsOpen = open;
				return true;
			}
			return false;
		}

		public void SetIndex(int index)
		{
			Config.Index = index;
			SetSprite(Config.IsOpen, index);
		}

		public void OnHandChange()
		{
			BaseExtra[] array = extras.ToArray();
			BaseExtra[] array2 = array;
			foreach (BaseExtra baseExtra in array2)
			{
				if (baseExtra.gameObject.activeSelf)
				{
					baseExtra.OnHandChange();
				}
			}
		}

		public bool DoCheckMiss(BaseCard baseCard)
		{
			bool result = false;
			if (baseCard != null && IsFree() && CalcClickMatch(baseCard))
			{
				MatchedError();
				result = true;
			}
			return result;
		}

		public void Undo(bool match)
		{
			BaseExtra[] array = extras.ToArray();
			BaseExtra[] array2 = array;
			foreach (BaseExtra baseExtra in array2)
			{
				if (baseExtra.gameObject.activeSelf)
				{
					baseExtra.OnUndo(match);
				}
			}
		}

		public void ClearCard()
		{
			foreach (BaseExtra extra in extras)
			{
				extra.OnRemoveCard();
			}
		}

		public void SetBoxCollider(bool enable)
		{
			boxCollider.enabled = enable;
		}

		public T GetExtra<T>() where T : BaseExtra
		{
			foreach (BaseExtra extra in extras)
			{
				if (extra is T)
				{
					return extra as T;
				}
			}
			return (T)null;
		}

		public bool HasExtras(ExtraType extraType)
		{
			return extras.Find((BaseExtra e) => e.GetType() == EnumUtility.GetStringType(extraType));
		}

		public bool IsFree()
		{
			foreach (BaseExtra extra in extras)
			{
				if (!extra.IsFree())
				{
					return false;
				}
			}
			return true;
		}

		public void SetSprite(bool state, int index)
		{
			opening = state;
			FrontPoker.UpdateNumber(index);
			FrontPoker.gameObject.SetActive(state);
			BackgroundPoker.gameObject.SetActive(!state);
			foreach (BaseExtra extra in extras)
			{
				extra.SetState(state);
			}
		}

		public virtual bool IsApplyGloden()
		{
			return true;
		}

		public void RemoveExtra(BaseExtra baseExtra)
		{
			if (extras.Contains(baseExtra))
			{
				extras.Remove(baseExtra);
			}
		}

		public void DestoryByMatch(BaseCard baseCard)
		{
			BaseExtra[] array = extras.ToArray();
			BaseExtra[] array2 = array;
			foreach (BaseExtra baseExtra in array2)
			{
				if (baseExtra.DestoryByMatch(baseCard))
				{
					return;
				}
			}
			DestoryCollect(step: true);
		}

		public void DestoryByGolden()
		{
			BaseExtra[] array = extras.ToArray();
			BaseExtra[] array2 = array;
			foreach (BaseExtra baseExtra in array2)
			{
				if (baseExtra.DestoryByGolden())
				{
					return;
				}
			}
			DestoryCollect(step: false);
		}

		public void DestoryByRocket()
		{
			BaseExtra[] array = extras.ToArray();
			BaseExtra[] array2 = array;
			foreach (BaseExtra baseExtra in array2)
			{
				if (baseExtra.DestoryByRocket())
				{
					return;
				}
			}
			DestoryCollect(step: false);
		}

		public void DestoryByColor()
		{
			BaseExtra[] array = extras.ToArray();
			BaseExtra[] array2 = array;
			foreach (BaseExtra baseExtra in array2)
			{
				if (baseExtra.DestoryByColor())
				{
					return;
				}
			}
			DestoryCollect(step: false);
		}

		public virtual void DestoryByBooster()
		{
		}

		public virtual void DestoryCollect(bool step)
		{
			PlayDesk.Get().RemoveCard(this);
			HandCardSystem.Get().FromDeskToRightHandCard(this);
			PlayDesk.Get().ClearCard();
			PlayDesk.Get().LinkOnce(base.transform.position);
			PlayDesk.Get().DestopChanged();
			PlayDesk.Get().CalcTopCard();
		}

		public bool IsBusy()
		{
			return isAnimtoring;
		}

		public virtual bool CalcClickMatch(BaseCard baseCard)
		{
			return false;
		}

		public void MatchedError()
		{
			if (!isAnimtoring)
			{
				UpdateFaceWithConfig();
				string text = $"PokerTransform_{PokerTransform.GetInstanceID()}";
				DOTween.Kill(text, complete: true);
				float y = opening ? 180 : 0;
				Sequence sequence = DOTween.Sequence();
				sequence.Append(PokerTransform.DOLocalRotate(new Vector3(0f, y, 10f), 0.05f));
				sequence.Append(PokerTransform.DOLocalRotate(new Vector3(0f, y, 350f), 0.1f));
				sequence.Append(PokerTransform.DOLocalRotate(new Vector3(0f, y, 0f), 0.05f));
				sequence.SetLoops(3);
				sequence.SetEase(Ease.Linear);
				sequence.SetId(text);
			}
		}

		public virtual bool StayInTop()
		{
			return true;
		}

		public virtual void CollectedToRightHand()
		{
		}

		protected virtual string GetBackground()
		{
			return "Prefabs/Pokers/BackgroundPoker";
		}

		protected virtual string GetFont()
		{
			return "Prefabs/Pokers/NumberPoker";
		}

		public virtual int GetColor()
		{
			return -1;
		}

		public virtual int GetNumber()
		{
			return -1;
		}

		public virtual int GetSuit()
		{
			return -1;
		}

		protected virtual void StartInitialized()
		{
		}
	}
}
