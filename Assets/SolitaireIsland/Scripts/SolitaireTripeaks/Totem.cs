using DG.Tweening;
using Nightingale.Localization;
using Nightingale.Utilitys;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class Totem : BaseAdditional
	{
		public int ColorType;

		public SpriteRenderer Background;

		public SpriteRenderer Front;

		public bool IsCompleted
		{
			get;
			private set;
		}

		private void Awake()
		{
			Front.gameObject.SetActive(value: false);
		}

		public override void UpdateLayer(int index)
		{
			Background.sortingOrder = 500 + index;
			Front.sortingOrder = 505 + index;
		}

		public override bool OnClick()
		{
			if (IsCompleted)
			{
				return false;
			}
			BaseCard flyCard = HandCardSystem.Get()._RightHandGroup.GetTop();
			if (flyCard == null || !(flyCard is NumberCard) || flyCard.GetSuit() != ColorType)
			{
				Shanke();
				return false;
			}
			IsCompleted = true;
			AudioUtility.GetSound().Play("Audios/Rope.mp3");
			flyCard = HandCardSystem.Get().FlyRightCard();
			flyCard.UpdateOrderLayer(32667);
			DOTween.Kill($"MoveToHandCard_{flyCard.GetInstanceID()}");
			flyCard.transform.parent = PlayDesk.Get().transform;
			PlayDesk.Get().DestopChanged();
			Front.transform.localScale = Vector3.zero;
			Front.gameObject.SetActive(value: true);
			Sequence sequence = DOTween.Sequence();
			sequence.Append(flyCard.transform.DOMove(base.transform.position, 0.5f));
			sequence.Join(flyCard.transform.DORotateX(80f, 0.2f));
			sequence.Join(flyCard.transform.DORotateZ(1800f, 1f));
			sequence.AppendCallback(delegate
			{
				AudioUtility.GetSound().Play("Audios/totem.mp3");
			});
			sequence.Append(flyCard.transform.DOScale(0f, 0.2f));
			sequence.Join(flyCard.transform.DORotate(Vector3.zero, 0.2f));
			sequence.Join(Front.transform.DOScale(1f, 0.2f));
			sequence.OnComplete(delegate
			{
				UnityEngine.Object.Destroy(flyCard.gameObject);
				PlayDesk.Get().OnCardChanged.Invoke();
			});
			OperatingHelper.Get().ClearStep();
			return true;
		}

		public void Shanke()
		{
			Transform child = base.transform.GetChild(0);
			child.localEulerAngles = Vector3.zero;
			string text = $"Totem_{child.GetInstanceID()}";
			DOTween.Kill(text, complete: true);
			Sequence sequence = DOTween.Sequence();
			sequence.Append(child.DOLocalRotate(new Vector3(0f, 0f, 10f), 0.05f));
			sequence.Append(child.DOLocalRotate(new Vector3(0f, 0f, 350f), 0.1f));
			sequence.Append(child.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.05f));
			sequence.SetLoops(3);
			sequence.SetEase(Ease.Linear);
			sequence.SetId(text);
			AudioUtility.GetSound().Play("Audios/Click_Error.mp3");
		}

		public static void CompletedAnitamion(UnityAction unityAction)
		{
			if (unityAction != null)
			{
				List<Totem> arrays = Object.FindObjectsOfType<Totem>().ToList();
				if (arrays.Count == 0)
				{
					unityAction();
				}
				else if (arrays.Find((Totem e) => !e.IsCompleted) != null)
				{
					PlayFailed(unityAction);
				}
				else
				{
					SingletonBehaviour<Effect2DUtility>.Get().CreateTextTipsUI(LocalizationUtility.Get().GetString("TreasureUnlocked"), 0f, delegate
					{
						SessionData.Get().PutCommodity(BoosterType.Coins, CommoditySource.Free, 1000L);
						SingletonClass<OnceGameData>.Get().CompletionCoins += 1000;
						Sequence sequence = DOTween.Sequence();
						for (int num = arrays.Count - 1; num >= 0; num--)
						{
							sequence.Append(arrays[num].gameObject.transform.DOScale(0f, 0.3f));
						}
						sequence.OnComplete(delegate
						{
							SingletonBehaviour<Effect2DUtility>.Get().CreateTitleIconLabelUI(Vector3.zero, LocalizationUtility.Get().GetString("TOTEMCOINS"), 1000.ToString(), unityAction).transform.localScale = new Vector3(2f, 2f, 2f);
							AudioUtility.GetSound().Play("Audios/GetCoins.mp3");
						});
					});
				}
			}
		}

		public static void PlayFailed(UnityAction unityAction)
		{
			Totem[] array = Object.FindObjectsOfType<Totem>();
			if (array.Length == 0)
			{
				if (unityAction != null)
				{
					unityAction();
				}
				return;
			}
			Sequence sequence = null;
			for (int i = 0; i < array.Length; i++)
			{
				Vector3 position = array[i].transform.position;
				position.y += 11.3f;
				array[i].transform.DORotate(Random.insideUnitSphere * 360f * 10f, 10f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
				sequence = DOTween.Sequence();
				sequence.PrependInterval((float)i * 0.02f);
				sequence.Append(array[i].transform.DOMove(position, 2f));
				sequence.SetEase(Ease.Linear);
			}
			sequence = DOTween.Sequence();
			sequence.PrependInterval(1f);
			sequence.OnComplete(delegate
			{
				if (unityAction != null)
				{
					unityAction();
				}
			});
		}

		public static void Create(List<ObjectConfig> configs)
		{
			if (configs != null)
			{
				PlayDesk playDesk = Object.FindObjectOfType<PlayDesk>();
				if (!(playDesk == null) && !playDesk.IsGameOver)
				{
					Sequence s = DOTween.Sequence();
					foreach (ObjectConfig config in configs)
					{
						if (!string.IsNullOrEmpty(config.Type) && config.Type.StartsWith("totem_"))
						{
							Totem component = Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, $"Prefabs/Additionals/{config.Type}")).GetComponent<Totem>();
							PlayAdditional.Get().Append(component);
							component.transform.localPosition = config.Position;
							component.transform.localEulerAngles = new Vector3(0f, 0f, config.EulerAngles);
							component.transform.localScale = Vector3.zero;
							s.Append(component.transform.DOScale(1f, 0.2f));
						}
					}
				}
			}
		}

		public static bool DoCheckMiss(int colorType)
		{
			Totem[] array = (from e in Object.FindObjectsOfType<Totem>()
				where e.ColorType == colorType && !e.IsCompleted
				select e).ToArray();
			Totem[] array2 = array;
			foreach (Totem totem in array2)
			{
				totem.Shanke();
			}
			return array.Length > 0;
		}
	}
}
