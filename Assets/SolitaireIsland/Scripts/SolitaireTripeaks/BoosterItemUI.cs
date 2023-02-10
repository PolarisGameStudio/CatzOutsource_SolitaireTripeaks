using DG.Tweening;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class BoosterItemUI : MonoBehaviour
	{
		public Transform SelectTransform;

		public Transform NormalTransform;

		public Transform NumberTransform;

		public Transform ParticleTransform;

		public Image IconImage;

		public Text NumberLabel;

		private UnityAction<BoosterType> unityAction;

		private bool isSelect;

		public BoosterType boosterType;

		private float scale;

		private bool isBreatheing;

		public void OnStart(BoosterType boosterType, UnityAction<BoosterType> unityAction = null)
		{
			this.unityAction = unityAction;
			this.boosterType = boosterType;
			UpdateNumber(CommoditySource.None);
			PackData.Get().GetCommodity(boosterType).OnChanged.AddListener(UpdateNumber);
			IconImage.sprite = AppearNodeConfig.Get().GetBoosterSprite(boosterType);
			IconImage.SetNativeSize();
			if (PackData.Get().GetCommodity(boosterType).GetTotal() == 0)
			{
				NumberTransform.gameObject.SetActive(value: false);
			}
			SelectTransform.gameObject.SetActive(value: false);
			NormalTransform.gameObject.SetActive(value: true);
			if (PackData.Get().ContainsBooster(boosterType))
			{
				isSelect = true;
				SelectTransform.gameObject.SetActive(value: true);
				NormalTransform.gameObject.SetActive(value: false);
			}
			if (PlayScene.Get() != null)
			{
				ParticleTransform.gameObject.SetActive(value: false);
				base.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);
			}
			else
			{
				base.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
			}
			Vector3 localScale = base.transform.localScale;
			scale = localScale.x;
		}

		public void Breathe(bool isBreathe)
		{
			if (!isSelect && isBreatheing != isBreathe)
			{
				DOTween.Kill(GetInstanceID());
				isBreatheing = isBreathe;
				base.transform.localScale = new Vector3(scale, scale, scale);
				if (isBreathe)
				{
					Sequence sequence = DOTween.Sequence();
					sequence.Append(base.transform.DOScale(1.1f * scale, 0.3f));
					sequence.Append(base.transform.DOScale(1f * scale, 0.3f));
					sequence.SetLoops(-1);
					sequence.SetId(GetInstanceID());
				}
			}
		}

		public void OnSelect()
		{
			if (isSelect)
			{
				return;
			}
			if (PackData.Get().GetCommodity(boosterType).GetTotal() > 0)
			{
				isSelect = true;
				DOTween.Kill(GetInstanceID());
				base.transform.localScale = new Vector3(scale, scale, scale);
				PackData.Get().AddBoosterType(boosterType);
				SelectTransform.gameObject.SetActive(isSelect);
				NormalTransform.gameObject.SetActive(!isSelect);
				if (unityAction != null)
				{
					unityAction(boosterType);
				}
			}
			else
			{
				PackData.Get().RemoveBoosterType(boosterType);
				SingletonClass<MySceneManager>.Get().Popup<ExchangeBoosterScene>("Scenes/ExchangeBoosterScene").OnStart(boosterType, OnSelect);
			}
		}

		private void UpdateNumber(CommoditySource source)
		{
			NumberLabel.text = PackData.Get().GetCommodity(boosterType).GetTotal()
				.ToString();
			NumberTransform.gameObject.SetActive(PackData.Get().GetCommodity(boosterType).GetTotal() > 0);
		}

		public void OnDestroy()
		{
			PackData.Get().GetCommodity(boosterType).OnChanged.RemoveListener(UpdateNumber);
		}
	}
}
