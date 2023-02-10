using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class SynchronizeUI : MonoBehaviour
	{
		public Text LevelLabel;

		public Text CoinsLabel;

		public Button SelectButton;

		public void SetInfo(SolitaireTripeaksData data, bool remote, UnityAction unityAction)
		{
			int max = data.Play.GetMax();
			LevelLabel.text = max.ToString();
			if (max == 0 && data.Play.HasLevelData(0, 0, 1))
			{
				LevelLabel.text = $"{UniverseConfig.Get().GetAllScheduleDatas().Count}+";
			}
			CoinsLabel.text = data.Pack.GetCommodity(BoosterType.Coins).GetTotal().ToString();
			SelectButton.onClick.AddListener(delegate
			{
				if (remote)
				{
					SolitaireTripeaksData.Get().Disable();
					SolitaireTripeaksData.Put(data);
					SingletonClass<MySceneManager>.Get().Close(new NavigationEffect());
					SingletonClass<MySceneManager>.Get().Navigation<LoadingScene>("Scenes/LoadingScene");
					SingletonBehaviour<LeaderBoardUtility>.Get().OnAppStart();
					SingletonBehaviour<ClubSystemHelper>.Get().OnAppStart();
					SingletonBehaviour<MessageUtility>.Get().OnAppStart();
				}
				else
				{
					SingletonClass<MySceneManager>.Get().Close(new JoinEffect());
					SingletonBehaviour<TripeaksLogUtility>.Get().UploadMinSynchronize();
				}
				if (unityAction != null)
				{
					unityAction();
				}
			});
		}
	}
}
