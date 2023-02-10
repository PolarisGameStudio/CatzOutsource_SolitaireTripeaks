using DG.Tweening;
using Nightingale.Utilitys;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class LevelStarUI : MonoBehaviour
	{
		public StarType StarType;

		public GameObject DoubleObject;

		public void SetVisable(bool visable)
		{
			base.gameObject.SetActive(visable);
		}

		public void SetLevelData(LevelData levelData)
		{
			if (levelData.StarComplete && StarType == StarType.Complete)
			{
				base.gameObject.SetActive(value: true);
			}
			else if (levelData.StarTime && StarType == StarType.Time)
			{
				base.gameObject.SetActive(value: true);
			}
			else if (levelData.StarSteaks && StarType == StarType.Streak)
			{
				base.gameObject.SetActive(value: true);
			}
			else
			{
				base.gameObject.SetActive(value: false);
			}
		}

		public bool SetAnimationLevelData(LevelData levelData, int index, int starIndex)
		{
			Vector3[] array = new Vector3[3]
			{
				new Vector3(-300f, 500f),
				new Vector3(0f, 500f),
				new Vector3(300f, 500f)
			};
			if (levelData.StarComplete && StarType == StarType.Complete)
			{
				base.gameObject.SetActive(value: true);
			}
			else if (levelData.StarTime && StarType == StarType.Time)
			{
				base.gameObject.SetActive(value: true);
			}
			else
			{
				if (!levelData.StarSteaks || StarType != StarType.Streak)
				{
					base.gameObject.SetActive(value: false);
					return false;
				}
				base.gameObject.SetActive(value: true);
			}
			base.gameObject.SetActive(value: false);
			Vector3 localPosition = base.transform.localPosition;
			base.transform.localPosition = localPosition + array[index];
			Sequence sequence = DOTween.Sequence();
			sequence.PrependInterval((float)(starIndex + 1) * 0.5f);
			sequence.AppendCallback(delegate
			{
				base.gameObject.SetActive(value: true);
				AudioUtility.GetSound().Play($"Audios/Star_{index + 1}.mp3");
			});
			sequence.Append(base.transform.DOLocalMove(localPosition, 0.15f));
			sequence.SetEase(Ease.Linear);
			sequence.OnComplete(delegate
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(LeveEndScene).Name, "Particles/StarParticle"));
				gameObject.GetComponent<ParticleSystem>().DOPlay();
				gameObject.transform.SetParent(base.transform, worldPositionStays: false);
				UnityEngine.Object.Destroy(gameObject, 3f);
			});
			return true;
		}

		private bool IsCompleted(LevelData levelData)
		{
			if (levelData.StarComplete && StarType == StarType.Complete)
			{
				return true;
			}
			if (levelData.StarTime && StarType == StarType.Time)
			{
				return true;
			}
			if (levelData.StarSteaks && StarType == StarType.Streak)
			{
				return true;
			}
			return false;
		}

		public bool SetLeaderBoardAnimationLevelData(LevelData lastLevelData, LevelData newLevelData, int index, int starIndex)
		{
			if (IsCompleted(lastLevelData))
			{
				base.gameObject.SetActive(value: true);
				return false;
			}
			if (IsCompleted(newLevelData))
			{
				base.gameObject.SetActive(value: true);
				base.transform.localScale = Vector3.zero;
				Sequence sequence = DOTween.Sequence();
				sequence.PrependInterval((float)(starIndex + 1) * 0.5f);
				sequence.Append(base.transform.DOScale(1.2f, 0.2f));
				sequence.Append(base.transform.DOScale(1f, 0.1f));
				sequence.SetEase(Ease.Linear);
				if (DoubleObject != null)
				{
					DoubleObject.SetActive(RankCoinData.Get().IsDouble());
				}
				return true;
			}
			base.gameObject.SetActive(value: false);
			return false;
		}
	}
}
