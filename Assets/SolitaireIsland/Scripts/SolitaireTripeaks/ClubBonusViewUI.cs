using Nightingale.Extensions;
using Nightingale.Utilitys;
using TriPeaks.ProtoData.Club;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class ClubBonusViewUI : DelayBehaviour
	{
		private void Awake()
		{
			MyClubResponse _MyClubResponse = SingletonBehaviour<ClubSystemHelper>.Get()._MyClubResponse;
			ClubBonusItemUI[] bonusItemUIs = base.gameObject.GetComponentsInChildren<ClubBonusItemUI>();
			int index;
			for (index = 0; index < bonusItemUIs.Length; index++)
			{
				if (_MyClubResponse == null || _MyClubResponse.Club == null || _MyClubResponse.LevelConfigs == null)
				{
					bonusItemUIs[index].gameObject.SetActive(value: false);
					continue;
				}
				ClubLevelConfig clubLevelConfig = _MyClubResponse.LevelConfigs.ToList().Find((ClubLevelConfig e) => e.Level == index);
				bonusItemUIs[index].SetInfo(_MyClubResponse.Club.Level, index, _MyClubResponse.Club.Score, clubLevelConfig?.MinScore ?? 0, clubLevelConfig?.MaxScore ?? 0);
			}
			DelayDo(delegate
			{
				if (_MyClubResponse.Club != null && _MyClubResponse.Club.Level - 1 >= 0 && _MyClubResponse.Club.Level - 1 <= bonusItemUIs.Length - 1)
				{
					RectTransformHelper.Center(bonusItemUIs[_MyClubResponse.Club.Level - 1].transform as RectTransform, CenterDir.Vertical);
				}
			});
		}
	}
}
