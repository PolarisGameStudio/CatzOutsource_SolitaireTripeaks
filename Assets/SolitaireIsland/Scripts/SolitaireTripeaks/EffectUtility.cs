using DG.Tweening;
using Nightingale.U2D;
using Nightingale.Utilitys;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class EffectUtility : SingletonBehaviour<EffectUtility>
	{
		public void CreateBoosterType(BoosterType boosterType, Vector3 position = default(Vector3))
		{
			switch (boosterType)
			{
			case BoosterType.Coins:
				CreateCoinEffect(position);
				break;
			}
			PackData.Get().GetCommodity(boosterType).PutChanged(CommoditySource.Free);
		}

		public void CreateCoinEffect(Vector3 position = default(Vector3))
		{
			AudioUtility.GetSound().Play("Audios/GetCoins.mp3");
			GameObject gameObject = new GameObject("Coin Effect");
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			Sprite sprite = SingletonBehaviour<LoaderUtility>.Get().GetAssetComponent<SpriteManager>(typeof(PlayScene).Name, "Sprites/SpecialPoker").GetSprite("coin_normal");
			Transform transform = CreateSpriteRenderer(gameObject.transform, sprite);
			transform.position = position;
			transform.localScale = Vector3.zero;
			transform.DOScale(1.5f, 0.2f).OnComplete(delegate
			{
				AudioUtility.GetSound().Play("Audios/GetCoins_2.mp3");
				Sequence sequence = DOTween.Sequence();
				for (int i = 0; i < 10; i++)
				{
					sequence.AppendInterval(0.1f);
					sequence.AppendCallback(delegate
					{
						Transform coinTransform = CreateSpriteRenderer(gameObject.transform, sprite);
						coinTransform.position = position;
						coinTransform.localScale = Vector3.zero;
						coinTransform.DOScale(1f, 0.15f).OnComplete(delegate
						{
							coinTransform.DOMove(MenuUITopLeft.GetMenu().CoinTransform.position, 0.6f).OnComplete(delegate
							{
								UnityEngine.Object.Destroy(coinTransform.gameObject);
							});
						});
					});
				}
				sequence.OnComplete(delegate
				{
					UnityEngine.Object.Destroy(transform.gameObject);
				});
			});
		}

		private Transform CreateSpriteRenderer(Transform transform, Sprite sprite)
		{
			GameObject gameObject = new GameObject("Coin");
			gameObject.transform.SetParent(transform, worldPositionStays: false);
			SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = sprite;
			spriteRenderer.sortingLayerName = "TopLayer";
			return gameObject.transform;
		}
	}
}
