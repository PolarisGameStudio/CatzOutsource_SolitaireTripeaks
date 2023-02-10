using DG.Tweening;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class Effect2DUtility : SingletonBehaviour<Effect2DUtility>
	{
		public void CreateBoosterUseEffectUI(BoosterType boosterType, UnityAction unityAction = null)
		{
			GameObject gameObject = Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, "UI/BoosterUseEffect"));
			gameObject.GetComponent<BoosterUseEffectUI>().OnStart(boosterType, unityAction);
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
		}

		public LabelUI CreateScoreUI(Vector3 position, int score, UnityAction unityAction)
		{
			return CreateScoreUI(position, score.ToString(), unityAction);
		}

		public LabelUI CreateScoreUI(Vector3 position, string score, UnityAction unityAction)
		{
			GameObject asset = SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, "UI/ScoreUI");
			LabelUI ScoreLabel = Object.Instantiate(asset).GetComponent<LabelUI>();
			ScoreLabel.transform.position = position;
			ScoreLabel.transform.SetParent(base.transform, worldPositionStays: true);
			Vector3 localPosition = ScoreLabel.transform.localPosition;
			localPosition.z = 0f;
			ScoreLabel.transform.localPosition = localPosition;
			ScoreLabel.transform.localScale = Vector3.one * 1.1f;
			ScoreLabel.SetString($"{score}");
			ScoreLabel.transform.DOLocalMoveY(localPosition.y + 100f, 1f).OnComplete(delegate
			{
				UnityEngine.Object.Destroy(ScoreLabel.gameObject);
				if (unityAction != null)
				{
					unityAction();
				}
			});
			Sequence s = DOTween.Sequence();
			s.PrependInterval(0.5f);
			s.AppendCallback(delegate
			{
				ScoreLabel.CrossFadeAlpha(0.5f);
			});
			return ScoreLabel;
		}

		public TextTipsUI CreateTextTipsUI(string message, float height = 0f, UnityAction unityAction = null)
		{
			GameObject gameObject = Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, "UI/TextTips"));
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			gameObject.transform.localPosition = new Vector3(0f, height, 0f);
			TextTipsUI textTipsUI = gameObject.GetComponent<TextTipsUI>();
			textTipsUI.MessageLabel.text = message.ToUpper();
			textTipsUI.BackgroundClone.DOFade(1f, 0.3f).SetEase(Ease.Linear);
			textTipsUI.Background.transform.localPosition = new Vector3(-1920f, 0f, 0f);
			textTipsUI.Background.transform.DOLocalMoveX(1920f, 0.5f).SetEase(Ease.Linear);
			textTipsUI.MessageLabel.transform.localPosition = new Vector3(-140f, 0f, 0f);
			textTipsUI.MessageLabel.transform.DOLocalMoveX(100f, 1f).OnComplete(delegate
			{
				UnityEngine.Object.Destroy(textTipsUI.gameObject);
				if (unityAction != null)
				{
					unityAction();
				}
			}).SetEase(Ease.Linear);
			return textTipsUI;
		}

		public TitleIconLabelUI CreateTitleIconLabelUI(Vector3 position, string title, string number, UnityAction unityAction)
		{
			GameObject g = Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, "UI/TitleIconLabelUI"));
			g.transform.SetParent(base.transform, worldPositionStays: false);
			g.transform.position = position;
			TitleIconLabelUI titleIconLabelUI = g.GetComponent<TitleIconLabelUI>();
			titleIconLabelUI.SetInf(title, number);
			Transform transform = g.transform;
			Vector3 localPosition = g.transform.localPosition;
			transform.DOLocalMoveY(localPosition.y + 100f, 1f).OnComplete(delegate
			{
				UnityEngine.Object.Destroy(g.gameObject);
			});
			Sequence s = DOTween.Sequence();
			s.PrependInterval(0.5f);
			s.AppendCallback(delegate
			{
				titleIconLabelUI.CrossFadeAlpha(0.5f);
			});
			s.AppendInterval(0.3f);
			s.AppendCallback(delegate
			{
				if (unityAction != null)
				{
					unityAction();
				}
			});
			return titleIconLabelUI;
		}

		public Text CreateText(string message, Vector3 position)
		{
			GameObject g = Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>("UI/TextLabel"));
			g.transform.localPosition = position;
			g.transform.SetParent(base.transform, worldPositionStays: true);
			Vector3 localPosition = g.transform.localPosition;
			localPosition.z = 0f;
			g.transform.localPosition = localPosition;
			g.transform.localScale = Vector3.one * 1.1f;
			g.transform.DOLocalMoveY(localPosition.y + 100f, 1f).OnComplete(delegate
			{
				UnityEngine.Object.Destroy(g);
			});
			Text component = g.GetComponent<Text>();
			component.text = message;
			return component;
		}
	}
}
