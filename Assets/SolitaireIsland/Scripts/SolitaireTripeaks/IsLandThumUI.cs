using DG.Tweening;
using Nightingale.Utilitys;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	[RequireComponent(typeof(Button))]
	public class IsLandThumUI : MonoBehaviour
	{
		public Transform _FlagParentTransform;

		public Transform _FriendParentTransform;

		public Vector3 locationPosition;

		public Transform _JumpTransform;

		public Transform[] _BoatsTransform;

		private ChapterConfig chapterConfig;

		private DowningProgressUI progressUI;

		private GameObject LockObject;

		private LabelUI _FlagLabelUI;

		private int world;

		public int Chapter
		{
			get;
			private set;
		}

		public void SetInfo(int world, int chapter, UnityAction<int> unityAction)
		{
			bool flag = PlayData.Get().IsLock(world, chapter);
			Chapter = chapter;
			this.world = world;
			SingletonBehaviour<SpecialActivityUtility>.Get().OnRefresh.AddListener(UpdateSpecialActivity);
			UpdateSpecialActivity();
			chapterConfig = UniverseConfig.Get().GetChapterConfig(world, chapter);
			chapterConfig.GetDetailsDownloadTask().AddListener(delegate(object asset, float progress)
			{
				if (asset != null)
				{
					if (progressUI != null)
					{
						UnityEngine.Object.Destroy(progressUI.gameObject);
					}
				}
				else
				{
					if (progressUI == null)
					{
						progressUI = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(SelectionIslandScene).Name, "UI/DowningProgressUI")).GetComponent<DowningProgressUI>();
						progressUI.transform.SetParent(base.transform, worldPositionStays: false);
						(progressUI.transform as RectTransform).anchoredPosition = new Vector2(0f, base.transform.GetComponent<Image>().preferredHeight * 0.5f);
					}
					progressUI.SetProgress(progress);
				}
			});
			if (flag)
			{
				CreateLock();
				base.transform.GetComponent<Image>().color = new Color32(167, 179, 182, byte.MaxValue);
			}
			else
			{
				CreateFlagUI();
			}
			Button component = GetComponent<Button>();
			component.onClick.AddListener(delegate
			{
				if (unityAction != null)
				{
					unityAction(chapter);
				}
			});
		}

		public void CreateBoat()
		{
			for (int i = 0; i < _BoatsTransform.Length; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>($"ui/Boat{Mathf.Min(i + 1, 2):D2}UI"));
				gameObject.transform.SetParent(_BoatsTransform[i], worldPositionStays: false);
			}
		}

		public void AddFriendSchedule(List<TripeaksPlayer> players)
		{
			FriendAvaterUI[] componentsInChildren = _FriendParentTransform.GetComponentsInChildren<FriendAvaterUI>();
			FriendAvaterUI[] array = componentsInChildren;
			foreach (FriendAvaterUI friendAvaterUI in array)
			{
				UnityEngine.Object.Destroy(friendAvaterUI.gameObject);
			}
			if (players == null)
			{
				return;
			}
			TripeaksPlayer[] array2 = (from e in players
				where e.IsInChapter(world, Chapter)
				select e).ToArray();
			int num = 0;
			GameObject asset = SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>("UI/FriendAvaterUI");
			TripeaksPlayer[] array3 = array2;
			foreach (TripeaksPlayer user in array3)
			{
				if (num > 4)
				{
					break;
				}
				num++;
				GameObject gameObject = UnityEngine.Object.Instantiate(asset);
				gameObject.transform.SetParent(_FriendParentTransform, worldPositionStays: false);
				gameObject.GetComponent<FriendAvaterUI>().SetUser(user);
			}
		}

		private void UpdateProgress(string tag, float progress)
		{
			if (!chapterConfig.fileName.Equals(tag))
			{
				return;
			}
			if (progress >= 1f)
			{
				if (progressUI != null)
				{
					UnityEngine.Object.Destroy(progressUI.gameObject);
				}
				return;
			}
			if (progressUI == null)
			{
				progressUI = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(SelectionIslandScene).Name, "UI/DowningProgressUI")).GetComponent<DowningProgressUI>();
				progressUI.transform.SetParent(base.transform, worldPositionStays: false);
				(progressUI.transform as RectTransform).anchoredPosition = new Vector2(0f, base.transform.GetComponent<Image>().preferredHeight * 0.5f);
			}
			progressUI.SetProgress(progress);
		}

		private void UpdateSpecialActivity()
		{
			if (SingletonBehaviour<SpecialActivityUtility>.Get().CreateSpecialActivityInSelectIsland(world, Chapter, base.transform))
			{
				SingletonBehaviour<SpecialActivityUtility>.Get().OnRefresh.RemoveListener(UpdateSpecialActivity);
			}
		}

		private void OnDestroy()
		{
			SingletonBehaviour<SpecialActivityUtility>.Get().OnRefresh.RemoveListener(UpdateSpecialActivity);
			chapterConfig.GetDetailsDownloadTask().RemoveAllListeners();
		}

		public void ShowUnlockAnmtion(UnityAction unityAction)
		{
			CreateLock();
			CreateFlagUI();
			_FlagLabelUI.transform.localScale = Vector3.zero;
			Sequence sequence = DOTween.Sequence();
			sequence.Append(LockObject.transform.DOShakeRotateZ(15f, 10, 1f).SetEase(Ease.Linear));
			sequence.Append(LockObject.transform.DOScale(3f, 0.5f));
			sequence.Join(LockObject.GetComponent<Image>().DOFade(0f, 0.5f));
			sequence.AppendCallback(delegate
			{
				UnityEngine.Object.Destroy(LockObject);
				LockObject = null;
			});
			sequence.Append(_FlagLabelUI.transform.DOScale(1.1f, 0.3f));
			sequence.Append(_FlagLabelUI.transform.DOScale(1f, 0.1f));
			sequence.OnComplete(delegate
			{
				if (unityAction != null)
				{
					unityAction();
				}
			});
		}

		private void CreateLock()
		{
			if (LockObject == null)
			{
				LockObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(SelectionIslandScene).Name, "UI/IslandLockUI"));
				LockObject.transform.SetParent(base.transform, worldPositionStays: false);
			}
		}

		private void CreateFlagUI()
		{
			if (_FlagLabelUI == null)
			{
				int num = chapterConfig.LevelCount * 3;
				int num2 = 0;
				ChapterData chapter = PlayData.Get().GetChapter(world, Chapter);
				if (chapter != null)
				{
					foreach (LevelData lv in chapter.lvs)
					{
						num2 += lv.Star;
					}
				}
				_FlagLabelUI = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(SelectionIslandScene).Name, "UI/FlagUI")).GetComponent<LabelUI>();
				_FlagLabelUI.transform.SetParent(_FlagParentTransform, worldPositionStays: false);
				_FlagLabelUI.SetString($"{num2}/{num}");
			}
		}
	}
}
