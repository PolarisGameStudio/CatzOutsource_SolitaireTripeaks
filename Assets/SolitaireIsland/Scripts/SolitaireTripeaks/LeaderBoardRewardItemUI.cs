using Nightingale.Localization;
using Nightingale.Utilitys;
using System.Collections.Generic;
using System.Linq;
using TriPeaks.ProtoData.Leaderboard;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class LeaderBoardRewardItemUI : MonoBehaviour
	{
		public Text TitleLabel;

		public Image StageIcon;

		public Text RewardLabel;

		public Image Background;

		public Image Light;

		public void SetInfo(bool isClan, int stage, int staged, List<RewardItem> rewards)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_LeaderBoard.json");
			TitleLabel.text = localizationUtility.GetString($"Stage_{stage}").ToUpper();
			StageIcon.sprite = SingletonBehaviour<StageIconHelper>.Get().GetSprite(stage, isClan);
			IEnumerable<RewardItem> enumerable = from e in rewards
				where e.Stage == stage
				where e.ItemId == 1
				select e;
			int num = (enumerable != null && enumerable.Count() != 0) ? enumerable.Min((RewardItem e) => e.Amount) : 0;
			int num2 = (enumerable != null && enumerable.Count() != 0) ? enumerable.Max((RewardItem e) => e.Amount) : 0;
			bool flag = stage == staged;
			Background.color = ((!flag) ? new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue) : new Color32(byte.MaxValue, 251, 122, byte.MaxValue));
			Light.gameObject.SetActive(flag);
			string arg = string.Empty + num;
			if (num >= 10000)
			{
				arg = num / 1000 + "K";
			}
			string arg2 = string.Empty + num2;
			if (num2 >= 10000)
			{
				arg2 = num2 / 1000 + "K";
			}
			RewardLabel.text = string.Format(localizationUtility.GetString("{0} to {1}"), arg, arg2);
		}
	}
}
