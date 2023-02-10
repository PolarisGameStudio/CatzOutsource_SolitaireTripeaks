using DG.Tweening;
using Nightingale.Extensions;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class PlayStreaksSystem : MonoBehaviour
	{
		private static PlayStreaksSystem _system;

		public UnityEvent StreaksLevelChanged = new UnityEvent();

		public UnityEvent StreaksGradeChanged = new UnityEvent();

		public int StreaksLevel;

		public int StreaksGrade;

		public int RealLinkCount;

		private int currentMaxLevel;

		public MultipleImageUI _MultipleImageUI;

		public ProgressBarUI _StepProgressBarUI;

		private const int max = 3;

		private const int _link = 5;

		private Sequence sequence;

		public static PlayStreaksSystem Get()
		{
			if (_system == null)
			{
				_system = UnityEngine.Object.FindObjectOfType<PlayStreaksSystem>();
			}
			return _system;
		}

		private void Awake()
		{
			_system = this;
		}

		private void OnDestroy()
		{
			StreaksLevelChanged.RemoveAllListeners();
			StreaksGradeChanged.RemoveAllListeners();
			_system = null;
		}

		private void Start()
		{
			_StepProgressBarUI.SetFillAmount(0f);
			_MultipleImageUI.SetImage(0);
		}

		public bool GetFirstStar()
		{
			return StreaksGrade >= 1;
		}

		public bool GetSecondStar()
		{
			return StreaksGrade >= 3;
		}

		private float GetProgress(int index)
		{
			float[] array = new float[6]
			{
				0.12f,
				0.25f,
				0.4f,
				0.58f,
				0.75f,
				1f
			};
			if (index < array.Length)
			{
				return array[index];
			}
			return 0f;
		}

		private void UpdateStepProgressBarUI(bool delay = false)
		{
			TweenCallback tweenCallback = delegate
			{
				if (StreaksGrade >= 3)
				{
					_StepProgressBarUI.SetFillAmount(GetProgress(5));
				}
				else
				{
					_StepProgressBarUI.UpdateFillAmount(GetProgress(StreaksLevel));
				}
				_MultipleImageUI.SetImage(StreaksGrade);
			};
			if (sequence != null)
			{
				sequence.Kill(complete: true);
				sequence = null;
			}
			if (delay)
			{
				sequence = DOTween.Sequence();
				sequence.AppendInterval(0.5f);
				sequence.AppendCallback(tweenCallback);
			}
			else
			{
				tweenCallback();
			}
		}

		public void SetCurrentLink(int linkGrade, int linkLevel, int linkCount)
		{
			StreaksGrade = linkGrade;
			StreaksLevel = linkLevel;
			RealLinkCount = linkCount;
			UpdateStepProgressBarUI();
		}

		public void StreaksOnce()
		{
			RealLinkCount++;
			StreaksLevel += GlobalBoosterUtility.Get().MultipleStreaks();
			if (StreaksLevel >= 5)
			{
				_StepProgressBarUI.UpdateFillAmount(GetProgress(5));
				StreaksLevel %= 5;
				StreaksGrade++;
				if (StreaksGrade > 3)
				{
					return;
				}
				UpdateStepProgressBarUI(delay: true);
				this.DelayDo(new WaitForEndOfFrame(), delegate
				{
					StreaksGradeChanged.Invoke();
				});
			}
			else
			{
				UpdateStepProgressBarUI();
			}
			this.DelayDo(new WaitForEndOfFrame(), delegate
			{
				StreaksLevelChanged.Invoke();
			});
			if (StreaksGrade > currentMaxLevel)
			{
				currentMaxLevel = StreaksGrade;
				if (currentMaxLevel > 3)
				{
					return;
				}
				ScoringSystem.Get().CreateScoreUIByStreaksNode(Vector3.zero, currentMaxLevel, null, music: false);
				SingletonBehaviour<EffectUtility>.Get().CreateCoinEffect(_MultipleImageUI.transform.position);
				if (currentMaxLevel == 3)
				{
					this.DelayDo(new WaitForSeconds(1.5f), delegate
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, "Particles/StarGetParticle"));
						gameObject.transform.position = _MultipleImageUI.transform.position;
						UnityEngine.Object.Destroy(gameObject, 4f);
					});
				}
			}
			SingletonClass<QuestHelper>.Get().DoQuest(QuestType.GetStreak, StreaksGrade * 5 + StreaksLevel);
		}

		public void ChangeHand()
		{
			RealLinkCount = 0;
			StreaksLevel = 0;
			UpdateStepProgressBarUI();
		}

		public void DoHideAnimtor(UnityAction unityAction = null)
		{
			base.transform.DOLocalMoveY(120f, 0.5f).OnComplete(delegate
			{
				if (unityAction != null)
				{
					unityAction();
				}
			});
		}
	}
}
