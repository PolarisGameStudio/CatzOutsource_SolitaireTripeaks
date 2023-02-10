using DG.Tweening;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class TipPokerSystem : MonoBehaviour
	{
		private GameObject TipsObject;

		private GameObject HandTipsGameObject;

		private GameObject ArrowGameObject;

		private GameObject TargetGameObject;

		private List<GameObject> TipsObjectsHide = new List<GameObject>();

		private List<GameObject> TipsObjects = new List<GameObject>();

		private static TipPokerSystem tipPoker;

		public bool IsRuning
		{
			get;
			set;
		}

		public static TipPokerSystem Get()
		{
			if (tipPoker == null)
			{
				GameObject gameObject = new GameObject("Tip Poker System");
				gameObject.transform.SetParent(PlayDesk.Get().transform, worldPositionStays: false);
				tipPoker = gameObject.AddComponent<TipPokerSystem>();
			}
			return tipPoker;
		}

		private void Awake()
		{
			tipPoker = this;
			IsRuning = true;
			TipsObject = SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, "Prefabs/TipsObject");
			HandTipsGameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, "Prefabs/HandTipsObject"));
			HandTipsGameObject.transform.SetParent(base.transform, worldPositionStays: false);
			HandTipsGameObject.SetActive(value: false);
			ArrowGameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, "Prefabs/ArrowObject"));
			ArrowGameObject.transform.SetParent(base.transform, worldPositionStays: false);
			ArrowGameObject.SetActive(value: false);
		}

		private void OnDestroy()
		{
			tipPoker = null;
		}

		public Transform[] GetTips()
		{
			BaseCard rightCard = HandCardSystem.Get()._RightHandGroup.GetTop();
			if (rightCard == null)
			{
				return new Transform[0];
			}
			List<Transform> list = (from e in PlayDesk.Get().Uppers
				where e.CalcClickMatch(rightCard)
				select e.transform).ToList();
			if (list.Count == 0)
			{
				BaseCard top = HandCardSystem.Get()._LeftHandGroup.GetTop();
				if (top != null)
				{
					list.Add(top.transform);
				}
			}
			else
			{
				list.Add(rightCard.transform);
			}
			return list.ToArray();
		}

		public bool OnlyHand()
		{
			BaseCard rightCard = HandCardSystem.Get()._RightHandGroup.GetTop();
			if (rightCard == null)
			{
				return false;
			}
			return PlayDesk.Get().Uppers.Count((BaseCard e) => e.CalcClickMatch(rightCard)) == 0;
		}

		public TipType OpenTip()
		{
			TipType result = TipType.None;
			if (!IsRuning)
			{
				return TipType.None;
			}
			try
			{
				HideTip();
				GameObject gameObject = null;
				if (PlayDesk.Get().Uppers.Count == 0)
				{
					return TipType.None;
				}
				BaseCard[] array = (from e in PlayDesk.Get().Uppers
					where e.CalcClickMatch(HandCardSystem.Get()._RightHandGroup.GetTop())
					select e).ToArray();
				BaseCard[] array2 = array;
				foreach (BaseCard baseCard in array2)
				{
					gameObject = CreateTipsObject();
					gameObject.transform.position = baseCard.transform.position;
					gameObject.transform.eulerAngles = baseCard.transform.eulerAngles;
				}
				result = ((array.Length <= 1) ? TipType.One : TipType.Double);
				if (array.Length == 0)
				{
					BaseCard top = HandCardSystem.Get()._LeftHandGroup.GetTop();
					if (top == null)
					{
						return TipType.None;
					}
					gameObject = CreateTipsObject();
					gameObject.transform.SetParent(top.transform);
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localEulerAngles = Vector3.zero;
					result = TipType.Hand;
				}
				HandTipsGameObject.SetActive(value: true);
				HandTipsGameObject.transform.SetParent(HandCardSystem.Get()._RightHandGroup.GetTop().transform, worldPositionStays: false);
				if (array.Length != 0 && array.Length != 1)
				{
					return result;
				}
				ArrowGameObject.SetActive(value: true);
				ArrowGameObject.transform.position = gameObject.transform.position + new Vector3(0f, 1f, 0f);
				DOTween.Kill(ArrowGameObject.GetInstanceID());
				Sequence sequence = DOTween.Sequence();
				Sequence s = sequence;
				Transform transform = ArrowGameObject.transform;
				Vector3 position = ArrowGameObject.transform.position;
				s.Append(transform.DOMoveY(position.y + 0.2f, 0.5f));
				Sequence s2 = sequence;
				Transform transform2 = ArrowGameObject.transform;
				Vector3 position2 = ArrowGameObject.transform.position;
				s2.Append(transform2.DOMoveY(position2.y, 0.5f));
				sequence.SetLoops(-1);
				sequence.SetId(ArrowGameObject.GetInstanceID());
				return result;
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
				return result;
			}
		}

		public void HideTip()
		{
			try
			{
				foreach (GameObject tipsObject in TipsObjects)
				{
					tipsObject.SetActive(value: false);
				}
				TipsObjectsHide.AddRange(TipsObjects);
				TipsObjects.Clear();
				ArrowGameObject.SetActive(value: false);
				HandTipsGameObject.SetActive(value: false);
			}
			catch (Exception)
			{
			}
		}

		public GameObject OpenTargetTip(Transform transform, float scaleY = 1f)
		{
			if (TargetGameObject == null)
			{
				TargetGameObject = new GameObject("HandTipsObject");
				UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, "Prefabs/HandTipsObject"), TargetGameObject.transform);
			}
			TargetGameObject.transform.SetParent(transform, worldPositionStays: false);
			TargetGameObject.transform.localScale = new Vector3(1f, scaleY, 1f);
			TargetGameObject.SetActive(value: true);
			return TargetGameObject;
		}

		public void CloseTargetTip()
		{
			if (TargetGameObject != null)
			{
				UnityEngine.Object.Destroy(TargetGameObject);
				TargetGameObject = null;
			}
		}

		private GameObject CreateTipsObject()
		{
			GameObject gameObject = null;
			if (TipsObjectsHide.Count > 0)
			{
				gameObject = TipsObjectsHide[0];
				TipsObjectsHide.RemoveAt(0);
				gameObject.SetActive(value: true);
			}
			else
			{
				gameObject = UnityEngine.Object.Instantiate(TipsObject);
			}
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			TipsObjects.Add(gameObject);
			return gameObject;
		}
	}
}
