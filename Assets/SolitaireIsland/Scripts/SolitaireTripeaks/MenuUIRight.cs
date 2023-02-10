using com.F4A.MobileThird;
using DG.Tweening;
using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Socials;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class MenuUIRight : MonoBehaviour
	{
		public Button _WorldButton;

		public Button _MapButton;

		public Text _FacebookText;

		public Button FacebookButton;

		public Button InBoxButton;

		public Button FreeCoinsButton;

		public Button QuestButton;

		public Button PackageButton;

		public Button AvaterButton;

		public Button PokerThemeButton;

		public Button GroupButton;

		public Button ClubButton;

		public Button ExportButton;

		public RectTransform ArrowRectTransform;

		private bool isHiding;

		private Sequence sequence;

		private static bool _IsExpertDaily = true;

		public static MenuUIRight GetMenu()
		{
			return UnityEngine.Object.FindObjectOfType<MenuUIRight>();
		}

		public static MenuUIRight CreateMenuUIRight(Transform transform)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(SelectionIslandScene).Name, "UI/MenuUI_Right"));
			gameObject.transform.SetParent(transform.Find("Canvas"), worldPositionStays: false);
			return gameObject.GetComponent<MenuUIRight>();
		}

		private void OnDestroy()
		{
			SingletonBehaviour<FacebookMananger>.Get().LoginChanged.RemoveListener(UpdateFacebook);
		}

		private void Awake()
		{
			UpdateFacebook(SingletonBehaviour<FacebookMananger>.Get().IsLogin());
			SingletonBehaviour<FacebookMananger>.Get().LoginChanged.AddListener(UpdateFacebook);
			SingletonBehaviour<GlobalConfig>.Get().CreateExclamationMark(ExportButton.gameObject, _IsExpertDaily && PlayData.Get().MustPlayMasterLevel());
			ExportButton.gameObject.SetActive(PlayData.Get().HasThanLevelData(0, 4, 18));
			ExportButton.onClick.AddListener(delegate
			{
				_IsExpertDaily = false;
				SingletonBehaviour<GlobalConfig>.Get().CreateExclamationMark(ExportButton.gameObject, number: false);
				SingletonClass<MySceneManager>.Get().Popup<ExpertLevelScene>("Scenes/ExpertLevelScene");
			});
			FacebookButton.onClick.AddListener(delegate
			{
				SingletonBehaviour<GlobalConfig>.Get().ShowLoginFacebook(AuxiliaryData.Get().IsFacebookReward);
			});
			InBoxButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Popup<InboxScene>("Scenes/InboxScene", new JoinEffect());
			});
			FreeCoinsButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Popup<FreeCoinScene>("Scenes/FreeCoinScene", new JoinEffect()).OnStart();
			});
			FreeCoinsButton.gameObject.SetActive(PlayData.Get().HasThanLevelData(0, 0, 0));
			PackageButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Popup<BagBoosterScene>("Scenes/BagBoosterScene");
			});
			QuestButton.onClick.AddListener(delegate
			{
				QuestsScene.ShowQuest(hasGoButton: true);
				SingletonBehaviour<GlobalConfig>.Get().CreateExclamationMark(QuestButton.gameObject, number: false);
			});
			SingletonBehaviour<GlobalConfig>.Get().CreateExclamationMark(QuestButton.gameObject, QuestData.Get().GetNumber() > 0);
			ClubButton.gameObject.SetActive(SingletonBehaviour<ClubSystemHelper>.Get().IsActive());
			ClubButton.onClick.AddListener(delegate
			{
				SingletonBehaviour<ClubSystemHelper>.Get().ShowClubScene();
			});
			SingletonBehaviour<GlobalConfig>.Get().CreateNumber(PokerThemeButton.gameObject, 1f, PokerThemeGroup.Get().UseableCount());
			PokerThemeButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Popup<PokerThemeScene>("Scenes/PokerThemeScene", new JoinEffect());
				SingletonBehaviour<GlobalConfig>.Get().CreateNumber(PokerThemeButton.gameObject, 1f, 0);
			});
			AchievementData.Get().CalcAchievement();
			AvaterButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Popup<AchievementScene>("Scenes/AchievementScene");
			});
			GroupButton.onClick.AddListener(delegate
			{
				if (isHiding)
				{
					OpenButtons(animator: true);
				}
				else
				{
					HideButtons(animator: true);
				}
			});
		}

		private void DelayAwake()
		{
			HideButtons(animator: true);
		}

		public void CreateInChapterSelect(int world)
		{
			PokerThemeButton.gameObject.SetActive(PokerData.Get().IsPokerThemeOpen());
			QuestButton.gameObject.SetActive(QuestsScene.IsOpen());
			_MapButton.gameObject.SetActive(value: false);
#if ENABLE_WORLD
			_WorldButton.gameObject.SetActive(UniverseConfig.Get().worlds.Count > 1);
			_WorldButton.onClick.AddListener(delegate
			{
				GlobalLoadingAnimation.Show("Scenes/LoadingGameScene", delegate
				{
					WorldScene worldScene = SingletonClass<MySceneManager>.Get().Navigation<WorldScene>("Scenes/WorldScene");
					worldScene.OnStart(world);
					return worldScene;
				});
			});
#endif
			HideButtons();
		}

		public void CreateInIsland(int world, int chapter)
		{
			PokerThemeButton.gameObject.SetActive(PokerData.Get().IsPokerThemeOpen());
			QuestButton.gameObject.SetActive(QuestsScene.IsOpen());
			_WorldButton.gameObject.SetActive(value: false);
			_MapButton.gameObject.SetActive(value: true);
			_MapButton.onClick.AddListener(delegate
			{
				JoinPlayHelper.JoinSelectionIslandScene(world, delegate(bool success)
				{
					if (!success)
					{
						JoinPlayHelper.JoinToIslandDetailScene();
					}
				});
				JoinPlayHelper.TryDownload(force: true);
			});
			OpenButtons();
			Invoke("DelayAwake", 2f);
		}

		public void OpenButtonsWithTips()
		{
			OpenButtons();
			CancelInvoke("DelayAwake");
		}

		private void OpenButtons(bool animator = false)
		{
			if (sequence != null)
			{
				sequence.Kill();
			}
			AvaterButton.gameObject.SetActive(value: true);
			PokerThemeButton.gameObject.SetActive(PokerData.Get().IsPokerThemeOpen());
			QuestButton.gameObject.SetActive(QuestsScene.IsOpen());
			PackageButton.gameObject.SetActive(value: true);
			int num = 0;
			if (animator)
			{
				sequence = DOTween.Sequence();
				sequence.Append((AvaterButton.transform as RectTransform).DOAnchorPos(new Vector3(445f + (float)num++ * 178f, 88.5f), 0.2f));
				if (PokerData.Get().IsPokerThemeOpen())
				{
					sequence.Join((PokerThemeButton.transform as RectTransform).DOAnchorPos(new Vector3(445f + (float)num++ * 178f, 88.5f), 0.2f));
				}
				if (QuestButton.gameObject.activeSelf)
				{
					sequence.Join((QuestButton.transform as RectTransform).DOAnchorPos(new Vector3(445f + (float)num++ * 178f, 88.5f), 0.2f));
				}
				if (PackageButton.gameObject.activeSelf)
				{
					sequence.Join((PackageButton.transform as RectTransform).DOAnchorPos(new Vector3(445f + (float)num++ * 178f, 88.5f), 0.2f));
				}
				ArrowRectTransform.DOLocalRotate(Vector3.zero, 0.2f);
			}
			else
			{
				(AvaterButton.transform as RectTransform).anchoredPosition = new Vector3(445f + (float)num++ * 178f, 88.5f);
				if (PokerData.Get().IsPokerThemeOpen())
				{
					(PokerThemeButton.transform as RectTransform).anchoredPosition = new Vector3(445f + (float)num++ * 178f, 88.5f);
				}
				if (QuestButton.gameObject.activeSelf)
				{
					(QuestButton.transform as RectTransform).anchoredPosition = new Vector3(445f + (float)num++ * 178f, 88.5f);
				}
				if (PackageButton.gameObject.activeSelf)
				{
					(PackageButton.transform as RectTransform).anchoredPosition = new Vector3(445f + (float)num++ * 178f, 88.5f);
				}
				ArrowRectTransform.localEulerAngles = Vector3.zero;
			}
			isHiding = false;
		}

		private void HideButtons(bool animator = false)
		{
			if (sequence != null)
			{
				sequence.Kill();
			}
			Vector2 vector = new Vector2(267f, 88.5f);
			if (animator)
			{
				sequence = DOTween.Sequence();
				sequence.Append((AvaterButton.transform as RectTransform).DOAnchorPos(vector, 0.2f));
				sequence.Join((PokerThemeButton.transform as RectTransform).DOAnchorPos(vector, 0.2f));
				sequence.Join((QuestButton.transform as RectTransform).DOAnchorPos(vector, 0.2f));
				sequence.Join((PackageButton.transform as RectTransform).DOAnchorPos(vector, 0.2f));
				sequence.OnComplete(delegate
				{
					HideButtons();
				});
				ArrowRectTransform.DOLocalRotate(new Vector3(0f, 180f, 0f), 0.2f);
			}
			else
			{
				AvaterButton.gameObject.SetActive(value: false);
				PokerThemeButton.gameObject.SetActive(value: false);
				QuestButton.gameObject.SetActive(value: false);
				PackageButton.gameObject.SetActive(value: false);
				(AvaterButton.transform as RectTransform).anchoredPosition = vector;
				(PokerThemeButton.transform as RectTransform).anchoredPosition = vector;
				(QuestButton.transform as RectTransform).anchoredPosition = vector;
				(PackageButton.transform as RectTransform).anchoredPosition = vector;
				ArrowRectTransform.localEulerAngles = new Vector3(0f, 180f, 0f);
			}
			isHiding = true;
		}

		private void UpdateFacebook(bool login)
		{
			_FacebookText.text = ((!login) ? LocalizationUtility.Get().GetString("btn_facebook_login") : LocalizationUtility.Get().GetString("btn_facebook_friends"));
		}

		public void HandleBtnFacebookPage_Click()
        {
			SocialManager.Instance.OpenLinkFacebookPage();
        }
	}
}
