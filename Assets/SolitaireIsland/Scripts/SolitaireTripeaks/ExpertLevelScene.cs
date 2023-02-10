using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ExpertLevelScene : BaseScene
	{
		public Button NextButton;

		public Button PreButton;

		public Button BackButton;

		public Button CloseButton;

		public LocalizationLabel CompletedLabel;

		public Transform ContentTransform;

		public Transform GroupContentTransform;

		private int PageIndex;

		private void Awake()
		{
			base.IsStay = true;
			NextButton.onClick.AddListener(delegate
			{
				UpdatePage(++PageIndex);
			});
			PreButton.onClick.AddListener(delegate
			{
				UpdatePage(--PageIndex);
			});
			BackButton.onClick.AddListener(delegate
			{
				UpdateGroup();
			});
			CloseButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Close(new PivotScaleEffect(Object.FindObjectOfType<MenuUIRight>().ExportButton.transform.position));
			});
			ExpertWorldConfig worldConfig = SingletonClass<ExpertLevelConfigGroup>.Get().GetWorldConfig();
			if (worldConfig == null)
			{
				return;
			}
			WorldData worldData = PlayData.Get().worlds.Find((WorldData e) => e.world == -1);
			if (worldData == null)
			{
				CompletedLabel.SetText(0, worldConfig.GetPoints().Count);
				UpdatePage(0);
				return;
			}
			CompletedLabel.SetText(worldData.GetLevels(), worldConfig.GetPoints().Count);
			int num = worldData.chapters.Count - 1;
			ChapterData chapterData = worldData.chapters[worldData.chapters.Count - 1];
			if (chapterData.lvs.Count == 10)
			{
				num++;
			}
			UpdatePage(num);
		}

		private void UpdateGroup()
		{
			BackButton.gameObject.SetActive(value: false);
			NextButton.gameObject.SetActive(value: false);
			PreButton.gameObject.SetActive(value: false);
			ContentTransform.gameObject.SetActive(value: false);
			GroupContentTransform.gameObject.SetActive(value: true);
			int count = SingletonClass<ExpertLevelConfigGroup>.Get().GetWorldConfig().GetPoints()
				.Count;
				int groupNumbers = Mathf.CeilToInt((float)count / 6f);
				groupNumbers = ((groupNumbers - 1) / 10 + 1) * 10;
				groupNumbers = Mathf.Max(groupNumbers, 10);
				int num = (count - 1) / groupNumbers + 1;
				List<ExpertGroupUI> list = GroupContentTransform.gameObject.GetComponentsInChildren<ExpertGroupUI>().ToList();
				foreach (ExpertGroupUI item in list)
				{
					item.gameObject.SetActive(value: false);
				}
				for (int i = 0; i < num; i++)
				{
					ExpertGroupUI expertGroupUI = list.Find((ExpertGroupUI e) => !e.gameObject.activeSelf);
					if (expertGroupUI == null)
					{
						GameObject gameObject = Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(ExpertLevelScene).Name, "UI/ExpertGroupUI"));
						gameObject.transform.SetParent(GroupContentTransform, worldPositionStays: false);
						expertGroupUI = gameObject.GetComponent<ExpertGroupUI>();
					}
					expertGroupUI.gameObject.SetActive(value: true);
					int num2 = i * groupNumbers + 1;
					int a = (i + 1) * groupNumbers;
					a = Mathf.Min(a, count);
					expertGroupUI.OnStart(i, $"{num2}-{a}", delegate(int index)
					{
						UpdatePage(index * groupNumbers / 10);
					});
				}
			}

			private void SetColor(Transform transform, Color color)
			{
				Image[] componentsInChildren = transform.GetComponentsInChildren<Image>();
				Image[] array = componentsInChildren;
				foreach (Image image in array)
				{
					image.color = color;
				}
			}

			private void UpdatePage(int index)
			{
				NextButton.gameObject.SetActive(value: true);
				PreButton.gameObject.SetActive(value: true);
				ContentTransform.gameObject.SetActive(value: true);
				GroupContentTransform.gameObject.SetActive(value: false);
				ExpertWorldConfig worldConfig = SingletonClass<ExpertLevelConfigGroup>.Get().GetWorldConfig();
				int count = worldConfig.chapters.Count;
				PageIndex = index;
				PageIndex = Mathf.Min(count - 1, PageIndex);
				PageIndex = Mathf.Max(0, PageIndex);
				BackButton.gameObject.SetActive(worldConfig.GetPoints().Count >= 50);
				NextButton.interactable = (PageIndex < count - 1);
				SetColor(NextButton.transform, (!NextButton.interactable) ? Color.gray : Color.white);
				PreButton.interactable = (PageIndex > 0);
				SetColor(PreButton.transform, (!PreButton.interactable) ? Color.gray : Color.white);
				List<LevelControl> list = ContentTransform.gameObject.GetComponentsInChildren<LevelControl>().ToList();
				foreach (LevelControl item in list)
				{
					item.gameObject.SetActive(value: false);
				}
				ChapterConfig chapterConfig = worldConfig.chapters[PageIndex];
				for (int i = 0; i < chapterConfig.LevelCount; i++)
				{
					LevelControl levelControl = list.Find((LevelControl e) => !e.gameObject.activeSelf);
					if (levelControl == null)
					{
						GameObject gameObject = Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(ExpertLevelScene).Name, "UI/ExpertLevel"));
						gameObject.transform.SetParent(ContentTransform, worldPositionStays: false);
						levelControl = gameObject.GetComponent<LevelControl>();
					}
					levelControl.gameObject.SetActive(value: true);
					levelControl.SetInfo(new ScheduleData(-1, PageIndex, i), delegate(ScheduleData schedule)
					{
						SingletonClass<AAOConfig>.Get().SetPlaySchedule(schedule);
						SingletonClass<MySceneManager>.Get().Popup<LevelScene>("Scenes/LevelScene", new NavigationEffect());
					});
				}
			}
		}
	}
