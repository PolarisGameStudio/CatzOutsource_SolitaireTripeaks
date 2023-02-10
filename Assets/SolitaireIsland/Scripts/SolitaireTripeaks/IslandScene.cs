using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class IslandScene : BaseScene
	{
		public Image Background;

		public RectTransform IsLandDetailsParentTransform;

		public MenuUIRight menuUIRight;

		private IsLandDetails IsLandDetails;

		public static IslandScene Create(int world)
		{
			return SingletonClass<MySceneManager>.Get().Navigation<IslandScene>("Scenes/IslandScene");
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			SingletonBehaviour<LoaderUtility>.Get().UnLoadScene(typeof(IslandScene).Name);
		}

		public void OnStart(int world, int chapter)
		{
			if (world == 1)
			{
				Background.color = new Color32(148, 148, 148, byte.MaxValue);
			}
			if (world == 2)
			{
				Background.color = new Color32(99, 99, 99, byte.MaxValue);
			}
			try
			{
				SingletonBehaviour<SpecialActivityUtility>.Get().CheckSpecialActivity();
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
			try
			{
				MenuUITopLeft.CreateMenuUITopLeft(base.transform);
				menuUIRight = MenuUIRight.CreateMenuUIRight(base.transform);
				menuUIRight.CreateInIsland(world, chapter);
			}
			catch (Exception ex2)
			{
				UnityEngine.Debug.Log(ex2.Message);
			}
			try
			{
				ChapterConfig chapterConfig = UniverseConfig.Get().GetChapterConfig(world, chapter);
#if GET_MAP_DETAIL
                IsLandDetails = UnityEngine.Object.Instantiate(GameConfigManager.Instance.MapDetails[0]).GetComponent<IsLandDetails>();
                IsLandDetails.transform.SetParent(IsLandDetailsParentTransform, worldPositionStays: false);
                IsLandDetails.OnStart(this, world, chapter);
#else
				AssetBundle detailsAssetBundle = chapterConfig.GetDetailsAssetBundle();
				IsLandDetails = UnityEngine.Object.Instantiate(detailsAssetBundle.LoadAsset<GameObject>("IsLandDetails")).GetComponent<IsLandDetails>();
				IsLandDetails.transform.SetParent(IsLandDetailsParentTransform, worldPositionStays: false);
				IsLandDetails.OnStart(this, world, chapter);
#endif
            }
			catch (Exception ex3)
			{
				UnityEngine.Debug.Log(ex3.Message);
			}
			try
			{
				Transform transform = IsLandDetails.transform.Find("SmallLight");
				if (transform != null)
				{
					transform.GetComponent<Renderer>().material.shader = Shader.Find("Mobile/Particles/Additive");
				}
			}
			catch (Exception ex4)
			{
				UnityEngine.Debug.Log(ex4.Message);
			}
			OnLoadCompeted();
		}

		public void JumpTo(ScheduleData schedule)
		{
			if (IsLandDetails != null)
			{
				IsLandDetails.JumpTo(schedule, 0.5f);
			}
		}
	}
}
