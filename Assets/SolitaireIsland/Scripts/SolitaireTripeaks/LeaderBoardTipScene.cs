using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class LeaderBoardTipScene : BaseScene
	{
		public Image StageImage;

		public Text MaxRewardLabel;

		public Text CurrentRewardLabel;

		public Button StageButton;

		private void Start()
		{
			base.IsStay = true;
			StageImage.sprite = SingletonBehaviour<StageIconHelper>.Get().GetSprite((int)RankCoinData.Get().Staged);
			StageImage.SetNativeSize();
			CurrentRewardLabel.text = SingletonBehaviour<LeaderBoardUtility>.Get().GetCoins((int)RankCoinData.Get().Staged, SingletonData<RankCache>.Get().Rank).ToString();
			Text maxRewardLabel = MaxRewardLabel;
			Vector2 coins = SingletonBehaviour<LeaderBoardUtility>.Get().GetCoins((int)RankCoinData.Get().Staged);
			maxRewardLabel.text = coins.y.ToString();
			StageButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Popup<LeaderboarGuidePopup>("Scenes/Pops/LeaderboarGuidePopup").OnStart(isClan: false, RankCoinData.Get().Staged, SingletonBehaviour<LeaderBoardUtility>.Get().GetRewards());
			});
		}
	}
}
